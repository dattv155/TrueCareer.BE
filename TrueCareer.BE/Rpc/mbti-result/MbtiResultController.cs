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
using TrueCareer.Enums;
using TrueCareer.Services.MMbtiResult;
using TrueCareer.Services.MMbtiPersonalType;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.mbti_result
{
    public partial class MbtiResultController : RpcController
    {
        private IMbtiPersonalTypeService MbtiPersonalTypeService;
        private IAppUserService AppUserService;
        private IMbtiResultService MbtiResultService;
        private ICurrentContext CurrentContext;
        public MbtiResultController(
            IMbtiPersonalTypeService MbtiPersonalTypeService,
            IAppUserService AppUserService,
            IMbtiResultService MbtiResultService,
            ICurrentContext CurrentContext
        )
        {
            this.MbtiPersonalTypeService = MbtiPersonalTypeService;
            this.AppUserService = AppUserService;
            this.MbtiResultService = MbtiResultService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MbtiResultRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] MbtiResult_MbtiResultFilterDTO MbtiResult_MbtiResultFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MbtiResultFilter MbtiResultFilter = ConvertFilterDTOToFilterEntity(MbtiResult_MbtiResultFilterDTO);
            MbtiResultFilter = await MbtiResultService.ToFilter(MbtiResultFilter);
            int count = await MbtiResultService.Count(MbtiResultFilter);
            return count;
        }

        [Route(MbtiResultRoute.List), HttpPost]
        public async Task<ActionResult<List<MbtiResult_MbtiResultDTO>>> List([FromBody] MbtiResult_MbtiResultFilterDTO MbtiResult_MbtiResultFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MbtiResultFilter MbtiResultFilter = ConvertFilterDTOToFilterEntity(MbtiResult_MbtiResultFilterDTO);
            MbtiResultFilter = await MbtiResultService.ToFilter(MbtiResultFilter);
            List<MbtiResult> MbtiResults = await MbtiResultService.List(MbtiResultFilter);
            List<MbtiResult_MbtiResultDTO> MbtiResult_MbtiResultDTOs = MbtiResults
                .Select(c => new MbtiResult_MbtiResultDTO(c)).ToList();
            return MbtiResult_MbtiResultDTOs;
        }

        [Route(MbtiResultRoute.Get), HttpPost]
        public async Task<ActionResult<MbtiResult_MbtiResultDTO>> Get([FromBody]MbtiResult_MbtiResultDTO MbtiResult_MbtiResultDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MbtiResult_MbtiResultDTO.Id))
                return Forbid();

            MbtiResult MbtiResult = await MbtiResultService.Get(MbtiResult_MbtiResultDTO.Id);
            return new MbtiResult_MbtiResultDTO(MbtiResult);
        }

        [Route(MbtiResultRoute.Create), HttpPost]
        public async Task<ActionResult<MbtiResult_MbtiResultDTO>> Create([FromBody] MbtiResult_MbtiResultDTO MbtiResult_MbtiResultDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(MbtiResult_MbtiResultDTO.Id))
                return Forbid();

            MbtiResult MbtiResult = ConvertDTOToEntity(MbtiResult_MbtiResultDTO);
            MbtiResult = await MbtiResultService.Create(MbtiResult);
            MbtiResult_MbtiResultDTO = new MbtiResult_MbtiResultDTO(MbtiResult);
            if (MbtiResult.IsValidated)
                return MbtiResult_MbtiResultDTO;
            else
                return BadRequest(MbtiResult_MbtiResultDTO);
        }

        [Route(MbtiResultRoute.Update), HttpPost]
        public async Task<ActionResult<MbtiResult_MbtiResultDTO>> Update([FromBody] MbtiResult_MbtiResultDTO MbtiResult_MbtiResultDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(MbtiResult_MbtiResultDTO.Id))
                return Forbid();

            MbtiResult MbtiResult = ConvertDTOToEntity(MbtiResult_MbtiResultDTO);
            MbtiResult = await MbtiResultService.Update(MbtiResult);
            MbtiResult_MbtiResultDTO = new MbtiResult_MbtiResultDTO(MbtiResult);
            if (MbtiResult.IsValidated)
                return MbtiResult_MbtiResultDTO;
            else
                return BadRequest(MbtiResult_MbtiResultDTO);
        }

        [Route(MbtiResultRoute.Delete), HttpPost]
        public async Task<ActionResult<MbtiResult_MbtiResultDTO>> Delete([FromBody] MbtiResult_MbtiResultDTO MbtiResult_MbtiResultDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(MbtiResult_MbtiResultDTO.Id))
                return Forbid();

            MbtiResult MbtiResult = ConvertDTOToEntity(MbtiResult_MbtiResultDTO);
            MbtiResult = await MbtiResultService.Delete(MbtiResult);
            MbtiResult_MbtiResultDTO = new MbtiResult_MbtiResultDTO(MbtiResult);
            if (MbtiResult.IsValidated)
                return MbtiResult_MbtiResultDTO;
            else
                return BadRequest(MbtiResult_MbtiResultDTO);
        }
        
        [Route(MbtiResultRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MbtiResultFilter MbtiResultFilter = new MbtiResultFilter();
            MbtiResultFilter = await MbtiResultService.ToFilter(MbtiResultFilter);
            MbtiResultFilter.Id = new IdFilter { In = Ids };
            MbtiResultFilter.Selects = MbtiResultSelect.Id;
            MbtiResultFilter.Skip = 0;
            MbtiResultFilter.Take = int.MaxValue;

            List<MbtiResult> MbtiResults = await MbtiResultService.List(MbtiResultFilter);
            MbtiResults = await MbtiResultService.BulkDelete(MbtiResults);
            if (MbtiResults.Any(x => !x.IsValidated))
                return BadRequest(MbtiResults.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(MbtiResultRoute.CalcResult), HttpPost]
        public async Task<ActionResult<MbtiResult_MbtiResultDTO>> CalcResult([FromBody] List<long> SingleTypeIds)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MbtiResult MbtiResult = new MbtiResult();
            MbtiResult = await MbtiResultService.CalcResult(SingleTypeIds);
            
            MbtiResult_MbtiResultDTO MbtiResult_MbtiResultDTO = new MbtiResult_MbtiResultDTO(MbtiResult);
            if (MbtiResult.IsValidated)
                return MbtiResult_MbtiResultDTO;
            else
                return BadRequest(MbtiResult_MbtiResultDTO);
        }
        
        [Route(MbtiResultRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MbtiPersonalTypeFilter MbtiPersonalTypeFilter = new MbtiPersonalTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = MbtiPersonalTypeSelect.ALL
            };
            List<MbtiPersonalType> MbtiPersonalTypes = await MbtiPersonalTypeService.List(MbtiPersonalTypeFilter);
            AppUserFilter UserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Users = await AppUserService.List(UserFilter);
            List<MbtiResult> MbtiResults = new List<MbtiResult>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(MbtiResults);
                int StartColumn = 1;
                int StartRow = 1;
                int UserIdColumn = 0 + StartColumn;
                int MbtiPersonalTypeIdColumn = 1 + StartColumn;
                int IdColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string UserIdValue = worksheet.Cells[i, UserIdColumn].Value?.ToString();
                    string MbtiPersonalTypeIdValue = worksheet.Cells[i, MbtiPersonalTypeIdColumn].Value?.ToString();
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    
                    MbtiResult MbtiResult = new MbtiResult();
                    MbtiPersonalType MbtiPersonalType = MbtiPersonalTypes.Where(x => x.Id.ToString() == MbtiPersonalTypeIdValue).FirstOrDefault();
                    MbtiResult.MbtiPersonalTypeId = MbtiPersonalType == null ? 0 : MbtiPersonalType.Id;
                    MbtiResult.MbtiPersonalType = MbtiPersonalType;
                    AppUser User = Users.Where(x => x.Id.ToString() == UserIdValue).FirstOrDefault();
                    MbtiResult.UserId = User == null ? 0 : User.Id;
                    MbtiResult.User = User;
                    
                    MbtiResults.Add(MbtiResult);
                }
            }
            MbtiResults = await MbtiResultService.Import(MbtiResults);
            if (MbtiResults.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < MbtiResults.Count; i++)
                {
                    MbtiResult MbtiResult = MbtiResults[i];
                    if (!MbtiResult.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (MbtiResult.Errors.ContainsKey(nameof(MbtiResult.UserId)))
                            Error += MbtiResult.Errors[nameof(MbtiResult.UserId)];
                        if (MbtiResult.Errors.ContainsKey(nameof(MbtiResult.MbtiPersonalTypeId)))
                            Error += MbtiResult.Errors[nameof(MbtiResult.MbtiPersonalTypeId)];
                        if (MbtiResult.Errors.ContainsKey(nameof(MbtiResult.Id)))
                            Error += MbtiResult.Errors[nameof(MbtiResult.Id)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(MbtiResultRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] MbtiResult_MbtiResultFilterDTO MbtiResult_MbtiResultFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region MbtiResult
                var MbtiResultFilter = ConvertFilterDTOToFilterEntity(MbtiResult_MbtiResultFilterDTO);
                MbtiResultFilter.Skip = 0;
                MbtiResultFilter.Take = int.MaxValue;
                MbtiResultFilter = await MbtiResultService.ToFilter(MbtiResultFilter);
                List<MbtiResult> MbtiResults = await MbtiResultService.List(MbtiResultFilter);

                var MbtiResultHeaders = new List<string>()
                {
                    "UserId",
                    "MbtiPersonalTypeId",
                    "Id",
                };
                List<object[]> MbtiResultData = new List<object[]>();
                for (int i = 0; i < MbtiResults.Count; i++)
                {
                    var MbtiResult = MbtiResults[i];
                    MbtiResultData.Add(new Object[]
                    {
                        MbtiResult.UserId,
                        MbtiResult.MbtiPersonalTypeId,
                        MbtiResult.Id,
                    });
                }
                excel.GenerateWorksheet("MbtiResult", MbtiResultHeaders, MbtiResultData);
                #endregion
                
                #region MbtiPersonalType
                var MbtiPersonalTypeFilter = new MbtiPersonalTypeFilter();
                MbtiPersonalTypeFilter.Selects = MbtiPersonalTypeSelect.ALL;
                MbtiPersonalTypeFilter.OrderBy = MbtiPersonalTypeOrder.Id;
                MbtiPersonalTypeFilter.OrderType = OrderType.ASC;
                MbtiPersonalTypeFilter.Skip = 0;
                MbtiPersonalTypeFilter.Take = int.MaxValue;
                List<MbtiPersonalType> MbtiPersonalTypes = await MbtiPersonalTypeService.List(MbtiPersonalTypeFilter);

                var MbtiPersonalTypeHeaders = new List<string>()
                {
                    "Id",
                    "Name",
                    "Code",
                };
                List<object[]> MbtiPersonalTypeData = new List<object[]>();
                for (int i = 0; i < MbtiPersonalTypes.Count; i++)
                {
                    var MbtiPersonalType = MbtiPersonalTypes[i];
                    MbtiPersonalTypeData.Add(new Object[]
                    {
                        MbtiPersonalType.Id,
                        MbtiPersonalType.Name,
                        MbtiPersonalType.Code,
                    });
                }
                excel.GenerateWorksheet("MbtiPersonalType", MbtiPersonalTypeHeaders, MbtiPersonalTypeData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "MbtiResult.xlsx");
        }

        [Route(MbtiResultRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] MbtiResult_MbtiResultFilterDTO MbtiResult_MbtiResultFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/MbtiResult_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "MbtiResult.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            MbtiResultFilter MbtiResultFilter = new MbtiResultFilter();
            MbtiResultFilter = await MbtiResultService.ToFilter(MbtiResultFilter);
            if (Id == 0)
            {

            }
            else
            {
                MbtiResultFilter.Id = new IdFilter { Equal = Id };
                int count = await MbtiResultService.Count(MbtiResultFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private MbtiResult ConvertDTOToEntity(MbtiResult_MbtiResultDTO MbtiResult_MbtiResultDTO)
        {
            MbtiResult_MbtiResultDTO.TrimString();
            MbtiResult MbtiResult = new MbtiResult();
            MbtiResult.UserId = MbtiResult_MbtiResultDTO.UserId;
            MbtiResult.MbtiPersonalTypeId = MbtiResult_MbtiResultDTO.MbtiPersonalTypeId;
            MbtiResult.Id = MbtiResult_MbtiResultDTO.Id;
            MbtiResult.MbtiPersonalType = MbtiResult_MbtiResultDTO.MbtiPersonalType == null ? null : new MbtiPersonalType
            {
                Id = MbtiResult_MbtiResultDTO.MbtiPersonalType.Id,
                Name = MbtiResult_MbtiResultDTO.MbtiPersonalType.Name,
                Code = MbtiResult_MbtiResultDTO.MbtiPersonalType.Code,
                Value = MbtiResult_MbtiResultDTO.MbtiPersonalType.Value,
                MbtiPersonalTypeMajorMappings = MbtiResult_MbtiResultDTO.MbtiPersonalType.MbtiPersonalTypeMajorMappings
            };
            MbtiResult.User = MbtiResult_MbtiResultDTO.User == null ? null : new AppUser
            {
                Id = MbtiResult_MbtiResultDTO.User.Id,
                Username = MbtiResult_MbtiResultDTO.User.Username,
                Email = MbtiResult_MbtiResultDTO.User.Email,
                Phone = MbtiResult_MbtiResultDTO.User.Phone,
                Password = MbtiResult_MbtiResultDTO.User.Password,
                DisplayName = MbtiResult_MbtiResultDTO.User.DisplayName,
                SexId = MbtiResult_MbtiResultDTO.User.SexId,
                Birthday = MbtiResult_MbtiResultDTO.User.Birthday,
                Avatar = MbtiResult_MbtiResultDTO.User.Avatar,
                CoverImage = MbtiResult_MbtiResultDTO.User.CoverImage,
            };
            MbtiResult.BaseLanguage = CurrentContext.Language;
            return MbtiResult;
        }

        private MbtiResultFilter ConvertFilterDTOToFilterEntity(MbtiResult_MbtiResultFilterDTO MbtiResult_MbtiResultFilterDTO)
        {
            MbtiResultFilter MbtiResultFilter = new MbtiResultFilter();
            MbtiResultFilter.Selects = MbtiResultSelect.ALL;
            MbtiResultFilter.Skip = MbtiResult_MbtiResultFilterDTO.Skip;
            MbtiResultFilter.Take = MbtiResult_MbtiResultFilterDTO.Take;
            MbtiResultFilter.OrderBy = MbtiResult_MbtiResultFilterDTO.OrderBy;
            MbtiResultFilter.OrderType = MbtiResult_MbtiResultFilterDTO.OrderType;

            MbtiResultFilter.UserId = MbtiResult_MbtiResultFilterDTO.UserId;
            MbtiResultFilter.MbtiPersonalTypeId = MbtiResult_MbtiResultFilterDTO.MbtiPersonalTypeId;
            MbtiResultFilter.Id = MbtiResult_MbtiResultFilterDTO.Id;
            return MbtiResultFilter;
        }
    }
}

