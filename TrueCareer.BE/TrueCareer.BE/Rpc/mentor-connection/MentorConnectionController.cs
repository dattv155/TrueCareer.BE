using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using TrueCareer.Entities;
using TrueCareer.Services.MMentorConnection;
using TrueCareer.Services.MConnectionType;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mentor_connection
{
    public partial class MentorConnectionController : RpcController
    {
        private IConnectionTypeService ConnectionTypeService;
        private IAppUserService AppUserService;
        private IMentorConnectionService MentorConnectionService;
        private ICurrentContext CurrentContext;
        public MentorConnectionController(
            IConnectionTypeService ConnectionTypeService,
            IAppUserService AppUserService,
            IMentorConnectionService MentorConnectionService,
            ICurrentContext CurrentContext
        )
        {
            this.ConnectionTypeService = ConnectionTypeService;
            this.AppUserService = AppUserService;
            this.MentorConnectionService = MentorConnectionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MentorConnectionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] MentorConnection_MentorConnectionFilterDTO MentorConnection_MentorConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorConnectionFilter MentorConnectionFilter = ConvertFilterDTOToFilterEntity(MentorConnection_MentorConnectionFilterDTO);
            MentorConnectionFilter = await MentorConnectionService.ToFilter(MentorConnectionFilter);
            int count = await MentorConnectionService.Count(MentorConnectionFilter);
            return count;
        }

        [Route(MentorConnectionRoute.List), HttpPost]
        public async Task<ActionResult<List<MentorConnection_MentorConnectionDTO>>> List([FromBody] MentorConnection_MentorConnectionFilterDTO MentorConnection_MentorConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorConnectionFilter MentorConnectionFilter = ConvertFilterDTOToFilterEntity(MentorConnection_MentorConnectionFilterDTO);
            MentorConnectionFilter = await MentorConnectionService.ToFilter(MentorConnectionFilter);
            List<MentorConnection> MentorConnections = await MentorConnectionService.List(MentorConnectionFilter);
            List<MentorConnection_MentorConnectionDTO> MentorConnection_MentorConnectionDTOs = MentorConnections
                .Select(c => new MentorConnection_MentorConnectionDTO(c)).ToList();
            return MentorConnection_MentorConnectionDTOs;
        }

        [Route(MentorConnectionRoute.Get), HttpPost]
        public async Task<ActionResult<MentorConnection_MentorConnectionDTO>> Get([FromBody]MentorConnection_MentorConnectionDTO MentorConnection_MentorConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorConnection_MentorConnectionDTO.Id))
                return Forbid();

            MentorConnection MentorConnection = await MentorConnectionService.Get(MentorConnection_MentorConnectionDTO.Id);
            return new MentorConnection_MentorConnectionDTO(MentorConnection);
        }

        [Route(MentorConnectionRoute.Create), HttpPost]
        public async Task<ActionResult<MentorConnection_MentorConnectionDTO>> Create([FromBody] MentorConnection_MentorConnectionDTO MentorConnection_MentorConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(MentorConnection_MentorConnectionDTO.Id))
                return Forbid();

            MentorConnection MentorConnection = ConvertDTOToEntity(MentorConnection_MentorConnectionDTO);
            MentorConnection = await MentorConnectionService.Create(MentorConnection);
            MentorConnection_MentorConnectionDTO = new MentorConnection_MentorConnectionDTO(MentorConnection);
            if (MentorConnection.IsValidated)
                return MentorConnection_MentorConnectionDTO;
            else
                return BadRequest(MentorConnection_MentorConnectionDTO);
        }

        [Route(MentorConnectionRoute.Update), HttpPost]
        public async Task<ActionResult<MentorConnection_MentorConnectionDTO>> Update([FromBody] MentorConnection_MentorConnectionDTO MentorConnection_MentorConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(MentorConnection_MentorConnectionDTO.Id))
                return Forbid();

            MentorConnection MentorConnection = ConvertDTOToEntity(MentorConnection_MentorConnectionDTO);
            MentorConnection = await MentorConnectionService.Update(MentorConnection);
            MentorConnection_MentorConnectionDTO = new MentorConnection_MentorConnectionDTO(MentorConnection);
            if (MentorConnection.IsValidated)
                return MentorConnection_MentorConnectionDTO;
            else
                return BadRequest(MentorConnection_MentorConnectionDTO);
        }

        [Route(MentorConnectionRoute.Delete), HttpPost]
        public async Task<ActionResult<MentorConnection_MentorConnectionDTO>> Delete([FromBody] MentorConnection_MentorConnectionDTO MentorConnection_MentorConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorConnection_MentorConnectionDTO.Id))
                return Forbid();

            MentorConnection MentorConnection = ConvertDTOToEntity(MentorConnection_MentorConnectionDTO);
            MentorConnection = await MentorConnectionService.Delete(MentorConnection);
            MentorConnection_MentorConnectionDTO = new MentorConnection_MentorConnectionDTO(MentorConnection);
            if (MentorConnection.IsValidated)
                return MentorConnection_MentorConnectionDTO;
            else
                return BadRequest(MentorConnection_MentorConnectionDTO);
        }
        
        [Route(MentorConnectionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorConnectionFilter MentorConnectionFilter = new MentorConnectionFilter();
            MentorConnectionFilter = await MentorConnectionService.ToFilter(MentorConnectionFilter);
            MentorConnectionFilter.Id = new IdFilter { In = Ids };
            MentorConnectionFilter.Selects = MentorConnectionSelect.Id;
            MentorConnectionFilter.Skip = 0;
            MentorConnectionFilter.Take = int.MaxValue;

            List<MentorConnection> MentorConnections = await MentorConnectionService.List(MentorConnectionFilter);
            MentorConnections = await MentorConnectionService.BulkDelete(MentorConnections);
            if (MentorConnections.Any(x => !x.IsValidated))
                return BadRequest(MentorConnections.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(MentorConnectionRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ConnectionTypeFilter ConnectionTypeFilter = new ConnectionTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ConnectionTypeSelect.ALL
            };
            List<ConnectionType> ConnectionTypes = await ConnectionTypeService.List(ConnectionTypeFilter);
            AppUserFilter MentorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Mentors = await AppUserService.List(MentorFilter);
            List<MentorConnection> MentorConnections = new List<MentorConnection>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(MentorConnections);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int MentorIdColumn = 1 + StartColumn;
                int UrlColumn = 2 + StartColumn;
                int ConnectionTypeIdColumn = 3 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string MentorIdValue = worksheet.Cells[i, MentorIdColumn].Value?.ToString();
                    string UrlValue = worksheet.Cells[i, UrlColumn].Value?.ToString();
                    string ConnectionTypeIdValue = worksheet.Cells[i, ConnectionTypeIdColumn].Value?.ToString();
                    
                    MentorConnection MentorConnection = new MentorConnection();
                    MentorConnection.Url = UrlValue;
                    ConnectionType ConnectionType = ConnectionTypes.Where(x => x.Id.ToString() == ConnectionTypeIdValue).FirstOrDefault();
                    MentorConnection.ConnectionTypeId = ConnectionType == null ? 0 : ConnectionType.Id;
                    MentorConnection.ConnectionType = ConnectionType;
                    AppUser Mentor = Mentors.Where(x => x.Id.ToString() == MentorIdValue).FirstOrDefault();
                    MentorConnection.MentorId = Mentor == null ? 0 : Mentor.Id;
                    MentorConnection.Mentor = Mentor;
                    
                    MentorConnections.Add(MentorConnection);
                }
            }
            MentorConnections = await MentorConnectionService.Import(MentorConnections);
            if (MentorConnections.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < MentorConnections.Count; i++)
                {
                    MentorConnection MentorConnection = MentorConnections[i];
                    if (!MentorConnection.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (MentorConnection.Errors.ContainsKey(nameof(MentorConnection.Id)))
                            Error += MentorConnection.Errors[nameof(MentorConnection.Id)];
                        if (MentorConnection.Errors.ContainsKey(nameof(MentorConnection.MentorId)))
                            Error += MentorConnection.Errors[nameof(MentorConnection.MentorId)];
                        if (MentorConnection.Errors.ContainsKey(nameof(MentorConnection.Url)))
                            Error += MentorConnection.Errors[nameof(MentorConnection.Url)];
                        if (MentorConnection.Errors.ContainsKey(nameof(MentorConnection.ConnectionTypeId)))
                            Error += MentorConnection.Errors[nameof(MentorConnection.ConnectionTypeId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(MentorConnectionRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MentorConnection_MentorConnectionFilterDTO MentorConnection_MentorConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region MentorConnection
                var MentorConnectionFilter = ConvertFilterDTOToFilterEntity(MentorConnection_MentorConnectionFilterDTO);
                MentorConnectionFilter.Skip = 0;
                MentorConnectionFilter.Take = int.MaxValue;
                MentorConnectionFilter = await MentorConnectionService.ToFilter(MentorConnectionFilter);
                List<MentorConnection> MentorConnections = await MentorConnectionService.List(MentorConnectionFilter);

                var MentorConnectionHeaders = new List<string>()
                {
                    "Id",
                    "MentorId",
                    "Url",
                    "ConnectionTypeId",
                };
                List<object[]> MentorConnectionData = new List<object[]>();
                for (int i = 0; i < MentorConnections.Count; i++)
                {
                    var MentorConnection = MentorConnections[i];
                    MentorConnectionData.Add(new Object[]
                    {
                        MentorConnection.Id,
                        MentorConnection.MentorId,
                        MentorConnection.Url,
                        MentorConnection.ConnectionTypeId,
                    });
                }
                excel.GenerateWorksheet("MentorConnection", MentorConnectionHeaders, MentorConnectionData);
                #endregion
                
                #region ConnectionType
                var ConnectionTypeFilter = new ConnectionTypeFilter();
                ConnectionTypeFilter.Selects = ConnectionTypeSelect.ALL;
                ConnectionTypeFilter.OrderBy = ConnectionTypeOrder.Id;
                ConnectionTypeFilter.OrderType = OrderType.ASC;
                ConnectionTypeFilter.Skip = 0;
                ConnectionTypeFilter.Take = int.MaxValue;
                List<ConnectionType> ConnectionTypes = await ConnectionTypeService.List(ConnectionTypeFilter);

                var ConnectionTypeHeaders = new List<string>()
                {
                    "Id",
                    "Name",
                    "Code",
                };
                List<object[]> ConnectionTypeData = new List<object[]>();
                for (int i = 0; i < ConnectionTypes.Count; i++)
                {
                    var ConnectionType = ConnectionTypes[i];
                    ConnectionTypeData.Add(new Object[]
                    {
                        ConnectionType.Id,
                        ConnectionType.Name,
                        ConnectionType.Code,
                    });
                }
                excel.GenerateWorksheet("ConnectionType", ConnectionTypeHeaders, ConnectionTypeData);
                #endregion
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string>()
                {
                    "Id",
                    "Username",
                    "Email",
                    "Phone",
                    "Password",
                    "DisplayName",
                    "SexId",
                    "Birthday",
                    "Avatar",
                    "CoverImage",
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.Password,
                        AppUser.DisplayName,
                        AppUser.SexId,
                        AppUser.Birthday,
                        AppUser.Avatar,
                        AppUser.CoverImage,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "MentorConnection.xlsx");
        }

        [Route(MentorConnectionRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] MentorConnection_MentorConnectionFilterDTO MentorConnection_MentorConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/MentorConnection_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "MentorConnection.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            MentorConnectionFilter MentorConnectionFilter = new MentorConnectionFilter();
            MentorConnectionFilter = await MentorConnectionService.ToFilter(MentorConnectionFilter);
            if (Id == 0)
            {

            }
            else
            {
                MentorConnectionFilter.Id = new IdFilter { Equal = Id };
                int count = await MentorConnectionService.Count(MentorConnectionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private MentorConnection ConvertDTOToEntity(MentorConnection_MentorConnectionDTO MentorConnection_MentorConnectionDTO)
        {
            MentorConnection_MentorConnectionDTO.TrimString();
            MentorConnection MentorConnection = new MentorConnection();
            MentorConnection.Id = MentorConnection_MentorConnectionDTO.Id;
            MentorConnection.MentorId = MentorConnection_MentorConnectionDTO.MentorId;
            MentorConnection.Url = MentorConnection_MentorConnectionDTO.Url;
            MentorConnection.ConnectionTypeId = MentorConnection_MentorConnectionDTO.ConnectionTypeId;
            MentorConnection.ConnectionType = MentorConnection_MentorConnectionDTO.ConnectionType == null ? null : new ConnectionType
            {
                Id = MentorConnection_MentorConnectionDTO.ConnectionType.Id,
                Name = MentorConnection_MentorConnectionDTO.ConnectionType.Name,
                Code = MentorConnection_MentorConnectionDTO.ConnectionType.Code,
            };
            MentorConnection.Mentor = MentorConnection_MentorConnectionDTO.Mentor == null ? null : new AppUser
            {
                Id = MentorConnection_MentorConnectionDTO.Mentor.Id,
                Username = MentorConnection_MentorConnectionDTO.Mentor.Username,
                Email = MentorConnection_MentorConnectionDTO.Mentor.Email,
                Phone = MentorConnection_MentorConnectionDTO.Mentor.Phone,
                Password = MentorConnection_MentorConnectionDTO.Mentor.Password,
                DisplayName = MentorConnection_MentorConnectionDTO.Mentor.DisplayName,
                SexId = MentorConnection_MentorConnectionDTO.Mentor.SexId,
                Birthday = MentorConnection_MentorConnectionDTO.Mentor.Birthday,
                Avatar = MentorConnection_MentorConnectionDTO.Mentor.Avatar,
                CoverImage = MentorConnection_MentorConnectionDTO.Mentor.CoverImage,
            };
            MentorConnection.BaseLanguage = CurrentContext.Language;
            return MentorConnection;
        }

        private MentorConnectionFilter ConvertFilterDTOToFilterEntity(MentorConnection_MentorConnectionFilterDTO MentorConnection_MentorConnectionFilterDTO)
        {
            MentorConnectionFilter MentorConnectionFilter = new MentorConnectionFilter();
            MentorConnectionFilter.Selects = MentorConnectionSelect.ALL;
            MentorConnectionFilter.Skip = MentorConnection_MentorConnectionFilterDTO.Skip;
            MentorConnectionFilter.Take = MentorConnection_MentorConnectionFilterDTO.Take;
            MentorConnectionFilter.OrderBy = MentorConnection_MentorConnectionFilterDTO.OrderBy;
            MentorConnectionFilter.OrderType = MentorConnection_MentorConnectionFilterDTO.OrderType;

            MentorConnectionFilter.Id = MentorConnection_MentorConnectionFilterDTO.Id;
            MentorConnectionFilter.MentorId = MentorConnection_MentorConnectionFilterDTO.MentorId;
            MentorConnectionFilter.Url = MentorConnection_MentorConnectionFilterDTO.Url;
            MentorConnectionFilter.ConnectionTypeId = MentorConnection_MentorConnectionFilterDTO.ConnectionTypeId;
            return MentorConnectionFilter;
        }
    }
}

