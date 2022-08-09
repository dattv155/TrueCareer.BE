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
using TrueCareer.Services.MSchool;

namespace TrueCareer.Rpc.school
{
    public partial class SchoolController : RpcController
    {
        private ISchoolService SchoolService;
        private ICurrentContext CurrentContext;
        public SchoolController(
            ISchoolService SchoolService,
            ICurrentContext CurrentContext
        )
        {
            this.SchoolService = SchoolService;
            this.CurrentContext = CurrentContext;
        }

        [Route(SchoolRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] School_SchoolFilterDTO School_SchoolFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SchoolFilter SchoolFilter = ConvertFilterDTOToFilterEntity(School_SchoolFilterDTO);
            SchoolFilter = await SchoolService.ToFilter(SchoolFilter);
            int count = await SchoolService.Count(SchoolFilter);
            return count;
        }

        [Route(SchoolRoute.List), HttpPost]
        public async Task<ActionResult<List<School_SchoolDTO>>> List([FromBody] School_SchoolFilterDTO School_SchoolFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SchoolFilter SchoolFilter = ConvertFilterDTOToFilterEntity(School_SchoolFilterDTO);
            SchoolFilter = await SchoolService.ToFilter(SchoolFilter);
            List<School> Schools = await SchoolService.List(SchoolFilter);
            List<School_SchoolDTO> School_SchoolDTOs = Schools
                .Select(c => new School_SchoolDTO(c)).ToList();
            return School_SchoolDTOs;
        }

        [Route(SchoolRoute.Get), HttpPost]
        public async Task<ActionResult<School_SchoolDTO>> Get([FromBody]School_SchoolDTO School_SchoolDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(School_SchoolDTO.Id))
                return Forbid();

            School School = await SchoolService.Get(School_SchoolDTO.Id);
            return new School_SchoolDTO(School);
        }

        [Route(SchoolRoute.Create), HttpPost]
        public async Task<ActionResult<School_SchoolDTO>> Create([FromBody] School_SchoolDTO School_SchoolDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(School_SchoolDTO.Id))
                return Forbid();

            School School = ConvertDTOToEntity(School_SchoolDTO);
            School = await SchoolService.Create(School);
            School_SchoolDTO = new School_SchoolDTO(School);
            if (School.IsValidated)
                return School_SchoolDTO;
            else
                return BadRequest(School_SchoolDTO);
        }

        [Route(SchoolRoute.Update), HttpPost]
        public async Task<ActionResult<School_SchoolDTO>> Update([FromBody] School_SchoolDTO School_SchoolDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(School_SchoolDTO.Id))
                return Forbid();

            School School = ConvertDTOToEntity(School_SchoolDTO);
            School = await SchoolService.Update(School);
            School_SchoolDTO = new School_SchoolDTO(School);
            if (School.IsValidated)
                return School_SchoolDTO;
            else
                return BadRequest(School_SchoolDTO);
        }

        [Route(SchoolRoute.Delete), HttpPost]
        public async Task<ActionResult<School_SchoolDTO>> Delete([FromBody] School_SchoolDTO School_SchoolDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(School_SchoolDTO.Id))
                return Forbid();

            School School = ConvertDTOToEntity(School_SchoolDTO);
            School = await SchoolService.Delete(School);
            School_SchoolDTO = new School_SchoolDTO(School);
            if (School.IsValidated)
                return School_SchoolDTO;
            else
                return BadRequest(School_SchoolDTO);
        }
        
        [Route(SchoolRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SchoolFilter SchoolFilter = new SchoolFilter();
            SchoolFilter = await SchoolService.ToFilter(SchoolFilter);
            SchoolFilter.Id = new IdFilter { In = Ids };
            SchoolFilter.Selects = SchoolSelect.Id;
            SchoolFilter.Skip = 0;
            SchoolFilter.Take = int.MaxValue;

            List<School> Schools = await SchoolService.List(SchoolFilter);
            Schools = await SchoolService.BulkDelete(Schools);
            if (Schools.Any(x => !x.IsValidated))
                return BadRequest(Schools.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(SchoolRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<School> Schools = new List<School>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Schools);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int DescriptionColumn = 2 + StartColumn;
                int RatingColumn = 4 + StartColumn;
                int CompleteTimeColumn = 5 + StartColumn;
                int StudentCountColumn = 6 + StartColumn;
                int PhoneNumberColumn = 7 + StartColumn;
                int AddressColumn = 8 + StartColumn;
                int SchoolImageColumn = 9 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i, NameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i, DescriptionColumn].Value?.ToString();
                    string RatingValue = worksheet.Cells[i, RatingColumn].Value?.ToString();
                    string CompleteTimeValue = worksheet.Cells[i, CompleteTimeColumn].Value?.ToString();
                    string StudentCountValue = worksheet.Cells[i, StudentCountColumn].Value?.ToString();
                    string PhoneNumberValue = worksheet.Cells[i, PhoneNumberColumn].Value?.ToString();
                    string AddressValue = worksheet.Cells[i, AddressColumn].Value?.ToString();
                    string SchoolImageValue = worksheet.Cells[i, SchoolImageColumn].Value?.ToString();
                    
                    School School = new School();
                    School.Name = NameValue;
                    School.Description = DescriptionValue;
                    School.Rating = decimal.TryParse(RatingValue, out decimal Rating) ? Rating : 0;
                    School.CompleteTime = CompleteTimeValue;
                    School.StudentCount = long.TryParse(StudentCountValue, out long StudentCount) ? StudentCount : 0;
                    School.PhoneNumber = PhoneNumberValue;
                    School.Address = AddressValue;
                    School.SchoolImage = SchoolImageValue;
                    
                    Schools.Add(School);
                }
            }
            Schools = await SchoolService.Import(Schools);
            if (Schools.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Schools.Count; i++)
                {
                    School School = Schools[i];
                    if (!School.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (School.Errors.ContainsKey(nameof(School.Id)))
                            Error += School.Errors[nameof(School.Id)];
                        if (School.Errors.ContainsKey(nameof(School.Name)))
                            Error += School.Errors[nameof(School.Name)];
                        if (School.Errors.ContainsKey(nameof(School.Description)))
                            Error += School.Errors[nameof(School.Description)];
                        if (School.Errors.ContainsKey(nameof(School.Rating)))
                            Error += School.Errors[nameof(School.Rating)];
                        if (School.Errors.ContainsKey(nameof(School.CompleteTime)))
                            Error += School.Errors[nameof(School.CompleteTime)];
                        if (School.Errors.ContainsKey(nameof(School.StudentCount)))
                            Error += School.Errors[nameof(School.StudentCount)];
                        if (School.Errors.ContainsKey(nameof(School.PhoneNumber)))
                            Error += School.Errors[nameof(School.PhoneNumber)];
                        if (School.Errors.ContainsKey(nameof(School.Address)))
                            Error += School.Errors[nameof(School.Address)];
                        if (School.Errors.ContainsKey(nameof(School.SchoolImage)))
                            Error += School.Errors[nameof(School.SchoolImage)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(SchoolRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] School_SchoolFilterDTO School_SchoolFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region School
                var SchoolFilter = ConvertFilterDTOToFilterEntity(School_SchoolFilterDTO);
                SchoolFilter.Skip = 0;
                SchoolFilter.Take = int.MaxValue;
                SchoolFilter = await SchoolService.ToFilter(SchoolFilter);
                List<School> Schools = await SchoolService.List(SchoolFilter);

                var SchoolHeaders = new List<string>()
                {
                    "Id",
                    "Name",
                    "Description",
                    "Rating",
                    "CompleteTime",
                    "StudentCount",
                    "PhoneNumber",
                    "Address",
                    "SchoolImage",
                };
                List<object[]> SchoolData = new List<object[]>();
                for (int i = 0; i < Schools.Count; i++)
                {
                    var School = Schools[i];
                    SchoolData.Add(new Object[]
                    {
                        School.Id,
                        School.Name,
                        School.Description,
                        School.Rating,
                        School.CompleteTime,
                        School.StudentCount,
                        School.PhoneNumber,
                        School.Address,
                        School.SchoolImage,
                    });
                }
                excel.GenerateWorksheet("School", SchoolHeaders, SchoolData);
                #endregion
                
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "School.xlsx");
        }

        [Route(SchoolRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] School_SchoolFilterDTO School_SchoolFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/School_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "School.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            SchoolFilter SchoolFilter = new SchoolFilter();
            SchoolFilter = await SchoolService.ToFilter(SchoolFilter);
            if (Id == 0)
            {

            }
            else
            {
                SchoolFilter.Id = new IdFilter { Equal = Id };
                int count = await SchoolService.Count(SchoolFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private School ConvertDTOToEntity(School_SchoolDTO School_SchoolDTO)
        {
            School_SchoolDTO.TrimString();
            School School = new School();
            School.Id = School_SchoolDTO.Id;
            School.Name = School_SchoolDTO.Name;
            School.Description = School_SchoolDTO.Description;
            School.Rating = School_SchoolDTO.Rating;
            School.CompleteTime = School_SchoolDTO.CompleteTime;
            School.StudentCount = School_SchoolDTO.StudentCount;
            School.PhoneNumber = School_SchoolDTO.PhoneNumber;
            School.Address = School_SchoolDTO.Address;
            School.SchoolImage = School_SchoolDTO.SchoolImage;
            School.SchoolMajorMappings = School_SchoolDTO.SchoolMajorMappings;
            School.BaseLanguage = CurrentContext.Language;
            return School;
        }

        private SchoolFilter ConvertFilterDTOToFilterEntity(School_SchoolFilterDTO School_SchoolFilterDTO)
        {
            SchoolFilter SchoolFilter = new SchoolFilter();
            SchoolFilter.Selects = SchoolSelect.ALL;
            SchoolFilter.Skip = School_SchoolFilterDTO.Skip;
            SchoolFilter.Take = School_SchoolFilterDTO.Take;
            SchoolFilter.OrderBy = School_SchoolFilterDTO.OrderBy;
            SchoolFilter.OrderType = School_SchoolFilterDTO.OrderType;

            SchoolFilter.Id = School_SchoolFilterDTO.Id;
            SchoolFilter.Name = School_SchoolFilterDTO.Name;
            SchoolFilter.Description = School_SchoolFilterDTO.Description;
            SchoolFilter.Rating = School_SchoolFilterDTO.Rating;
            SchoolFilter.CompleteTime = School_SchoolFilterDTO.CompleteTime;
            SchoolFilter.StudentCount = School_SchoolFilterDTO.StudentCount;
            SchoolFilter.PhoneNumber = School_SchoolFilterDTO.PhoneNumber;
            SchoolFilter.Address = School_SchoolFilterDTO.Address;
            SchoolFilter.SchoolImage = School_SchoolFilterDTO.SchoolImage;
            return SchoolFilter;
        }
    }
}

