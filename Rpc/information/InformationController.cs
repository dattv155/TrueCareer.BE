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
using TrueCareer.Services.MInformation;
using TrueCareer.Services.MInformationType;
using TrueCareer.Services.MTopic;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.information
{
    public partial class InformationController : RpcController
    {
        private IInformationTypeService InformationTypeService;
        private ITopicService TopicService;
        private IAppUserService AppUserService;
        private IInformationService InformationService;
        private ICurrentContext CurrentContext;
        public InformationController(
            IInformationTypeService InformationTypeService,
            ITopicService TopicService,
            IAppUserService AppUserService,
            IInformationService InformationService,
            ICurrentContext CurrentContext
        )
        {
            this.InformationTypeService = InformationTypeService;
            this.TopicService = TopicService;
            this.AppUserService = AppUserService;
            this.InformationService = InformationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(InformationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationFilter InformationFilter = ConvertFilterDTOToFilterEntity(Information_InformationFilterDTO);
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            int count = await InformationService.Count(InformationFilter);
            return count;
        }

        [Route(InformationRoute.List), HttpPost]
        public async Task<ActionResult<List<Information_InformationDTO>>> List([FromBody] Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationFilter InformationFilter = ConvertFilterDTOToFilterEntity(Information_InformationFilterDTO);
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            List<Information> Information = await InformationService.List(InformationFilter);
            List<Information_InformationDTO> Information_InformationDTOs = Information
                .Select(c => new Information_InformationDTO(c)).ToList();
            return Information_InformationDTOs;
        }

        [Route(InformationRoute.Get), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Get([FromBody]Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = await InformationService.Get(Information_InformationDTO.Id);
            return new Information_InformationDTO(Information);
        }

        [Route(InformationRoute.Create), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Create([FromBody] Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = ConvertDTOToEntity(Information_InformationDTO);
            Information = await InformationService.Create(Information);
            Information_InformationDTO = new Information_InformationDTO(Information);
            if (Information.IsValidated)
                return Information_InformationDTO;
            else
                return BadRequest(Information_InformationDTO);
        }

        [Route(InformationRoute.Update), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Update([FromBody] Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = ConvertDTOToEntity(Information_InformationDTO);
            Information = await InformationService.Update(Information);
            Information_InformationDTO = new Information_InformationDTO(Information);
            if (Information.IsValidated)
                return Information_InformationDTO;
            else
                return BadRequest(Information_InformationDTO);
        }

        [Route(InformationRoute.Delete), HttpPost]
        public async Task<ActionResult<Information_InformationDTO>> Delete([FromBody] Information_InformationDTO Information_InformationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Information_InformationDTO.Id))
                return Forbid();

            Information Information = ConvertDTOToEntity(Information_InformationDTO);
            Information = await InformationService.Delete(Information);
            Information_InformationDTO = new Information_InformationDTO(Information);
            if (Information.IsValidated)
                return Information_InformationDTO;
            else
                return BadRequest(Information_InformationDTO);
        }
        
        [Route(InformationRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            InformationFilter InformationFilter = new InformationFilter();
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            InformationFilter.Id = new IdFilter { In = Ids };
            InformationFilter.Selects = InformationSelect.Id;
            InformationFilter.Skip = 0;
            InformationFilter.Take = int.MaxValue;

            List<Information> Information = await InformationService.List(InformationFilter);
            Information = await InformationService.BulkDelete(Information);
            if (Information.Any(x => !x.IsValidated))
                return BadRequest(Information.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(InformationRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            InformationTypeFilter InformationTypeFilter = new InformationTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = InformationTypeSelect.ALL
            };
            List<InformationType> InformationTypes = await InformationTypeService.List(InformationTypeFilter);
            TopicFilter TopicFilter = new TopicFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TopicSelect.ALL
            };
            List<Topic> Topics = await TopicService.List(TopicFilter);
            AppUserFilter UserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Users = await AppUserService.List(UserFilter);
            List<Information> Information = new List<Information>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Information);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int InformationTypeIdColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int DescriptionColumn = 3 + StartColumn;
                int StartAtColumn = 4 + StartColumn;
                int RoleColumn = 5 + StartColumn;
                int ImageColumn = 6 + StartColumn;
                int TopicIdColumn = 7 + StartColumn;
                int UserIdColumn = 8 + StartColumn;
                int EndAtColumn = 9 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string InformationTypeIdValue = worksheet.Cells[i, InformationTypeIdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i, NameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i, DescriptionColumn].Value?.ToString();
                    string StartAtValue = worksheet.Cells[i, StartAtColumn].Value?.ToString();
                    string RoleValue = worksheet.Cells[i, RoleColumn].Value?.ToString();
                    string ImageValue = worksheet.Cells[i, ImageColumn].Value?.ToString();
                    string TopicIdValue = worksheet.Cells[i, TopicIdColumn].Value?.ToString();
                    string UserIdValue = worksheet.Cells[i, UserIdColumn].Value?.ToString();
                    string EndAtValue = worksheet.Cells[i, EndAtColumn].Value?.ToString();
                    
                    Information Information = new Information();
                    Information.Name = NameValue;
                    Information.Description = DescriptionValue;
                    Information.StartAt = DateTime.TryParse(StartAtValue, out DateTime StartAt) ? StartAt : DateTime.Now;
                    Information.Role = RoleValue;
                    Information.Image = ImageValue;
                    Information.EndAt = DateTime.TryParse(EndAtValue, out DateTime EndAt) ? EndAt : DateTime.Now;
                    InformationType InformationType = InformationTypes.Where(x => x.Id.ToString() == InformationTypeIdValue).FirstOrDefault();
                    Information.InformationTypeId = InformationType == null ? 0 : InformationType.Id;
                    Information.InformationType = InformationType;
                    Topic Topic = Topics.Where(x => x.Id.ToString() == TopicIdValue).FirstOrDefault();
                    Information.TopicId = Topic == null ? 0 : Topic.Id;
                    Information.Topic = Topic;
                    AppUser User = Users.Where(x => x.Id.ToString() == UserIdValue).FirstOrDefault();
                    Information.UserId = User == null ? 0 : User.Id;
                    Information.User = User;
                    
                    Information.Add(Information);
                }
            }
            Information = await InformationService.Import(Information);
            if (Information.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Information.Count; i++)
                {
                    Information Information = Information[i];
                    if (!Information.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Information.Errors.ContainsKey(nameof(Information.Id)))
                            Error += Information.Errors[nameof(Information.Id)];
                        if (Information.Errors.ContainsKey(nameof(Information.InformationTypeId)))
                            Error += Information.Errors[nameof(Information.InformationTypeId)];
                        if (Information.Errors.ContainsKey(nameof(Information.Name)))
                            Error += Information.Errors[nameof(Information.Name)];
                        if (Information.Errors.ContainsKey(nameof(Information.Description)))
                            Error += Information.Errors[nameof(Information.Description)];
                        if (Information.Errors.ContainsKey(nameof(Information.StartAt)))
                            Error += Information.Errors[nameof(Information.StartAt)];
                        if (Information.Errors.ContainsKey(nameof(Information.Role)))
                            Error += Information.Errors[nameof(Information.Role)];
                        if (Information.Errors.ContainsKey(nameof(Information.Image)))
                            Error += Information.Errors[nameof(Information.Image)];
                        if (Information.Errors.ContainsKey(nameof(Information.TopicId)))
                            Error += Information.Errors[nameof(Information.TopicId)];
                        if (Information.Errors.ContainsKey(nameof(Information.UserId)))
                            Error += Information.Errors[nameof(Information.UserId)];
                        if (Information.Errors.ContainsKey(nameof(Information.EndAt)))
                            Error += Information.Errors[nameof(Information.EndAt)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(InformationRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Information
                var InformationFilter = ConvertFilterDTOToFilterEntity(Information_InformationFilterDTO);
                InformationFilter.Skip = 0;
                InformationFilter.Take = int.MaxValue;
                InformationFilter = await InformationService.ToFilter(InformationFilter);
                List<Information> Information = await InformationService.List(InformationFilter);

                var InformationHeaders = new List<string>()
                {
                    "Id",
                    "InformationTypeId",
                    "Name",
                    "Description",
                    "StartAt",
                    "Role",
                    "Image",
                    "TopicId",
                    "UserId",
                    "EndAt",
                };
                List<object[]> InformationData = new List<object[]>();
                for (int i = 0; i < Information.Count; i++)
                {
                    var Information = Information[i];
                    InformationData.Add(new Object[]
                    {
                        Information.Id,
                        Information.InformationTypeId,
                        Information.Name,
                        Information.Description,
                        Information.StartAt,
                        Information.Role,
                        Information.Image,
                        Information.TopicId,
                        Information.UserId,
                        Information.EndAt,
                    });
                }
                excel.GenerateWorksheet("Information", InformationHeaders, InformationData);
                #endregion
                
                #region InformationType
                var InformationTypeFilter = new InformationTypeFilter();
                InformationTypeFilter.Selects = InformationTypeSelect.ALL;
                InformationTypeFilter.OrderBy = InformationTypeOrder.Id;
                InformationTypeFilter.OrderType = OrderType.ASC;
                InformationTypeFilter.Skip = 0;
                InformationTypeFilter.Take = int.MaxValue;
                List<InformationType> InformationTypes = await InformationTypeService.List(InformationTypeFilter);

                var InformationTypeHeaders = new List<string>()
                {
                    "Id",
                    "Name",
                    "Code",
                };
                List<object[]> InformationTypeData = new List<object[]>();
                for (int i = 0; i < InformationTypes.Count; i++)
                {
                    var InformationType = InformationTypes[i];
                    InformationTypeData.Add(new Object[]
                    {
                        InformationType.Id,
                        InformationType.Name,
                        InformationType.Code,
                    });
                }
                excel.GenerateWorksheet("InformationType", InformationTypeHeaders, InformationTypeData);
                #endregion
                #region Topic
                var TopicFilter = new TopicFilter();
                TopicFilter.Selects = TopicSelect.ALL;
                TopicFilter.OrderBy = TopicOrder.Id;
                TopicFilter.OrderType = OrderType.ASC;
                TopicFilter.Skip = 0;
                TopicFilter.Take = int.MaxValue;
                List<Topic> Topics = await TopicService.List(TopicFilter);

                var TopicHeaders = new List<string>()
                {
                    "Id",
                    "Title",
                    "Description",
                    "Cost",
                };
                List<object[]> TopicData = new List<object[]>();
                for (int i = 0; i < Topics.Count; i++)
                {
                    var Topic = Topics[i];
                    TopicData.Add(new Object[]
                    {
                        Topic.Id,
                        Topic.Title,
                        Topic.Description,
                        Topic.Cost,
                    });
                }
                excel.GenerateWorksheet("Topic", TopicHeaders, TopicData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "Information.xlsx");
        }

        [Route(InformationRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Information_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Information.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            InformationFilter InformationFilter = new InformationFilter();
            InformationFilter = await InformationService.ToFilter(InformationFilter);
            if (Id == 0)
            {

            }
            else
            {
                InformationFilter.Id = new IdFilter { Equal = Id };
                int count = await InformationService.Count(InformationFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Information ConvertDTOToEntity(Information_InformationDTO Information_InformationDTO)
        {
            Information_InformationDTO.TrimString();
            Information Information = new Information();
            Information.Id = Information_InformationDTO.Id;
            Information.InformationTypeId = Information_InformationDTO.InformationTypeId;
            Information.Name = Information_InformationDTO.Name;
            Information.Description = Information_InformationDTO.Description;
            Information.StartAt = Information_InformationDTO.StartAt;
            Information.Role = Information_InformationDTO.Role;
            Information.Image = Information_InformationDTO.Image;
            Information.TopicId = Information_InformationDTO.TopicId;
            Information.UserId = Information_InformationDTO.UserId;
            Information.EndAt = Information_InformationDTO.EndAt;
            Information.InformationType = Information_InformationDTO.InformationType == null ? null : new InformationType
            {
                Id = Information_InformationDTO.InformationType.Id,
                Name = Information_InformationDTO.InformationType.Name,
                Code = Information_InformationDTO.InformationType.Code,
            };
            Information.Topic = Information_InformationDTO.Topic == null ? null : new Topic
            {
                Id = Information_InformationDTO.Topic.Id,
                Title = Information_InformationDTO.Topic.Title,
                Description = Information_InformationDTO.Topic.Description,
                Cost = Information_InformationDTO.Topic.Cost,
            };
            Information.User = Information_InformationDTO.User == null ? null : new AppUser
            {
                Id = Information_InformationDTO.User.Id,
                Username = Information_InformationDTO.User.Username,
                Email = Information_InformationDTO.User.Email,
                Phone = Information_InformationDTO.User.Phone,
                Password = Information_InformationDTO.User.Password,
                DisplayName = Information_InformationDTO.User.DisplayName,
                SexId = Information_InformationDTO.User.SexId,
                Birthday = Information_InformationDTO.User.Birthday,
                Avatar = Information_InformationDTO.User.Avatar,
                CoverImage = Information_InformationDTO.User.CoverImage,
            };
            Information.BaseLanguage = CurrentContext.Language;
            return Information;
        }

        private InformationFilter ConvertFilterDTOToFilterEntity(Information_InformationFilterDTO Information_InformationFilterDTO)
        {
            InformationFilter InformationFilter = new InformationFilter();
            InformationFilter.Selects = InformationSelect.ALL;
            InformationFilter.Skip = Information_InformationFilterDTO.Skip;
            InformationFilter.Take = Information_InformationFilterDTO.Take;
            InformationFilter.OrderBy = Information_InformationFilterDTO.OrderBy;
            InformationFilter.OrderType = Information_InformationFilterDTO.OrderType;

            InformationFilter.Id = Information_InformationFilterDTO.Id;
            InformationFilter.InformationTypeId = Information_InformationFilterDTO.InformationTypeId;
            InformationFilter.Name = Information_InformationFilterDTO.Name;
            InformationFilter.Description = Information_InformationFilterDTO.Description;
            InformationFilter.StartAt = Information_InformationFilterDTO.StartAt;
            InformationFilter.Role = Information_InformationFilterDTO.Role;
            InformationFilter.Image = Information_InformationFilterDTO.Image;
            InformationFilter.TopicId = Information_InformationFilterDTO.TopicId;
            InformationFilter.UserId = Information_InformationFilterDTO.UserId;
            InformationFilter.EndAt = Information_InformationFilterDTO.EndAt;
            InformationFilter.CreatedAt = Information_InformationFilterDTO.CreatedAt;
            InformationFilter.UpdatedAt = Information_InformationFilterDTO.UpdatedAt;
            return InformationFilter;
        }
    }
}

