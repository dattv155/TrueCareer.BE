using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using TrueCareer.Entities;
using TrueCareer.Repositories;
using TrueSight.Common;
using Google.Apis.Auth;

namespace TrueCareer.Services.MAppUser
{
    public partial class AppUserService
    {
        public async Task<AppUser> GoogleLogin(string idToken)
        {
            try
            {
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                if (validPayload != null)
                {
                    AppUserFilter AppUserFilter = new AppUserFilter
                    {
                        Skip = 0,
                        Take = 1,
                        Username = new StringFilter { Equal = validPayload.Email },
                        Selects = AppUserSelect.Id,
                    };
                    var AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                    var AppUser = AppUsers.FirstOrDefault();
                    if (AppUser == null)
                    {
                        AppUser = new AppUser
                        {
                            Username = validPayload.Email,
                            DisplayName = validPayload.Name,
                            Email = validPayload.Email,
                            Password = "",
                            Phone = "",
                            SexId = 3,
                        };
                        await UOW.AppUserRepository.Create(AppUser);
                    }
                    else
                    {
                        AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                    }

                    AppUser.DisplayName = validPayload.Name;
                    await UOW.AppUserRepository.Update(AppUser);
                    return AppUser;
                }
            }
            catch (Exception ex)
            {
                throw new MessageException(ex);
            }
            return null;
        }
        public async Task<AppUser> FacebookLogin(string idToken)
        {
            try
            {
                IRestClient RestClient = new RestClient("https://graph.facebook.com");
                IRestRequest RestRequest = new RestRequest("/me");
                RestRequest.AddParameter("access_token", idToken);


                var result = RestClient.Get<dynamic>(RestRequest);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string Username = result.Data["id"];
                    string DisplayName = result.Data["name"];
                    AppUserFilter AppUserFilter = new AppUserFilter
                    {
                        Skip = 0,
                        Take = 1,
                        Username = new StringFilter { Equal = Username },
                        // VerificationTypeId = new IdFilter { Equal = VerificationTypeEnum.FACEBOOK.Id },
                        Selects = AppUserSelect.Id,
                    };
                    var AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                    var AppUser = AppUsers.FirstOrDefault();
                    if (AppUser == null)
                    {
                        AppUser = new AppUser
                        {
                            Username = Username,
                            DisplayName = DisplayName,
                            Email = "",
                            // VerificationTypeId = VerificationTypeEnum.FACEBOOK.Id,
                            // StatusId = StatusEnum.ACTIVE.Id,
                            Password = "",
                            Phone = "",
                            SexId = 3,
                        };
                        await UOW.AppUserRepository.Create(AppUser);
                    }
                    else
                    {
                        AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                    }

                    AppUser.DisplayName = DisplayName;
                    await UOW.AppUserRepository.Update(AppUser);
                    return AppUser;
                }
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> AppleLogin(string idToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(idToken);
                var tokenS = jsonToken as JwtSecurityToken;

                IRestClient RestClient = new RestClient("https://appleid.apple.com");
                IRestRequest RestRequest = new RestRequest("/auth/keys");

                var result = RestClient.Get<AppleKeySets>(RestRequest);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<AppleKeySet> AppleKeySets = result.Data.keys;
                    AppleKeySet AppleKeySet = AppleKeySets
                        .Where(x => x.kid == tokenS.Header.Kid)
                        .FirstOrDefault();

                    var validationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKeyResolver = (token, secutiryToken, kid, validationParameters) =>
                        {
                            string PublicRSAKeyBase64 = Configuration["Config:PublicRSAKey"];
                            byte[] PublicRSAKeyBytes = Convert.FromBase64String(PublicRSAKeyBase64);
                            string PublicRSAKey = Encoding.Default.GetString(PublicRSAKeyBytes);

                            var rsa = RSA.Create();
                            rsa.ImportParameters(new RSAParameters
                            {
                                Exponent = Base64UrlEncoder.DecodeBytes(AppleKeySet.e),
                                Modulus = Base64UrlEncoder.DecodeBytes(AppleKeySet.n),
                            });

                            SecurityKey RSASecurityKey = new RsaSecurityKey(rsa);
                            return new List<SecurityKey> { RSASecurityKey };
                        }
                    };
                    try
                    {
                        SecurityToken validatedToken;
                        var principal = handler.ValidateToken(idToken, validationParameters, out validatedToken);
                        string Username = principal.Claims.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value).FirstOrDefault();
                        string DisplayName = Username;
                        AppUserFilter AppUserFilter = new AppUserFilter
                        {
                            Skip = 0,
                            Take = 1,
                            Username = new StringFilter { Equal = Username },
                            // VerificationTypeId = new IdFilter { Equal = VerificationTypeEnum.APPLE.Id },
                            Selects = AppUserSelect.Id,
                        };
                        var AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                        var AppUser = AppUsers.FirstOrDefault();
                        if (AppUser == null)
                        {
                            AppUser = new AppUser
                            {
                                Username = Username,
                                DisplayName = DisplayName,
                                Email = Username,
                                // VerificationTypeId = VerificationTypeEnum.APPLE.Id,
                                // StatusId = StatusEnum.ACTIVE.Id,
                                Password = "",
                                Phone = "",
                                SexId = 3,
                            };
                            await UOW.AppUserRepository.Create(AppUser);
                        }
                        else
                        {
                            AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                        }

                        AppUser.DisplayName = DisplayName;
                        await UOW.AppUserRepository.Update(AppUser);
                        return AppUser;
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("{0}\n {1}", e.Message, e.StackTrace);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new MessageException(ex);
            }
            return null;
        }
    }
    
    public class AppleKeySet
    {
        public string kty { get; set; }
        public string kid { get; set; }
        public string use { get; set; }
        public string alg { get; set; }
        public string n { get; set; }
        public string e { get; set; }
    }

    public class AppleKeySets
    {
        public List<AppleKeySet> keys { get; set; }
    }
    
}