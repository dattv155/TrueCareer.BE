using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TrueCareer.Services.MAppUser
{
    public interface IAppUserService :  IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> Import(List<AppUser> AppUsers);
        Task<AppUserFilter> ToFilter(AppUserFilter AppUserFilter);
        Task<AppUser> Login(AppUser AppUser);
        Task<AppUser> Register(AppUser AppUser);
        Task<AppUser> ChangePassword(AppUser AppUser);
        Task<AppUser> ForgotPassword(AppUser AppUser);
        Task<AppUser> VerifyOtpCode(AppUser AppUser);
        Task<AppUser> RecoveryPassword(AppUser AppUser);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;
        private IConfiguration Configuration;

        public AppUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IAppUserValidator AppUserValidator,
            ILogging Logging,
            IConfiguration Configuration
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
            this.Configuration = Configuration;
            this.AppUserValidator = AppUserValidator;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return 0;
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            await AppUserValidator.Get(AppUser);
            if (AppUser == null)
                return null;
            return AppUser;
        }
        
        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                AppUser.Id = 0;
                var Password = AppUser.Password;
                AppUser.Password = HashPassword(Password);
                await UOW.AppUserRepository.Create(AppUser);
                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                Logging.CreateAuditLog(AppUser, new { }, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);

                await UOW.AppUserRepository.Update(AppUser);

                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                Logging.CreateAuditLog(AppUser, oldData, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Delete(AppUser);
                Logging.CreateAuditLog(new { }, AppUser, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {
                await UOW.AppUserRepository.BulkDelete(AppUsers);
                Logging.CreateAuditLog(new { }, AppUsers, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;

        }
        
        public async Task<List<AppUser>> Import(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;
            try
            {
                await UOW.AppUserRepository.BulkMerge(AppUsers);

                Logging.CreateAuditLog(AppUsers, new { }, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }     
        
        public async Task<AppUserFilter> ToFilter(AppUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AppUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AppUserFilter subFilter = new AppUserFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Username))
                        subFilter.Username = FilterBuilder.Merge(subFilter.Username, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Email))
                        subFilter.Email = FilterBuilder.Merge(subFilter.Email, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Phone))
                        subFilter.Phone = FilterBuilder.Merge(subFilter.Phone, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Password))
                        subFilter.Password = FilterBuilder.Merge(subFilter.Password, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DisplayName))
                        subFilter.DisplayName = FilterBuilder.Merge(subFilter.DisplayName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SexId))
                        subFilter.SexId = FilterBuilder.Merge(subFilter.SexId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Birthday))
                        subFilter.Birthday = FilterBuilder.Merge(subFilter.Birthday, FilterPermissionDefinition.DateFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Avatar))
                        subFilter.Avatar = FilterBuilder.Merge(subFilter.Avatar, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CoverImage))
                        subFilter.CoverImage = FilterBuilder.Merge(subFilter.CoverImage, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<AppUser> AppUsers)
        {
            List<Sex> Sexes = new List<Sex>();
            Sexes.AddRange(AppUsers.Select(x => new Sex { Id = x.SexId }));
            
            Sexes = Sexes.Distinct().ToList();
            RabbitManager.PublishList(Sexes, RoutingKeyEnum.SexUsed.Code);
        }
        private string CreateToken(long id, string userName, Guid rowId, double? expiredTime = null)
        {
            if (expiredTime == null)
                expiredTime = double.TryParse(Configuration["Config:ExpiredTime"], out double time) ? time : 0;

            string PrivateRSAKeyBase64 = Configuration["Config:PrivateRSAKey"];
            byte[] PrivateRSAKeyBytes = Convert.FromBase64String(PrivateRSAKeyBase64);
            string PrivateRSAKey = Encoding.Default.GetString(PrivateRSAKeyBytes);

            RSAParameters rsaParams;
            using (var tr = new StringReader(PrivateRSAKey))
            {
                var pemReader = new PemReader(tr);
                var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                if (keyPair == null)
                {
                    throw new Exception("Could not read RSA private key");
                }
                var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
            }

            RSA rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
            var jwt = new JwtSecurityToken(
                claims: new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.PrimarySid, rowId.ToString()),
                    
                },
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(expiredTime.Value),
                signingCredentials: signingCredentials
            );

            string Token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Token;
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public async Task<AppUser> Login(AppUser AppUser)
        {
            if (!await AppUserValidator.Login(AppUser))
                return AppUser;
            AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
            CurrentContext.UserId = AppUser.Id;
            AppUser.Token = CreateToken(AppUser.Id, AppUser.Username, AppUser.RowId);
            return AppUser;
        }

        public async Task<AppUser> Register(AppUser AppUser)
        {
            if (!await AppUserValidator.Register(AppUser))
                return AppUser;

            try
            {
                AppUser.Id = 0;
                var Password = AppUser.Password;
                AppUser.Password = HashPassword(Password);
                await UOW.AppUserRepository.Create(AppUser);
                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                Logging.CreateAuditLog(AppUser, new { }, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> ChangePassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ChangePassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);

                await UOW.AppUserRepository.Update(oldData);

                var newData = await UOW.AppUserRepository.Get(AppUser.Id);

                Mail mail = new Mail
                {
                    Subject = "Change Password AppUser",
                    Body = $"Your password has been changed at {StaticParams.DateTimeNow.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")}",
                    Recipients = new List<string> { newData.Email },
                    RowId = Guid.NewGuid()
                };
                RabbitManager.PublishSingle(mail, "Mail.Send");
                Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> ForgotPassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ForgotPassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = 1,
                    Email = new StringFilter { Equal = AppUser.Email },
                    Selects = AppUserSelect.ALL
                })).FirstOrDefault();

                CurrentContext.UserId = oldData.Id;

                oldData.OtpCode = GenerateOTPCode();
                oldData.OtpExpired = StaticParams.DateTimeNow.AddHours(1);


                await UOW.AppUserRepository.Update(oldData);


                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                Mail mail = new Mail
                {
                    Subject = "Otp Code",
                    Body = $"Otp Code recovery password: {newData.OtpCode}",
                    Recipients = new List<string> { newData.Email },
                    RowId = Guid.NewGuid()
                };
                RabbitManager.PublishSingle(mail, "Mail.Send");
                Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> VerifyOtpCode(AppUser AppUser)
        {
            if (!await AppUserValidator.VerifyOptCode(AppUser))
                return AppUser;
            AppUser appUser = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Email = new StringFilter { Equal = AppUser.Email },
                Selects = AppUserSelect.ALL
            })).FirstOrDefault();
            appUser.Token = CreateToken(appUser.Id, appUser.Username, appUser.RowId, 300);
            return appUser;
        }

        public async Task<AppUser> RecoveryPassword(AppUser AppUser)
        {
            if (AppUser.Id == 0)
                return null;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                CurrentContext.UserId = AppUser.Id;
                oldData.Password = HashPassword(AppUser.Password);

                await UOW.AppUserRepository.Update(oldData);


                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                Mail mail = new Mail
                {
                    Subject = "Recovery Password",
                    Body = $"Your password has been recovered.",
                    Recipients = new List<string> { newData.Email },
                    RowId = Guid.NewGuid()
                };
                RabbitManager.PublishSingle(mail, "Mail.Send");
                Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        private string GenerateOTPCode()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }

    }
}
