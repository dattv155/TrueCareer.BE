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
using TrueCareer.Services.MActiveTime;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.active_time
{
    public partial class ActiveTimeController : RpcController
    {
        private IAppUserService AppUserService;
        private IActiveTimeService ActiveTimeService;
        private ICurrentContext CurrentContext;
        public ActiveTimeController(
            IAppUserService AppUserService,
            IActiveTimeService ActiveTimeService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ActiveTimeService = ActiveTimeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ActiveTimeRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ActiveTime_ActiveTimeFilterDTO ActiveTime_ActiveTimeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ActiveTimeFilter ActiveTimeFilter = ConvertFilterDTOToFilterEntity(ActiveTime_ActiveTimeFilterDTO);
            ActiveTimeFilter = await ActiveTimeService.ToFilter(ActiveTimeFilter);
            int count = await ActiveTimeService.Count(ActiveTimeFilter);
            return count;
        }

        [Route(ActiveTimeRoute.List), HttpPost]
        public async Task<ActionResult<List<ActiveTime_ActiveTimeDTO>>> List([FromBody] ActiveTime_ActiveTimeFilterDTO ActiveTime_ActiveTimeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ActiveTimeFilter ActiveTimeFilter = ConvertFilterDTOToFilterEntity(ActiveTime_ActiveTimeFilterDTO);
            ActiveTimeFilter = await ActiveTimeService.ToFilter(ActiveTimeFilter);
            List<ActiveTime> ActiveTimes = await ActiveTimeService.List(ActiveTimeFilter);
            List<ActiveTime_ActiveTimeDTO> ActiveTime_ActiveTimeDTOs = ActiveTimes
                .Select(c => new ActiveTime_ActiveTimeDTO(c)).ToList();
            return ActiveTime_ActiveTimeDTOs;
        }

        [Route(ActiveTimeRoute.Get), HttpPost]
        public async Task<ActionResult<ActiveTime_ActiveTimeDTO>> Get([FromBody]ActiveTime_ActiveTimeDTO ActiveTime_ActiveTimeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ActiveTime_ActiveTimeDTO.Id))
                return Forbid();

            ActiveTime ActiveTime = await ActiveTimeService.Get(ActiveTime_ActiveTimeDTO.Id);
            return new ActiveTime_ActiveTimeDTO(ActiveTime);
        }

        [Route(ActiveTimeRoute.Create), HttpPost]
        public async Task<ActionResult<ActiveTime_ActiveTimeDTO>> Create([FromBody] ActiveTime_ActiveTimeDTO ActiveTime_ActiveTimeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ActiveTime_ActiveTimeDTO.Id))
                return Forbid();

            ActiveTime ActiveTime = ConvertDTOToEntity(ActiveTime_ActiveTimeDTO);
            ActiveTime = await ActiveTimeService.Create(ActiveTime);
            ActiveTime_ActiveTimeDTO = new ActiveTime_ActiveTimeDTO(ActiveTime);
            if (ActiveTime.IsValidated)
                return ActiveTime_ActiveTimeDTO;
            else
                return BadRequest(ActiveTime_ActiveTimeDTO);
        }

        [Route(ActiveTimeRoute.Update), HttpPost]
        public async Task<ActionResult<ActiveTime_ActiveTimeDTO>> Update([FromBody] ActiveTime_ActiveTimeDTO ActiveTime_ActiveTimeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ActiveTime_ActiveTimeDTO.Id))
                return Forbid();

            ActiveTime ActiveTime = ConvertDTOToEntity(ActiveTime_ActiveTimeDTO);
            ActiveTime = await ActiveTimeService.Update(ActiveTime);
            ActiveTime_ActiveTimeDTO = new ActiveTime_ActiveTimeDTO(ActiveTime);
            if (ActiveTime.IsValidated)
                return ActiveTime_ActiveTimeDTO;
            else
                return BadRequest(ActiveTime_ActiveTimeDTO);
        }

        [Route(ActiveTimeRoute.Delete), HttpPost]
        public async Task<ActionResult<ActiveTime_ActiveTimeDTO>> Delete([FromBody] ActiveTime_ActiveTimeDTO ActiveTime_ActiveTimeDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ActiveTime_ActiveTimeDTO.Id))
                return Forbid();

            ActiveTime ActiveTime = ConvertDTOToEntity(ActiveTime_ActiveTimeDTO);
            ActiveTime = await ActiveTimeService.Delete(ActiveTime);
            ActiveTime_ActiveTimeDTO = new ActiveTime_ActiveTimeDTO(ActiveTime);
            if (ActiveTime.IsValidated)
                return ActiveTime_ActiveTimeDTO;
            else
                return BadRequest(ActiveTime_ActiveTimeDTO);
        }
        
        [Route(ActiveTimeRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ActiveTimeFilter ActiveTimeFilter = new ActiveTimeFilter();
            ActiveTimeFilter = await ActiveTimeService.ToFilter(ActiveTimeFilter);
            ActiveTimeFilter.Id = new IdFilter { In = Ids };
            ActiveTimeFilter.Selects = ActiveTimeSelect.Id;
            ActiveTimeFilter.Skip = 0;
            ActiveTimeFilter.Take = int.MaxValue;

            List<ActiveTime> ActiveTimes = await ActiveTimeService.List(ActiveTimeFilter);
            ActiveTimes = await ActiveTimeService.BulkDelete(ActiveTimes);
            if (ActiveTimes.Any(x => !x.IsValidated))
                return BadRequest(ActiveTimes.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ActiveTimeRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter MentorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Mentors = await AppUserService.List(MentorFilter);
            List<ActiveTime> ActiveTimes = new List<ActiveTime>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ActiveTimes);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int StartAtColumn = 1 + StartColumn;
                int EndAtColumn = 2 + StartColumn;
                int MentorIdColumn = 3 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string StartAtValue = worksheet.Cells[i, StartAtColumn].Value?.ToString();
                    string EndAtValue = worksheet.Cells[i, EndAtColumn].Value?.ToString();
                    string MentorIdValue = worksheet.Cells[i, MentorIdColumn].Value?.ToString();
                    
                    ActiveTime ActiveTime = new ActiveTime();
                    ActiveTime.StartAt = DateTime.TryParse(StartAtValue, out DateTime StartAt) ? StartAt : DateTime.Now;
                    ActiveTime.EndAt = DateTime.TryParse(EndAtValue, out DateTime EndAt) ? EndAt : DateTime.Now;
                    AppUser Mentor = Mentors.Where(x => x.Id.ToString() == MentorIdValue).FirstOrDefault();
                    ActiveTime.MentorId = Mentor == null ? 0 : Mentor.Id;
                    ActiveTime.Mentor = Mentor;
                    
                    ActiveTimes.Add(ActiveTime);
                }
            }
            ActiveTimes = await ActiveTimeService.Import(ActiveTimes);
            if (ActiveTimes.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ActiveTimes.Count; i++)
                {
                    ActiveTime ActiveTime = ActiveTimes[i];
                    if (!ActiveTime.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ActiveTime.Errors.ContainsKey(nameof(ActiveTime.Id)))
                            Error += ActiveTime.Errors[nameof(ActiveTime.Id)];
                        if (ActiveTime.Errors.ContainsKey(nameof(ActiveTime.StartAt)))
                            Error += ActiveTime.Errors[nameof(ActiveTime.StartAt)];
                        if (ActiveTime.Errors.ContainsKey(nameof(ActiveTime.EndAt)))
                            Error += ActiveTime.Errors[nameof(ActiveTime.EndAt)];
                        if (ActiveTime.Errors.ContainsKey(nameof(ActiveTime.MentorId)))
                            Error += ActiveTime.Errors[nameof(ActiveTime.MentorId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ActiveTimeRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ActiveTime_ActiveTimeFilterDTO ActiveTime_ActiveTimeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ActiveTime
                var ActiveTimeFilter = ConvertFilterDTOToFilterEntity(ActiveTime_ActiveTimeFilterDTO);
                ActiveTimeFilter.Skip = 0;
                ActiveTimeFilter.Take = int.MaxValue;
                ActiveTimeFilter = await ActiveTimeService.ToFilter(ActiveTimeFilter);
                List<ActiveTime> ActiveTimes = await ActiveTimeService.List(ActiveTimeFilter);

                var ActiveTimeHeaders = new List<string>()
                {
                    "Id",
                    "StartAt",
                    "EndAt",
                    "MentorId",
                };
                List<object[]> ActiveTimeData = new List<object[]>();
                for (int i = 0; i < ActiveTimes.Count; i++)
                {
                    var ActiveTime = ActiveTimes[i];
                    ActiveTimeData.Add(new Object[]
                    {
                        ActiveTime.Id,
                        ActiveTime.StartAt,
                        ActiveTime.EndAt,
                        ActiveTime.MentorId,
                    });
                }
                excel.GenerateWorksheet("ActiveTime", ActiveTimeHeaders, ActiveTimeData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "ActiveTime.xlsx");
        }

        [Route(ActiveTimeRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] ActiveTime_ActiveTimeFilterDTO ActiveTime_ActiveTimeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/ActiveTime_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "ActiveTime.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ActiveTimeFilter ActiveTimeFilter = new ActiveTimeFilter();
            ActiveTimeFilter = await ActiveTimeService.ToFilter(ActiveTimeFilter);
            if (Id == 0)
            {

            }
            else
            {
                ActiveTimeFilter.Id = new IdFilter { Equal = Id };
                int count = await ActiveTimeService.Count(ActiveTimeFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ActiveTime ConvertDTOToEntity(ActiveTime_ActiveTimeDTO ActiveTime_ActiveTimeDTO)
        {
            ActiveTime_ActiveTimeDTO.TrimString();
            ActiveTime ActiveTime = new ActiveTime();
            ActiveTime.Id = ActiveTime_ActiveTimeDTO.Id;
            ActiveTime.StartAt = ActiveTime_ActiveTimeDTO.StartAt;
            ActiveTime.EndAt = ActiveTime_ActiveTimeDTO.EndAt;
            ActiveTime.MentorId = ActiveTime_ActiveTimeDTO.MentorId;
            ActiveTime.Mentor = ActiveTime_ActiveTimeDTO.Mentor == null ? null : new AppUser
            {
                Id = ActiveTime_ActiveTimeDTO.Mentor.Id,
                Username = ActiveTime_ActiveTimeDTO.Mentor.Username,
                Email = ActiveTime_ActiveTimeDTO.Mentor.Email,
                Phone = ActiveTime_ActiveTimeDTO.Mentor.Phone,
                Password = ActiveTime_ActiveTimeDTO.Mentor.Password,
                DisplayName = ActiveTime_ActiveTimeDTO.Mentor.DisplayName,
                SexId = ActiveTime_ActiveTimeDTO.Mentor.SexId,
                Birthday = ActiveTime_ActiveTimeDTO.Mentor.Birthday,
                Avatar = ActiveTime_ActiveTimeDTO.Mentor.Avatar,
                CoverImage = ActiveTime_ActiveTimeDTO.Mentor.CoverImage,
            };
            ActiveTime.BaseLanguage = CurrentContext.Language;
            return ActiveTime;
        }

        private ActiveTimeFilter ConvertFilterDTOToFilterEntity(ActiveTime_ActiveTimeFilterDTO ActiveTime_ActiveTimeFilterDTO)
        {
            ActiveTimeFilter ActiveTimeFilter = new ActiveTimeFilter();
            ActiveTimeFilter.Selects = ActiveTimeSelect.ALL;
            ActiveTimeFilter.Skip = ActiveTime_ActiveTimeFilterDTO.Skip;
            ActiveTimeFilter.Take = ActiveTime_ActiveTimeFilterDTO.Take;
            ActiveTimeFilter.OrderBy = ActiveTime_ActiveTimeFilterDTO.OrderBy;
            ActiveTimeFilter.OrderType = ActiveTime_ActiveTimeFilterDTO.OrderType;

            ActiveTimeFilter.Id = ActiveTime_ActiveTimeFilterDTO.Id;
            ActiveTimeFilter.StartAt = ActiveTime_ActiveTimeFilterDTO.StartAt;
            ActiveTimeFilter.EndAt = ActiveTime_ActiveTimeFilterDTO.EndAt;
            ActiveTimeFilter.MentorId = ActiveTime_ActiveTimeFilterDTO.MentorId;
            return ActiveTimeFilter;
        }
    }
}

