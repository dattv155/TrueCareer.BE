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
using TrueCareer.Services.MQuestion;
using TrueCareer.Services.MChoice;
using TrueCareer.Services.MMbtiSingleType;

namespace TrueCareer.Rpc.question
{
    public partial class QuestionController : RpcController
    {
        private IChoiceService ChoiceService;
        private IMbtiSingleTypeService MbtiSingleTypeService;
        private IQuestionService QuestionService;
        private ICurrentContext CurrentContext;
        public QuestionController(
            IChoiceService ChoiceService,
            IMbtiSingleTypeService MbtiSingleTypeService,
            IQuestionService QuestionService,
            ICurrentContext CurrentContext
        )
        {
            this.ChoiceService = ChoiceService;
            this.MbtiSingleTypeService = MbtiSingleTypeService;
            this.QuestionService = QuestionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(QuestionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO);
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            int count = await QuestionService.Count(QuestionFilter);
            return count;
        }

        [Route(QuestionRoute.List), HttpPost]
        public async Task<ActionResult<List<Question_QuestionDTO>>> List([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO);
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            List<Question> Questions = await QuestionService.List(QuestionFilter);
            List<Question_QuestionDTO> Question_QuestionDTOs = Questions
                .Select(c => new Question_QuestionDTO(c)).ToList();
            return Question_QuestionDTOs;
        }

        [Route(QuestionRoute.Get), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Get([FromBody]Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = await QuestionService.Get(Question_QuestionDTO.Id);
            return new Question_QuestionDTO(Question);
        }

        [Route(QuestionRoute.Create), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Create([FromBody] Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = ConvertDTOToEntity(Question_QuestionDTO);
            Question = await QuestionService.Create(Question);
            Question_QuestionDTO = new Question_QuestionDTO(Question);
            if (Question.IsValidated)
                return Question_QuestionDTO;
            else
                return BadRequest(Question_QuestionDTO);
        }

        [Route(QuestionRoute.Update), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Update([FromBody] Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = ConvertDTOToEntity(Question_QuestionDTO);
            Question = await QuestionService.Update(Question);
            Question_QuestionDTO = new Question_QuestionDTO(Question);
            if (Question.IsValidated)
                return Question_QuestionDTO;
            else
                return BadRequest(Question_QuestionDTO);
        }

        [Route(QuestionRoute.Delete), HttpPost]
        public async Task<ActionResult<Question_QuestionDTO>> Delete([FromBody] Question_QuestionDTO Question_QuestionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Question_QuestionDTO.Id))
                return Forbid();

            Question Question = ConvertDTOToEntity(Question_QuestionDTO);
            Question = await QuestionService.Delete(Question);
            Question_QuestionDTO = new Question_QuestionDTO(Question);
            if (Question.IsValidated)
                return Question_QuestionDTO;
            else
                return BadRequest(Question_QuestionDTO);
        }
        
        [Route(QuestionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            QuestionFilter.Id = new IdFilter { In = Ids };
            QuestionFilter.Selects = QuestionSelect.Id;
            QuestionFilter.Skip = 0;
            QuestionFilter.Take = int.MaxValue;

            List<Question> Questions = await QuestionService.List(QuestionFilter);
            Questions = await QuestionService.BulkDelete(Questions);
            if (Questions.Any(x => !x.IsValidated))
                return BadRequest(Questions.Where(x => !x.IsValidated));
            return true;
        }

        [Route(QuestionRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Question> Questions = new List<Question>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Questions);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int QuestionContentColumn = 1 + StartColumn;
                int DescriptionColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string QuestionContentValue = worksheet.Cells[i, QuestionContentColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i, DescriptionColumn].Value?.ToString();
                    
                    Question Question = new Question();
                    Question.QuestionContent = QuestionContentValue;
                    Question.Description = DescriptionValue;
                    
                    Questions.Add(Question);
                }
            }
            Questions = await QuestionService.Import(Questions);
            if (Questions.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Questions.Count; i++)
                {
                    Question Question = Questions[i];
                    if (!Question.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Question.Errors.ContainsKey(nameof(Question.Id)))
                            Error += Question.Errors[nameof(Question.Id)];
                        if (Question.Errors.ContainsKey(nameof(Question.QuestionContent)))
                            Error += Question.Errors[nameof(Question.QuestionContent)];
                        if (Question.Errors.ContainsKey(nameof(Question.Description)))
                            Error += Question.Errors[nameof(Question.Description)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(QuestionRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Question
                var QuestionFilter = ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO);
                QuestionFilter.Skip = 0;
                QuestionFilter.Take = int.MaxValue;
                QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
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
                
                #region Choice
                var ChoiceFilter = new ChoiceFilter();
                ChoiceFilter.Selects = ChoiceSelect.ALL;
                ChoiceFilter.OrderBy = ChoiceOrder.Id;
                ChoiceFilter.OrderType = OrderType.ASC;
                ChoiceFilter.Skip = 0;
                ChoiceFilter.Take = int.MaxValue;
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Question.xlsx");
        }

        [Route(QuestionRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Question_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Question.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter = await QuestionService.ToFilter(QuestionFilter);
            if (Id == 0)
            {

            }
            else
            {
                QuestionFilter.Id = new IdFilter { Equal = Id };
                int count = await QuestionService.Count(QuestionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Question ConvertDTOToEntity(Question_QuestionDTO Question_QuestionDTO)
        {
            Question_QuestionDTO.TrimString();
            Question Question = new Question();
            Question.Id = Question_QuestionDTO.Id;
            Question.QuestionContent = Question_QuestionDTO.QuestionContent;
            Question.Description = Question_QuestionDTO.Description;
            Question.Choices = Question_QuestionDTO.Choices?
                .Select(x => new Choice
                {
                    Id = x.Id,
                    ChoiceContent = x.ChoiceContent,
                    Description = x.Description,
                    MbtiSingleTypeId = x.MbtiSingleTypeId,
                    MbtiSingleType = x.MbtiSingleType == null ? null : new MbtiSingleType
                    {
                        Id = x.MbtiSingleType.Id,
                        Code = x.MbtiSingleType.Code,
                        Name = x.MbtiSingleType.Name,
                    },
                }).ToList();
            Question.BaseLanguage = CurrentContext.Language;
            return Question;
        }

        private QuestionFilter ConvertFilterDTOToFilterEntity(Question_QuestionFilterDTO Question_QuestionFilterDTO)
        {
            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter.Selects = QuestionSelect.ALL;
            QuestionFilter.Skip = Question_QuestionFilterDTO.Skip;
            QuestionFilter.Take = Question_QuestionFilterDTO.Take;
            QuestionFilter.OrderBy = Question_QuestionFilterDTO.OrderBy;
            QuestionFilter.OrderType = Question_QuestionFilterDTO.OrderType;

            QuestionFilter.Id = Question_QuestionFilterDTO.Id;
            QuestionFilter.QuestionContent = Question_QuestionFilterDTO.QuestionContent;
            QuestionFilter.Description = Question_QuestionFilterDTO.Description;
            return QuestionFilter;
        }
    }
}

