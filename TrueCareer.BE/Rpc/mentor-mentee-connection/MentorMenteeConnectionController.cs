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
using TrueCareer.Services.MMentorMenteeConnection;
using TrueCareer.Services.MMentorConnection;
using TrueCareer.Services.MConnectionStatus;
using TrueCareer.Services.MMentorReview;
using TrueCareer.Services.MAppUser;
using TrueCareer.Enums;

namespace TrueCareer.Rpc.mentor_mentee_connection
{
    public partial class MentorMenteeConnectionController : RpcController
    {
        private IMentorConnectionService MentorConnectionService;
        private IConnectionStatusService ConnectionStatusService;
        private IAppUserService AppUserService;
        private IMentorMenteeConnectionService MentorMenteeConnectionService;
        private IMentorReviewService MentorReviewService;
        private ICurrentContext CurrentContext;
        public MentorMenteeConnectionController(
            IMentorConnectionService MentorConnectionService,
            IConnectionStatusService ConnectionStatusService,
            IAppUserService AppUserService,
            IMentorMenteeConnectionService MentorMenteeConnectionService,
            IMentorReviewService MentorReviewService,
            ICurrentContext CurrentContext
        )
        {
            this.MentorConnectionService = MentorConnectionService;
            this.ConnectionStatusService = ConnectionStatusService;
            this.AppUserService = AppUserService;
            this.MentorMenteeConnectionService = MentorMenteeConnectionService;
            this.MentorReviewService = MentorReviewService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MentorMenteeConnectionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] MentorMenteeConnection_MentorMenteeConnectionFilterDTO MentorMenteeConnection_MentorMenteeConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorMenteeConnectionFilter MentorMenteeConnectionFilter = ConvertFilterDTOToFilterEntity(MentorMenteeConnection_MentorMenteeConnectionFilterDTO);
            MentorMenteeConnectionFilter = await MentorMenteeConnectionService.ToFilter(MentorMenteeConnectionFilter);
            int count = await MentorMenteeConnectionService.Count(MentorMenteeConnectionFilter);
            return count;
        }

        [Route(MentorMenteeConnectionRoute.List), HttpPost]
        public async Task<ActionResult<List<MentorMenteeConnection_MentorMenteeConnectionDTO>>> List([FromBody] MentorMenteeConnection_MentorMenteeConnectionFilterDTO MentorMenteeConnection_MentorMenteeConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorMenteeConnectionFilter MentorMenteeConnectionFilter = ConvertFilterDTOToFilterEntity(MentorMenteeConnection_MentorMenteeConnectionFilterDTO);
            MentorMenteeConnectionFilter = await MentorMenteeConnectionService.ToFilter(MentorMenteeConnectionFilter);
            List<MentorMenteeConnection> MentorMenteeConnections = await MentorMenteeConnectionService.List(MentorMenteeConnectionFilter);
            List<MentorMenteeConnection_MentorMenteeConnectionDTO> MentorMenteeConnection_MentorMenteeConnectionDTOs = MentorMenteeConnections
                .Select(c => new MentorMenteeConnection_MentorMenteeConnectionDTO(c)).ToList();
            return MentorMenteeConnection_MentorMenteeConnectionDTOs;
        }

        [Route(MentorMenteeConnectionRoute.Get), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorMenteeConnectionDTO>> Get([FromBody] MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorMenteeConnection_MentorMenteeConnectionDTO.Id))
                return Forbid();

            MentorMenteeConnection MentorMenteeConnection = await MentorMenteeConnectionService.Get(MentorMenteeConnection_MentorMenteeConnectionDTO.Id);
            return new MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection);
        }

        [Route(MentorMenteeConnectionRoute.Create), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorMenteeConnectionDTO>> Create([FromBody] MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorMenteeConnection_MentorMenteeConnectionDTO.Id))
                return Forbid();

            MentorMenteeConnection MentorMenteeConnection = ConvertDTOToEntity(MentorMenteeConnection_MentorMenteeConnectionDTO);
            MentorMenteeConnection.ConnectionStatusId = ConnectionStatusEnum.PENDING.Id;
            MentorMenteeConnection.ConnectionStatus = new ConnectionStatus()
            {
                Id = ConnectionStatusEnum.PENDING.Id,
                Code = ConnectionStatusEnum.PENDING.Code,
                Name = ConnectionStatusEnum.PENDING.Name
            };
            MentorMenteeConnection = await MentorMenteeConnectionService.Create(MentorMenteeConnection);
            MentorMenteeConnection_MentorMenteeConnectionDTO = new MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection);
            if (MentorMenteeConnection.IsValidated)
                return MentorMenteeConnection_MentorMenteeConnectionDTO;
            else
                return BadRequest(MentorMenteeConnection_MentorMenteeConnectionDTO);
        }

        [Route(MentorMenteeConnectionRoute.Approve), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorMenteeConnectionDTO>> Approve([FromBody] MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorMenteeConnection_MentorMenteeConnectionDTO.Id))
                return Forbid();

            MentorMenteeConnection MentorMenteeConnection = ConvertDTOToEntity(MentorMenteeConnection_MentorMenteeConnectionDTO);
            MentorMenteeConnection.ConnectionStatusId = ConnectionStatusEnum.COMING_SOON.Id;
            MentorMenteeConnection.ConnectionStatus = new ConnectionStatus()
            {
                Id = ConnectionStatusEnum.COMING_SOON.Id,
                Code = ConnectionStatusEnum.COMING_SOON.Code,
                Name = ConnectionStatusEnum.COMING_SOON.Name
            };
            MentorMenteeConnection = await MentorMenteeConnectionService.Update(MentorMenteeConnection);
            MentorMenteeConnection_MentorMenteeConnectionDTO = new MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection);
            if (MentorMenteeConnection.IsValidated)
                return MentorMenteeConnection_MentorMenteeConnectionDTO;
            else
                return BadRequest(MentorMenteeConnection_MentorMenteeConnectionDTO);
        }

        [Route(MentorMenteeConnectionRoute.Reject), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorMenteeConnectionDTO>> Reject([FromBody] MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorMenteeConnection_MentorMenteeConnectionDTO.Id))
                return Forbid();

            MentorMenteeConnection MentorMenteeConnection = ConvertDTOToEntity(MentorMenteeConnection_MentorMenteeConnectionDTO);
            MentorMenteeConnection.ConnectionStatusId = ConnectionStatusEnum.REJECTED.Id;
            MentorMenteeConnection.ConnectionStatus = new ConnectionStatus()
            {
                Id = ConnectionStatusEnum.REJECTED.Id,
                Code = ConnectionStatusEnum.REJECTED.Code,
                Name = ConnectionStatusEnum.REJECTED.Name
            };
            MentorMenteeConnection = await MentorMenteeConnectionService.Update(MentorMenteeConnection);
            MentorMenteeConnection_MentorMenteeConnectionDTO = new MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection);
            if (MentorMenteeConnection.IsValidated)
                return MentorMenteeConnection_MentorMenteeConnectionDTO;
            else
                return BadRequest(MentorMenteeConnection_MentorMenteeConnectionDTO);
        }

        [Route(MentorMenteeConnectionRoute.Cancel), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorMenteeConnectionDTO>> Cancel([FromBody] MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorMenteeConnection_MentorMenteeConnectionDTO.Id))
                return Forbid();

            MentorMenteeConnection MentorMenteeConnection = ConvertDTOToEntity(MentorMenteeConnection_MentorMenteeConnectionDTO);
            MentorMenteeConnection.ConnectionStatusId = ConnectionStatusEnum.CANCEL.Id;
            MentorMenteeConnection.ConnectionStatus = new ConnectionStatus()
            {
                Id = ConnectionStatusEnum.CANCEL.Id,
                Code = ConnectionStatusEnum.CANCEL.Code,
                Name = ConnectionStatusEnum.CANCEL.Name
            };
            MentorMenteeConnection = await MentorMenteeConnectionService.Update(MentorMenteeConnection);
            MentorMenteeConnection_MentorMenteeConnectionDTO = new MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection);
            if (MentorMenteeConnection.IsValidated)
                return MentorMenteeConnection_MentorMenteeConnectionDTO;
            else
                return BadRequest(MentorMenteeConnection_MentorMenteeConnectionDTO);
        }
        [Route(MentorMenteeConnectionRoute.Review), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorReviewDTO>> Review([FromBody] MentorMenteeConnection_MentorReviewDTO MentorMenteeConnection_MentorReviewDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MentorReview MentorReview = new MentorReview()
            {
                Id = MentorMenteeConnection_MentorReviewDTO.Id,
                Description = MentorMenteeConnection_MentorReviewDTO.Description,
                ContentReview = MentorMenteeConnection_MentorReviewDTO.ContentReview,
                Star = MentorMenteeConnection_MentorReviewDTO.Star,
                CreatorId = MentorMenteeConnection_MentorReviewDTO.CreatorId,
                MentorId = MentorMenteeConnection_MentorReviewDTO.MentorId,
                Creator = MentorMenteeConnection_MentorReviewDTO.Creator == null ? null : new AppUser
                {
                    Id = MentorMenteeConnection_MentorReviewDTO.Creator.Id,
                    Username = MentorMenteeConnection_MentorReviewDTO.Creator.Username,
                    Email = MentorMenteeConnection_MentorReviewDTO.Creator.Email,
                    Phone = MentorMenteeConnection_MentorReviewDTO.Creator.Phone,
                    Password = MentorMenteeConnection_MentorReviewDTO.Creator.Password,
                    DisplayName = MentorMenteeConnection_MentorReviewDTO.Creator.DisplayName,
                    SexId = MentorMenteeConnection_MentorReviewDTO.Creator.SexId,
                    Birthday = MentorMenteeConnection_MentorReviewDTO.Creator.Birthday,
                    Avatar = MentorMenteeConnection_MentorReviewDTO.Creator.Avatar,
                    CoverImage = MentorMenteeConnection_MentorReviewDTO.Creator.CoverImage,
                },
                Mentor = MentorMenteeConnection_MentorReviewDTO.Mentor == null ? null : new AppUser
                {
                    Id = MentorMenteeConnection_MentorReviewDTO.Mentor.Id,
                    Username = MentorMenteeConnection_MentorReviewDTO.Mentor.Username,
                    Email = MentorMenteeConnection_MentorReviewDTO.Mentor.Email,
                    Phone = MentorMenteeConnection_MentorReviewDTO.Mentor.Phone,
                    Password = MentorMenteeConnection_MentorReviewDTO.Mentor.Password,
                    DisplayName = MentorMenteeConnection_MentorReviewDTO.Mentor.DisplayName,
                    SexId = MentorMenteeConnection_MentorReviewDTO.Mentor.SexId,
                    Birthday = MentorMenteeConnection_MentorReviewDTO.Mentor.Birthday,
                    Avatar = MentorMenteeConnection_MentorReviewDTO.Mentor.Avatar,
                    CoverImage = MentorMenteeConnection_MentorReviewDTO.Mentor.CoverImage,
                },
                Time = MentorMenteeConnection_MentorReviewDTO.Time
            };

            MentorReview = await MentorReviewService.Create(MentorReview);
            MentorMenteeConnection_MentorReviewDTO = new MentorMenteeConnection_MentorReviewDTO(MentorReview);
            if (MentorReview.IsValidated)
                return MentorMenteeConnection_MentorReviewDTO;
            else
                return BadRequest(MentorMenteeConnection_MentorReviewDTO);
        }

        [Route(MentorMenteeConnectionRoute.Update), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorMenteeConnectionDTO>> Update([FromBody] MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorMenteeConnection_MentorMenteeConnectionDTO.Id))
                return Forbid();

            MentorMenteeConnection MentorMenteeConnection = ConvertDTOToEntity(MentorMenteeConnection_MentorMenteeConnectionDTO);
            MentorMenteeConnection = await MentorMenteeConnectionService.Update(MentorMenteeConnection);
            MentorMenteeConnection_MentorMenteeConnectionDTO = new MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection);
            if (MentorMenteeConnection.IsValidated)
                return MentorMenteeConnection_MentorMenteeConnectionDTO;
            else
                return BadRequest(MentorMenteeConnection_MentorMenteeConnectionDTO);
        }

        [Route(MentorMenteeConnectionRoute.Delete), HttpPost]
        public async Task<ActionResult<MentorMenteeConnection_MentorMenteeConnectionDTO>> Delete([FromBody] MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MentorMenteeConnection_MentorMenteeConnectionDTO.Id))
                return Forbid();

            MentorMenteeConnection MentorMenteeConnection = ConvertDTOToEntity(MentorMenteeConnection_MentorMenteeConnectionDTO);
            MentorMenteeConnection = await MentorMenteeConnectionService.Delete(MentorMenteeConnection);
            MentorMenteeConnection_MentorMenteeConnectionDTO = new MentorMenteeConnection_MentorMenteeConnectionDTO(MentorMenteeConnection);
            if (MentorMenteeConnection.IsValidated)
                return MentorMenteeConnection_MentorMenteeConnectionDTO;
            else
                return BadRequest(MentorMenteeConnection_MentorMenteeConnectionDTO);
        }

        [Route(MentorMenteeConnectionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MentorMenteeConnectionFilter MentorMenteeConnectionFilter = new MentorMenteeConnectionFilter();
            MentorMenteeConnectionFilter = await MentorMenteeConnectionService.ToFilter(MentorMenteeConnectionFilter);
            MentorMenteeConnectionFilter.Id = new IdFilter { In = Ids };
            MentorMenteeConnectionFilter.Selects = MentorMenteeConnectionSelect.Id;
            MentorMenteeConnectionFilter.Skip = 0;
            MentorMenteeConnectionFilter.Take = int.MaxValue;

            List<MentorMenteeConnection> MentorMenteeConnections = await MentorMenteeConnectionService.List(MentorMenteeConnectionFilter);
            MentorMenteeConnections = await MentorMenteeConnectionService.BulkDelete(MentorMenteeConnections);
            if (MentorMenteeConnections.Any(x => !x.IsValidated))
                return BadRequest(MentorMenteeConnections.Where(x => !x.IsValidated));
            return true;
        }

        [Route(MentorMenteeConnectionRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MentorConnectionFilter ConnectionFilter = new MentorConnectionFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = MentorConnectionSelect.ALL
            };
            List<MentorConnection> Connections = await MentorConnectionService.List(ConnectionFilter);
            ConnectionStatusFilter ConnectionStatusFilter = new ConnectionStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ConnectionStatusSelect.ALL
            };
            List<ConnectionStatus> ConnectionStatuses = await ConnectionStatusService.List(ConnectionStatusFilter);
            AppUserFilter MenteeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Mentees = await AppUserService.List(MenteeFilter);
            AppUserFilter MentorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Mentors = await AppUserService.List(MentorFilter);
            List<MentorMenteeConnection> MentorMenteeConnections = new List<MentorMenteeConnection>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(MentorMenteeConnections);
                int StartColumn = 1;
                int StartRow = 1;
                int MentorIdColumn = 0 + StartColumn;
                int MenteeIdColumn = 1 + StartColumn;
                int ConnectionIdColumn = 2 + StartColumn;
                int FirstMessageColumn = 3 + StartColumn;
                int ConnectionStatusIdColumn = 4 + StartColumn;
                int ActiveTimeIdColumn = 5 + StartColumn;
                int IdColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string MentorIdValue = worksheet.Cells[i, MentorIdColumn].Value?.ToString();
                    string MenteeIdValue = worksheet.Cells[i, MenteeIdColumn].Value?.ToString();
                    string ConnectionIdValue = worksheet.Cells[i, ConnectionIdColumn].Value?.ToString();
                    string FirstMessageValue = worksheet.Cells[i, FirstMessageColumn].Value?.ToString();
                    string ConnectionStatusIdValue = worksheet.Cells[i, ConnectionStatusIdColumn].Value?.ToString();
                    string ActiveTimeIdValue = worksheet.Cells[i, ActiveTimeIdColumn].Value?.ToString();
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();

                    MentorMenteeConnection MentorMenteeConnection = new MentorMenteeConnection();
                    MentorMenteeConnection.FirstMessage = FirstMessageValue;
                    MentorConnection Connection = Connections.Where(x => x.Id.ToString() == ConnectionIdValue).FirstOrDefault();
                    MentorMenteeConnection.ConnectionId = Connection == null ? 0 : Connection.Id;
                    MentorMenteeConnection.Connection = Connection;
                    ConnectionStatus ConnectionStatus = ConnectionStatuses.Where(x => x.Id.ToString() == ConnectionStatusIdValue).FirstOrDefault();
                    MentorMenteeConnection.ConnectionStatusId = ConnectionStatus == null ? 0 : ConnectionStatus.Id;
                    MentorMenteeConnection.ConnectionStatus = ConnectionStatus;
                    AppUser Mentee = Mentees.Where(x => x.Id.ToString() == MenteeIdValue).FirstOrDefault();
                    MentorMenteeConnection.MenteeId = Mentee == null ? 0 : Mentee.Id;
                    MentorMenteeConnection.Mentee = Mentee;
                    AppUser Mentor = Mentors.Where(x => x.Id.ToString() == MentorIdValue).FirstOrDefault();
                    MentorMenteeConnection.MentorId = Mentor == null ? 0 : Mentor.Id;
                    MentorMenteeConnection.Mentor = Mentor;

                    MentorMenteeConnections.Add(MentorMenteeConnection);
                }
            }
            MentorMenteeConnections = await MentorMenteeConnectionService.Import(MentorMenteeConnections);
            if (MentorMenteeConnections.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < MentorMenteeConnections.Count; i++)
                {
                    MentorMenteeConnection MentorMenteeConnection = MentorMenteeConnections[i];
                    if (!MentorMenteeConnection.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (MentorMenteeConnection.Errors.ContainsKey(nameof(MentorMenteeConnection.MentorId)))
                            Error += MentorMenteeConnection.Errors[nameof(MentorMenteeConnection.MentorId)];
                        if (MentorMenteeConnection.Errors.ContainsKey(nameof(MentorMenteeConnection.MenteeId)))
                            Error += MentorMenteeConnection.Errors[nameof(MentorMenteeConnection.MenteeId)];
                        if (MentorMenteeConnection.Errors.ContainsKey(nameof(MentorMenteeConnection.ConnectionId)))
                            Error += MentorMenteeConnection.Errors[nameof(MentorMenteeConnection.ConnectionId)];
                        if (MentorMenteeConnection.Errors.ContainsKey(nameof(MentorMenteeConnection.FirstMessage)))
                            Error += MentorMenteeConnection.Errors[nameof(MentorMenteeConnection.FirstMessage)];
                        if (MentorMenteeConnection.Errors.ContainsKey(nameof(MentorMenteeConnection.ConnectionStatusId)))
                            Error += MentorMenteeConnection.Errors[nameof(MentorMenteeConnection.ConnectionStatusId)];
                        if (MentorMenteeConnection.Errors.ContainsKey(nameof(MentorMenteeConnection.ActiveTimeId)))
                            Error += MentorMenteeConnection.Errors[nameof(MentorMenteeConnection.ActiveTimeId)];
                        if (MentorMenteeConnection.Errors.ContainsKey(nameof(MentorMenteeConnection.Id)))
                            Error += MentorMenteeConnection.Errors[nameof(MentorMenteeConnection.Id)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(MentorMenteeConnectionRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MentorMenteeConnection_MentorMenteeConnectionFilterDTO MentorMenteeConnection_MentorMenteeConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region MentorMenteeConnection
                var MentorMenteeConnectionFilter = ConvertFilterDTOToFilterEntity(MentorMenteeConnection_MentorMenteeConnectionFilterDTO);
                MentorMenteeConnectionFilter.Skip = 0;
                MentorMenteeConnectionFilter.Take = int.MaxValue;
                MentorMenteeConnectionFilter = await MentorMenteeConnectionService.ToFilter(MentorMenteeConnectionFilter);
                List<MentorMenteeConnection> MentorMenteeConnections = await MentorMenteeConnectionService.List(MentorMenteeConnectionFilter);

                var MentorMenteeConnectionHeaders = new List<string>()
                {
                    "MentorId",
                    "MenteeId",
                    "ConnectionId",
                    "FirstMessage",
                    "ConnectionStatusId",
                    "ActiveTimeId",
                    "Id",
                };
                List<object[]> MentorMenteeConnectionData = new List<object[]>();
                for (int i = 0; i < MentorMenteeConnections.Count; i++)
                {
                    var MentorMenteeConnection = MentorMenteeConnections[i];
                    MentorMenteeConnectionData.Add(new Object[]
                    {
                        MentorMenteeConnection.MentorId,
                        MentorMenteeConnection.MenteeId,
                        MentorMenteeConnection.ConnectionId,
                        MentorMenteeConnection.FirstMessage,
                        MentorMenteeConnection.ConnectionStatusId,
                        MentorMenteeConnection.ActiveTimeId,
                        MentorMenteeConnection.Id,
                    });
                }
                excel.GenerateWorksheet("MentorMenteeConnection", MentorMenteeConnectionHeaders, MentorMenteeConnectionData);
                #endregion

                #region MentorConnection
                var MentorConnectionFilter = new MentorConnectionFilter();
                MentorConnectionFilter.Selects = MentorConnectionSelect.ALL;
                MentorConnectionFilter.OrderBy = MentorConnectionOrder.Id;
                MentorConnectionFilter.OrderType = OrderType.ASC;
                MentorConnectionFilter.Skip = 0;
                MentorConnectionFilter.Take = int.MaxValue;
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
                #region ConnectionStatus
                var ConnectionStatusFilter = new ConnectionStatusFilter();
                ConnectionStatusFilter.Selects = ConnectionStatusSelect.ALL;
                ConnectionStatusFilter.OrderBy = ConnectionStatusOrder.Id;
                ConnectionStatusFilter.OrderType = OrderType.ASC;
                ConnectionStatusFilter.Skip = 0;
                ConnectionStatusFilter.Take = int.MaxValue;
                List<ConnectionStatus> ConnectionStatuses = await ConnectionStatusService.List(ConnectionStatusFilter);

                var ConnectionStatusHeaders = new List<string>()
                {
                    "Id",
                    "Code",
                    "Name",
                };
                List<object[]> ConnectionStatusData = new List<object[]>();
                for (int i = 0; i < ConnectionStatuses.Count; i++)
                {
                    var ConnectionStatus = ConnectionStatuses[i];
                    ConnectionStatusData.Add(new Object[]
                    {
                        ConnectionStatus.Id,
                        ConnectionStatus.Code,
                        ConnectionStatus.Name,
                    });
                }
                excel.GenerateWorksheet("ConnectionStatus", ConnectionStatusHeaders, ConnectionStatusData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "MentorMenteeConnection.xlsx");
        }

        [Route(MentorMenteeConnectionRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] MentorMenteeConnection_MentorMenteeConnectionFilterDTO MentorMenteeConnection_MentorMenteeConnectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            string path = "Templates/MentorMenteeConnection_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "MentorMenteeConnection.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            MentorMenteeConnectionFilter MentorMenteeConnectionFilter = new MentorMenteeConnectionFilter();
            MentorMenteeConnectionFilter = await MentorMenteeConnectionService.ToFilter(MentorMenteeConnectionFilter);
            if (Id == 0)
            {

            }
            else
            {
                MentorMenteeConnectionFilter.Id = new IdFilter { Equal = Id };
                int count = await MentorMenteeConnectionService.Count(MentorMenteeConnectionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private MentorMenteeConnection ConvertDTOToEntity(MentorMenteeConnection_MentorMenteeConnectionDTO MentorMenteeConnection_MentorMenteeConnectionDTO)
        {
            MentorMenteeConnection_MentorMenteeConnectionDTO.TrimString();
            MentorMenteeConnection MentorMenteeConnection = new MentorMenteeConnection();
            MentorMenteeConnection.MentorId = MentorMenteeConnection_MentorMenteeConnectionDTO.MentorId;
            MentorMenteeConnection.MenteeId = MentorMenteeConnection_MentorMenteeConnectionDTO.MenteeId;
            MentorMenteeConnection.ConnectionId = MentorMenteeConnection_MentorMenteeConnectionDTO.ConnectionId;
            MentorMenteeConnection.FirstMessage = MentorMenteeConnection_MentorMenteeConnectionDTO.FirstMessage;
            MentorMenteeConnection.ConnectionStatusId = MentorMenteeConnection_MentorMenteeConnectionDTO.ConnectionStatusId;
            MentorMenteeConnection.ActiveTimeId = MentorMenteeConnection_MentorMenteeConnectionDTO.ActiveTimeId;
            MentorMenteeConnection.Id = MentorMenteeConnection_MentorMenteeConnectionDTO.Id;
            MentorMenteeConnection.Connection = MentorMenteeConnection_MentorMenteeConnectionDTO.Connection == null ? null : new MentorConnection
            {
                Id = MentorMenteeConnection_MentorMenteeConnectionDTO.Connection.Id,
                MentorId = MentorMenteeConnection_MentorMenteeConnectionDTO.Connection.MentorId,
                Url = MentorMenteeConnection_MentorMenteeConnectionDTO.Connection.Url,
                ConnectionTypeId = MentorMenteeConnection_MentorMenteeConnectionDTO.Connection.ConnectionTypeId,
            };
            MentorMenteeConnection.ConnectionStatus = MentorMenteeConnection_MentorMenteeConnectionDTO.ConnectionStatus == null ? null : new ConnectionStatus
            {
                Id = MentorMenteeConnection_MentorMenteeConnectionDTO.ConnectionStatus.Id,
                Code = MentorMenteeConnection_MentorMenteeConnectionDTO.ConnectionStatus.Code,
                Name = MentorMenteeConnection_MentorMenteeConnectionDTO.ConnectionStatus.Name,
            };
            MentorMenteeConnection.Mentee = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee == null ? null : new AppUser
            {
                Id = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.Id,
                Username = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.Username,
                Email = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.Email,
                Phone = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.Phone,
                Password = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.Password,
                DisplayName = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.DisplayName,
                SexId = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.SexId,
                Birthday = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.Birthday,
                Avatar = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.Avatar,
                CoverImage = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentee.CoverImage,
            };
            MentorMenteeConnection.Mentor = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor == null ? null : new AppUser
            {
                Id = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.Id,
                Username = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.Username,
                Email = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.Email,
                Phone = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.Phone,
                Password = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.Password,
                DisplayName = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.DisplayName,
                SexId = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.SexId,
                Birthday = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.Birthday,
                Avatar = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.Avatar,
                CoverImage = MentorMenteeConnection_MentorMenteeConnectionDTO.Mentor.CoverImage,
            };
            MentorMenteeConnection.BaseLanguage = CurrentContext.Language;
            return MentorMenteeConnection;
        }

        private MentorMenteeConnectionFilter ConvertFilterDTOToFilterEntity(MentorMenteeConnection_MentorMenteeConnectionFilterDTO MentorMenteeConnection_MentorMenteeConnectionFilterDTO)
        {
            MentorMenteeConnectionFilter MentorMenteeConnectionFilter = new MentorMenteeConnectionFilter();
            MentorMenteeConnectionFilter.Selects = MentorMenteeConnectionSelect.ALL;
            MentorMenteeConnectionFilter.Skip = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.Skip;
            MentorMenteeConnectionFilter.Take = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.Take;
            MentorMenteeConnectionFilter.OrderBy = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.OrderBy;
            MentorMenteeConnectionFilter.OrderType = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.OrderType;

            MentorMenteeConnectionFilter.MentorId = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.MentorId;
            MentorMenteeConnectionFilter.MenteeId = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.MenteeId;
            MentorMenteeConnectionFilter.ConnectionId = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.ConnectionId;
            MentorMenteeConnectionFilter.FirstMessage = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.FirstMessage;
            MentorMenteeConnectionFilter.ConnectionStatusId = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.ConnectionStatusId;
            MentorMenteeConnectionFilter.ActiveTimeId = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.ActiveTimeId;
            MentorMenteeConnectionFilter.Id = MentorMenteeConnection_MentorMenteeConnectionFilterDTO.Id;
            return MentorMenteeConnectionFilter;
        }
    }
}

