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
using TrueCareer.Services.MTopic;

namespace TrueCareer.Rpc.topic
{
    public partial class TopicController : RpcController
    {
        private ITopicService TopicService;
        private ICurrentContext CurrentContext;
        public TopicController(
            ITopicService TopicService,
            ICurrentContext CurrentContext
        )
        {
            this.TopicService = TopicService;
            this.CurrentContext = CurrentContext;
        }

        [Route(TopicRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Topic_TopicFilterDTO Topic_TopicFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TopicFilter TopicFilter = ConvertFilterDTOToFilterEntity(Topic_TopicFilterDTO);
            TopicFilter = await TopicService.ToFilter(TopicFilter);
            int count = await TopicService.Count(TopicFilter);
            return count;
        }

        [Route(TopicRoute.List), HttpPost]
        public async Task<ActionResult<List<Topic_TopicDTO>>> List([FromBody] Topic_TopicFilterDTO Topic_TopicFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TopicFilter TopicFilter = ConvertFilterDTOToFilterEntity(Topic_TopicFilterDTO);
            TopicFilter = await TopicService.ToFilter(TopicFilter);
            List<Topic> Topics = await TopicService.List(TopicFilter);
            List<Topic_TopicDTO> Topic_TopicDTOs = Topics
                .Select(c => new Topic_TopicDTO(c)).ToList();
            return Topic_TopicDTOs;
        }

        [Route(TopicRoute.Get), HttpPost]
        public async Task<ActionResult<Topic_TopicDTO>> Get([FromBody]Topic_TopicDTO Topic_TopicDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Topic_TopicDTO.Id))
                return Forbid();

            Topic Topic = await TopicService.Get(Topic_TopicDTO.Id);
            return new Topic_TopicDTO(Topic);
        }

        [Route(TopicRoute.Create), HttpPost]
        public async Task<ActionResult<Topic_TopicDTO>> Create([FromBody] Topic_TopicDTO Topic_TopicDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Topic_TopicDTO.Id))
                return Forbid();

            Topic Topic = ConvertDTOToEntity(Topic_TopicDTO);
            Topic = await TopicService.Create(Topic);
            Topic_TopicDTO = new Topic_TopicDTO(Topic);
            if (Topic.IsValidated)
                return Topic_TopicDTO;
            else
                return BadRequest(Topic_TopicDTO);
        }

        [Route(TopicRoute.Update), HttpPost]
        public async Task<ActionResult<Topic_TopicDTO>> Update([FromBody] Topic_TopicDTO Topic_TopicDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Topic_TopicDTO.Id))
                return Forbid();

            Topic Topic = ConvertDTOToEntity(Topic_TopicDTO);
            Topic = await TopicService.Update(Topic);
            Topic_TopicDTO = new Topic_TopicDTO(Topic);
            if (Topic.IsValidated)
                return Topic_TopicDTO;
            else
                return BadRequest(Topic_TopicDTO);
        }

        [Route(TopicRoute.Delete), HttpPost]
        public async Task<ActionResult<Topic_TopicDTO>> Delete([FromBody] Topic_TopicDTO Topic_TopicDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Topic_TopicDTO.Id))
                return Forbid();

            Topic Topic = ConvertDTOToEntity(Topic_TopicDTO);
            Topic = await TopicService.Delete(Topic);
            Topic_TopicDTO = new Topic_TopicDTO(Topic);
            if (Topic.IsValidated)
                return Topic_TopicDTO;
            else
                return BadRequest(Topic_TopicDTO);
        }
        
        [Route(TopicRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TopicFilter TopicFilter = new TopicFilter();
            TopicFilter = await TopicService.ToFilter(TopicFilter);
            TopicFilter.Id = new IdFilter { In = Ids };
            TopicFilter.Selects = TopicSelect.Id;
            TopicFilter.Skip = 0;
            TopicFilter.Take = int.MaxValue;

            List<Topic> Topics = await TopicService.List(TopicFilter);
            Topics = await TopicService.BulkDelete(Topics);
            if (Topics.Any(x => !x.IsValidated))
                return BadRequest(Topics.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(TopicRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Topic> Topics = new List<Topic>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Topics);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int TitleColumn = 1 + StartColumn;
                int DescriptionColumn = 2 + StartColumn;
                int CostColumn = 3 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string TitleValue = worksheet.Cells[i, TitleColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i, DescriptionColumn].Value?.ToString();
                    string CostValue = worksheet.Cells[i, CostColumn].Value?.ToString();
                    
                    Topic Topic = new Topic();
                    Topic.Title = TitleValue;
                    Topic.Description = DescriptionValue;
                    Topic.Cost = decimal.TryParse(CostValue, out decimal Cost) ? Cost : 0;
                    
                    Topics.Add(Topic);
                }
            }
            Topics = await TopicService.Import(Topics);
            if (Topics.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Topics.Count; i++)
                {
                    Topic Topic = Topics[i];
                    if (!Topic.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Topic.Errors.ContainsKey(nameof(Topic.Id)))
                            Error += Topic.Errors[nameof(Topic.Id)];
                        if (Topic.Errors.ContainsKey(nameof(Topic.Title)))
                            Error += Topic.Errors[nameof(Topic.Title)];
                        if (Topic.Errors.ContainsKey(nameof(Topic.Description)))
                            Error += Topic.Errors[nameof(Topic.Description)];
                        if (Topic.Errors.ContainsKey(nameof(Topic.Cost)))
                            Error += Topic.Errors[nameof(Topic.Cost)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(TopicRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Topic_TopicFilterDTO Topic_TopicFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Topic
                var TopicFilter = ConvertFilterDTOToFilterEntity(Topic_TopicFilterDTO);
                TopicFilter.Skip = 0;
                TopicFilter.Take = int.MaxValue;
                TopicFilter = await TopicService.ToFilter(TopicFilter);
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
                
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Topic.xlsx");
        }

        [Route(TopicRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Topic_TopicFilterDTO Topic_TopicFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Topic_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Topic.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            TopicFilter TopicFilter = new TopicFilter();
            TopicFilter = await TopicService.ToFilter(TopicFilter);
            if (Id == 0)
            {

            }
            else
            {
                TopicFilter.Id = new IdFilter { Equal = Id };
                int count = await TopicService.Count(TopicFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Topic ConvertDTOToEntity(Topic_TopicDTO Topic_TopicDTO)
        {
            Topic_TopicDTO.TrimString();
            Topic Topic = new Topic();
            Topic.Id = Topic_TopicDTO.Id;
            Topic.Title = Topic_TopicDTO.Title;
            Topic.Description = Topic_TopicDTO.Description;
            Topic.Cost = Topic_TopicDTO.Cost;
            Topic.BaseLanguage = CurrentContext.Language;
            return Topic;
        }

        private TopicFilter ConvertFilterDTOToFilterEntity(Topic_TopicFilterDTO Topic_TopicFilterDTO)
        {
            TopicFilter TopicFilter = new TopicFilter();
            TopicFilter.Selects = TopicSelect.ALL;
            TopicFilter.Skip = Topic_TopicFilterDTO.Skip;
            TopicFilter.Take = Topic_TopicFilterDTO.Take;
            TopicFilter.OrderBy = Topic_TopicFilterDTO.OrderBy;
            TopicFilter.OrderType = Topic_TopicFilterDTO.OrderType;

            TopicFilter.Id = Topic_TopicFilterDTO.Id;
            TopicFilter.Title = Topic_TopicFilterDTO.Title;
            TopicFilter.Description = Topic_TopicFilterDTO.Description;
            TopicFilter.Cost = Topic_TopicFilterDTO.Cost;
            return TopicFilter;
        }
    }
}

