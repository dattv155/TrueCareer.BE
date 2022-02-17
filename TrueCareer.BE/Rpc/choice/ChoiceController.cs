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
using TrueCareer.Services.MChoice;
using TrueCareer.Services.MMbtiSingleType;
using TrueCareer.Services.MQuestion;

namespace TrueCareer.Rpc.choice
{
    public partial class ChoiceController : RpcController
    {
        private IMbtiSingleTypeService MbtiSingleTypeService;
        private IQuestionService QuestionService;
        private IChoiceService ChoiceService;
        private ICurrentContext CurrentContext;
        public ChoiceController(
            IMbtiSingleTypeService MbtiSingleTypeService,
            IQuestionService QuestionService,
            IChoiceService ChoiceService,
            ICurrentContext CurrentContext
        )
        {
            this.MbtiSingleTypeService = MbtiSingleTypeService;
            this.QuestionService = QuestionService;
            this.ChoiceService = ChoiceService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ChoiceRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Choice_ChoiceFilterDTO Choice_ChoiceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ChoiceFilter ChoiceFilter = ConvertFilterDTOToFilterEntity(Choice_ChoiceFilterDTO);
            ChoiceFilter = await ChoiceService.ToFilter(ChoiceFilter);
            int count = await ChoiceService.Count(ChoiceFilter);
            return count;
        }

        [Route(ChoiceRoute.List), HttpPost]
        public async Task<ActionResult<List<Choice_ChoiceDTO>>> List([FromBody] Choice_ChoiceFilterDTO Choice_ChoiceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ChoiceFilter ChoiceFilter = ConvertFilterDTOToFilterEntity(Choice_ChoiceFilterDTO);
            ChoiceFilter = await ChoiceService.ToFilter(ChoiceFilter);
            List<Choice> Choices = await ChoiceService.List(ChoiceFilter);
            List<Choice_ChoiceDTO> Choice_ChoiceDTOs = Choices
                .Select(c => new Choice_ChoiceDTO(c)).ToList();
            return Choice_ChoiceDTOs;
        }

        [Route(ChoiceRoute.Get), HttpPost]
        public async Task<ActionResult<Choice_ChoiceDTO>> Get([FromBody]Choice_ChoiceDTO Choice_ChoiceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Choice_ChoiceDTO.Id))
                return Forbid();

            Choice Choice = await ChoiceService.Get(Choice_ChoiceDTO.Id);
            return new Choice_ChoiceDTO(Choice);
        }

        [Route(ChoiceRoute.Create), HttpPost]
        public async Task<ActionResult<Choice_ChoiceDTO>> Create([FromBody] Choice_ChoiceDTO Choice_ChoiceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Choice_ChoiceDTO.Id))
                return Forbid();

            Choice Choice = ConvertDTOToEntity(Choice_ChoiceDTO);
            Choice = await ChoiceService.Create(Choice);
            Choice_ChoiceDTO = new Choice_ChoiceDTO(Choice);
            if (Choice.IsValidated)
                return Choice_ChoiceDTO;
            else
                return BadRequest(Choice_ChoiceDTO);
        }

        [Route(ChoiceRoute.Update), HttpPost]
        public async Task<ActionResult<Choice_ChoiceDTO>> Update([FromBody] Choice_ChoiceDTO Choice_ChoiceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Choice_ChoiceDTO.Id))
                return Forbid();

            Choice Choice = ConvertDTOToEntity(Choice_ChoiceDTO);
            Choice = await ChoiceService.Update(Choice);
            Choice_ChoiceDTO = new Choice_ChoiceDTO(Choice);
            if (Choice.IsValidated)
                return Choice_ChoiceDTO;
            else
                return BadRequest(Choice_ChoiceDTO);
        }

        [Route(ChoiceRoute.Delete), HttpPost]
        public async Task<ActionResult<Choice_ChoiceDTO>> Delete([FromBody] Choice_ChoiceDTO Choice_ChoiceDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Choice_ChoiceDTO.Id))
                return Forbid();

            Choice Choice = ConvertDTOToEntity(Choice_ChoiceDTO);
            Choice = await ChoiceService.Delete(Choice);
            Choice_ChoiceDTO = new Choice_ChoiceDTO(Choice);
            if (Choice.IsValidated)
                return Choice_ChoiceDTO;
            else
                return BadRequest(Choice_ChoiceDTO);
        }
        
        [Route(ChoiceRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ChoiceFilter ChoiceFilter = new ChoiceFilter();
            ChoiceFilter = await ChoiceService.ToFilter(ChoiceFilter);
            ChoiceFilter.Id = new IdFilter { In = Ids };
            ChoiceFilter.Selects = ChoiceSelect.Id;
            ChoiceFilter.Skip = 0;
            ChoiceFilter.Take = int.MaxValue;

            List<Choice> Choices = await ChoiceService.List(ChoiceFilter);
            Choices = await ChoiceService.BulkDelete(Choices);
            if (Choices.Any(x => !x.IsValidated))
                return BadRequest(Choices.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ChoiceRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MbtiSingleTypeFilter MbtiSingleTypeFilter = new MbtiSingleTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = MbtiSingleTypeSelect.ALL
            };
            List<MbtiSingleType> MbtiSingleTypes = await MbtiSingleTypeService.List(MbtiSingleTypeFilter);
            QuestionFilter QuestionFilter = new QuestionFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = QuestionSelect.ALL
            };
            List<Question> Questions = await QuestionService.List(QuestionFilter);
            List<Choice> Choices = new List<Choice>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Choices);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int ChoiceContentColumn = 1 + StartColumn;
                int DescriptionColumn = 2 + StartColumn;
                int QuestionIdColumn = 3 + StartColumn;
                int MbtiSingleTypeIdColumn = 4 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string ChoiceContentValue = worksheet.Cells[i, ChoiceContentColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i, DescriptionColumn].Value?.ToString();
                    string QuestionIdValue = worksheet.Cells[i, QuestionIdColumn].Value?.ToString();
                    string MbtiSingleTypeIdValue = worksheet.Cells[i, MbtiSingleTypeIdColumn].Value?.ToString();
                    
                    Choice Choice = new Choice();
                    Choice.ChoiceContent = ChoiceContentValue;
                    Choice.Description = DescriptionValue;
                    MbtiSingleType MbtiSingleType = MbtiSingleTypes.Where(x => x.Id.ToString() == MbtiSingleTypeIdValue).FirstOrDefault();
                    Choice.MbtiSingleTypeId = MbtiSingleType == null ? 0 : MbtiSingleType.Id;
                    Choice.MbtiSingleType = MbtiSingleType;
                    Question Question = Questions.Where(x => x.Id.ToString() == QuestionIdValue).FirstOrDefault();
                    Choice.QuestionId = Question == null ? 0 : Question.Id;
                    Choice.Question = Question;
                    
                    Choices.Add(Choice);
                }
            }
            Choices = await ChoiceService.Import(Choices);
            if (Choices.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Choices.Count; i++)
                {
                    Choice Choice = Choices[i];
                    if (!Choice.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Choice.Errors.ContainsKey(nameof(Choice.Id)))
                            Error += Choice.Errors[nameof(Choice.Id)];
                        if (Choice.Errors.ContainsKey(nameof(Choice.ChoiceContent)))
                            Error += Choice.Errors[nameof(Choice.ChoiceContent)];
                        if (Choice.Errors.ContainsKey(nameof(Choice.Description)))
                            Error += Choice.Errors[nameof(Choice.Description)];
                        if (Choice.Errors.ContainsKey(nameof(Choice.QuestionId)))
                            Error += Choice.Errors[nameof(Choice.QuestionId)];
                        if (Choice.Errors.ContainsKey(nameof(Choice.MbtiSingleTypeId)))
                            Error += Choice.Errors[nameof(Choice.MbtiSingleTypeId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ChoiceRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Choice_ChoiceFilterDTO Choice_ChoiceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Choice
                var ChoiceFilter = ConvertFilterDTOToFilterEntity(Choice_ChoiceFilterDTO);
                ChoiceFilter.Skip = 0;
                ChoiceFilter.Take = int.MaxValue;
                ChoiceFilter = await ChoiceService.ToFilter(ChoiceFilter);
                List<Choice> Choices = await ChoiceService.List(ChoiceFilter);

                var ChoiceHeaders = new List<string>()
                {
                    "Id",
                    "ChoiceContent",
                    "Description",
                    "QuestionId",
                    "MbtiSingleTypeId",
                };
                List<object[]> ChoiceData = new List<object[]>();
                for (int i = 0; i < Choices.Count; i++)
                {
                    var Choice = Choices[i];
                    ChoiceData.Add(new Object[]
                    {
                        Choice.Id,
                        Choice.ChoiceContent,
                        Choice.Description,
                        Choice.QuestionId,
                        Choice.MbtiSingleTypeId,
                    });
                }
                excel.GenerateWorksheet("Choice", ChoiceHeaders, ChoiceData);
                #endregion
                
                #region MbtiSingleType
                var MbtiSingleTypeFilter = new MbtiSingleTypeFilter();
                MbtiSingleTypeFilter.Selects = MbtiSingleTypeSelect.ALL;
                MbtiSingleTypeFilter.OrderBy = MbtiSingleTypeOrder.Id;
                MbtiSingleTypeFilter.OrderType = OrderType.ASC;
                MbtiSingleTypeFilter.Skip = 0;
                MbtiSingleTypeFilter.Take = int.MaxValue;
                List<MbtiSingleType> MbtiSingleTypes = await MbtiSingleTypeService.List(MbtiSingleTypeFilter);

                var MbtiSingleTypeHeaders = new List<string>()
                {
                    "Id",
                    "Code",
                    "Name",
                };
                List<object[]> MbtiSingleTypeData = new List<object[]>();
                for (int i = 0; i < MbtiSingleTypes.Count; i++)
                {
                    var MbtiSingleType = MbtiSingleTypes[i];
                    MbtiSingleTypeData.Add(new Object[]
                    {
                        MbtiSingleType.Id,
                        MbtiSingleType.Code,
                        MbtiSingleType.Name,
                    });
                }
                excel.GenerateWorksheet("MbtiSingleType", MbtiSingleTypeHeaders, MbtiSingleTypeData);
                #endregion
                #region Question
                var QuestionFilter = new QuestionFilter();
                QuestionFilter.Selects = QuestionSelect.ALL;
                QuestionFilter.OrderBy = QuestionOrder.Id;
                QuestionFilter.OrderType = OrderType.ASC;
                QuestionFilter.Skip = 0;
                QuestionFilter.Take = int.MaxValue;
                List<Question> Questions = await QuestionService.List(QuestionFilter);

                var QuestionHeaders = new List<string>()
                {
                    "Id",
                    "QuestionContent",
                    "Description",
                };
                List<object[]> QuestionData = new List<object[]>();
                for (int i = 0; i < Questions.Count; i++)
                {
                    var Question = Questions[i];
                    QuestionData.Add(new Object[]
                    {
                        Question.Id,
                        Question.QuestionContent,
                        Question.Description,
                    });
                }
                excel.GenerateWorksheet("Question", QuestionHeaders, QuestionData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Choice.xlsx");
        }

        [Route(ChoiceRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Choice_ChoiceFilterDTO Choice_ChoiceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Choice_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Choice.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ChoiceFilter ChoiceFilter = new ChoiceFilter();
            ChoiceFilter = await ChoiceService.ToFilter(ChoiceFilter);
            if (Id == 0)
            {

            }
            else
            {
                ChoiceFilter.Id = new IdFilter { Equal = Id };
                int count = await ChoiceService.Count(ChoiceFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Choice ConvertDTOToEntity(Choice_ChoiceDTO Choice_ChoiceDTO)
        {
            Choice_ChoiceDTO.TrimString();
            Choice Choice = new Choice();
            Choice.Id = Choice_ChoiceDTO.Id;
            Choice.ChoiceContent = Choice_ChoiceDTO.ChoiceContent;
            Choice.Description = Choice_ChoiceDTO.Description;
            Choice.QuestionId = Choice_ChoiceDTO.QuestionId;
            Choice.MbtiSingleTypeId = Choice_ChoiceDTO.MbtiSingleTypeId;
            Choice.MbtiSingleType = Choice_ChoiceDTO.MbtiSingleType == null ? null : new MbtiSingleType
            {
                Id = Choice_ChoiceDTO.MbtiSingleType.Id,
                Code = Choice_ChoiceDTO.MbtiSingleType.Code,
                Name = Choice_ChoiceDTO.MbtiSingleType.Name,
            };
            Choice.Question = Choice_ChoiceDTO.Question == null ? null : new Question
            {
                Id = Choice_ChoiceDTO.Question.Id,
                QuestionContent = Choice_ChoiceDTO.Question.QuestionContent,
                Description = Choice_ChoiceDTO.Question.Description,
            };
            Choice.BaseLanguage = CurrentContext.Language;
            return Choice;
        }

        private ChoiceFilter ConvertFilterDTOToFilterEntity(Choice_ChoiceFilterDTO Choice_ChoiceFilterDTO)
        {
            ChoiceFilter ChoiceFilter = new ChoiceFilter();
            ChoiceFilter.Selects = ChoiceSelect.ALL;
            ChoiceFilter.Skip = Choice_ChoiceFilterDTO.Skip;
            ChoiceFilter.Take = Choice_ChoiceFilterDTO.Take;
            ChoiceFilter.OrderBy = Choice_ChoiceFilterDTO.OrderBy;
            ChoiceFilter.OrderType = Choice_ChoiceFilterDTO.OrderType;

            ChoiceFilter.Id = Choice_ChoiceFilterDTO.Id;
            ChoiceFilter.ChoiceContent = Choice_ChoiceFilterDTO.ChoiceContent;
            ChoiceFilter.Description = Choice_ChoiceFilterDTO.Description;
            ChoiceFilter.QuestionId = Choice_ChoiceFilterDTO.QuestionId;
            ChoiceFilter.MbtiSingleTypeId = Choice_ChoiceFilterDTO.MbtiSingleTypeId;
            return ChoiceFilter;
        }
    }
}

