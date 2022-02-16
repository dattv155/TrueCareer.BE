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
using TrueCareer.Services.MMajor;

namespace TrueCareer.Rpc.major
{
    public partial class MajorController : RpcController
    {
        private IMajorService MajorService;
        private ICurrentContext CurrentContext;
        public MajorController(
            IMajorService MajorService,
            ICurrentContext CurrentContext
        )
        {
            this.MajorService = MajorService;
            this.CurrentContext = CurrentContext;
        }

        [Route(MajorRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Major_MajorFilterDTO Major_MajorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MajorFilter MajorFilter = ConvertFilterDTOToFilterEntity(Major_MajorFilterDTO);
            MajorFilter = await MajorService.ToFilter(MajorFilter);
            int count = await MajorService.Count(MajorFilter);
            return count;
        }

        [Route(MajorRoute.List), HttpPost]
        public async Task<ActionResult<List<Major_MajorDTO>>> List([FromBody] Major_MajorFilterDTO Major_MajorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MajorFilter MajorFilter = ConvertFilterDTOToFilterEntity(Major_MajorFilterDTO);
            MajorFilter = await MajorService.ToFilter(MajorFilter);
            List<Major> Majors = await MajorService.List(MajorFilter);
            List<Major_MajorDTO> Major_MajorDTOs = Majors
                .Select(c => new Major_MajorDTO(c)).ToList();
            return Major_MajorDTOs;
        }

        [Route(MajorRoute.Get), HttpPost]
        public async Task<ActionResult<Major_MajorDTO>> Get([FromBody]Major_MajorDTO Major_MajorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Major_MajorDTO.Id))
                return Forbid();

            Major Major = await MajorService.Get(Major_MajorDTO.Id);
            return new Major_MajorDTO(Major);
        }

        [Route(MajorRoute.Create), HttpPost]
        public async Task<ActionResult<Major_MajorDTO>> Create([FromBody] Major_MajorDTO Major_MajorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Major_MajorDTO.Id))
                return Forbid();

            Major Major = ConvertDTOToEntity(Major_MajorDTO);
            Major = await MajorService.Create(Major);
            Major_MajorDTO = new Major_MajorDTO(Major);
            if (Major.IsValidated)
                return Major_MajorDTO;
            else
                return BadRequest(Major_MajorDTO);
        }

        [Route(MajorRoute.Update), HttpPost]
        public async Task<ActionResult<Major_MajorDTO>> Update([FromBody] Major_MajorDTO Major_MajorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Major_MajorDTO.Id))
                return Forbid();

            Major Major = ConvertDTOToEntity(Major_MajorDTO);
            Major = await MajorService.Update(Major);
            Major_MajorDTO = new Major_MajorDTO(Major);
            if (Major.IsValidated)
                return Major_MajorDTO;
            else
                return BadRequest(Major_MajorDTO);
        }

        [Route(MajorRoute.Delete), HttpPost]
        public async Task<ActionResult<Major_MajorDTO>> Delete([FromBody] Major_MajorDTO Major_MajorDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Major_MajorDTO.Id))
                return Forbid();

            Major Major = ConvertDTOToEntity(Major_MajorDTO);
            Major = await MajorService.Delete(Major);
            Major_MajorDTO = new Major_MajorDTO(Major);
            if (Major.IsValidated)
                return Major_MajorDTO;
            else
                return BadRequest(Major_MajorDTO);
        }
        
        [Route(MajorRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MajorFilter MajorFilter = new MajorFilter();
            MajorFilter = await MajorService.ToFilter(MajorFilter);
            MajorFilter.Id = new IdFilter { In = Ids };
            MajorFilter.Selects = MajorSelect.Id;
            MajorFilter.Skip = 0;
            MajorFilter.Take = int.MaxValue;

            List<Major> Majors = await MajorService.List(MajorFilter);
            Majors = await MajorService.BulkDelete(Majors);
            if (Majors.Any(x => !x.IsValidated))
                return BadRequest(Majors.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(MajorRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Major> Majors = new List<Major>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Majors);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int DescriptionColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i, NameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i, DescriptionColumn].Value?.ToString();
                    
                    Major Major = new Major();
                    Major.Name = NameValue;
                    Major.Description = DescriptionValue;
                    
                    Majors.Add(Major);
                }
            }
            Majors = await MajorService.Import(Majors);
            if (Majors.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Majors.Count; i++)
                {
                    Major Major = Majors[i];
                    if (!Major.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Major.Errors.ContainsKey(nameof(Major.Id)))
                            Error += Major.Errors[nameof(Major.Id)];
                        if (Major.Errors.ContainsKey(nameof(Major.Name)))
                            Error += Major.Errors[nameof(Major.Name)];
                        if (Major.Errors.ContainsKey(nameof(Major.Description)))
                            Error += Major.Errors[nameof(Major.Description)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(MajorRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Major_MajorFilterDTO Major_MajorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Major
                var MajorFilter = ConvertFilterDTOToFilterEntity(Major_MajorFilterDTO);
                MajorFilter.Skip = 0;
                MajorFilter.Take = int.MaxValue;
                MajorFilter = await MajorService.ToFilter(MajorFilter);
                List<Major> Majors = await MajorService.List(MajorFilter);

                var MajorHeaders = new List<string>()
                {
                    "Id",
                    "Name",
                    "Description",
                };
                List<object[]> MajorData = new List<object[]>();
                for (int i = 0; i < Majors.Count; i++)
                {
                    var Major = Majors[i];
                    MajorData.Add(new Object[]
                    {
                        Major.Id,
                        Major.Name,
                        Major.Description,
                    });
                }
                excel.GenerateWorksheet("Major", MajorHeaders, MajorData);
                #endregion
                
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Major.xlsx");
        }

        [Route(MajorRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Major_MajorFilterDTO Major_MajorFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Major_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Major.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            MajorFilter MajorFilter = new MajorFilter();
            MajorFilter = await MajorService.ToFilter(MajorFilter);
            if (Id == 0)
            {

            }
            else
            {
                MajorFilter.Id = new IdFilter { Equal = Id };
                int count = await MajorService.Count(MajorFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Major ConvertDTOToEntity(Major_MajorDTO Major_MajorDTO)
        {
            Major_MajorDTO.TrimString();
            Major Major = new Major();
            Major.Id = Major_MajorDTO.Id;
            Major.Name = Major_MajorDTO.Name;
            Major.Description = Major_MajorDTO.Description;
            Major.BaseLanguage = CurrentContext.Language;
            return Major;
        }

        private MajorFilter ConvertFilterDTOToFilterEntity(Major_MajorFilterDTO Major_MajorFilterDTO)
        {
            MajorFilter MajorFilter = new MajorFilter();
            MajorFilter.Selects = MajorSelect.ALL;
            MajorFilter.Skip = Major_MajorFilterDTO.Skip;
            MajorFilter.Take = Major_MajorFilterDTO.Take;
            MajorFilter.OrderBy = Major_MajorFilterDTO.OrderBy;
            MajorFilter.OrderType = Major_MajorFilterDTO.OrderType;

            MajorFilter.Id = Major_MajorFilterDTO.Id;
            MajorFilter.Name = Major_MajorFilterDTO.Name;
            MajorFilter.Description = Major_MajorFilterDTO.Description;
            return MajorFilter;
        }
    }
}

