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
using TrueCareer.Services.MImage;

namespace TrueCareer.Rpc.image
{
    public partial class ImageController : RpcController
    {
        private IImageService ImageService;
        private ICurrentContext CurrentContext;
        public ImageController(
            IImageService ImageService,
            ICurrentContext CurrentContext
        )
        {
            this.ImageService = ImageService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ImageRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
            ImageFilter = await ImageService.ToFilter(ImageFilter);
            int count = await ImageService.Count(ImageFilter);
            return count;
        }

        [Route(ImageRoute.List), HttpPost]
        public async Task<ActionResult<List<Image_ImageDTO>>> List([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
            ImageFilter = await ImageService.ToFilter(ImageFilter);
            List<Image> Images = await ImageService.List(ImageFilter);
            List<Image_ImageDTO> Image_ImageDTOs = Images
                .Select(c => new Image_ImageDTO(c)).ToList();
            return Image_ImageDTOs;
        }

        [Route(ImageRoute.Get), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Get([FromBody]Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = await ImageService.Get(Image_ImageDTO.Id);
            return new Image_ImageDTO(Image);
        }

        [Route(ImageRoute.Create), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Create([FromBody] Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = ConvertDTOToEntity(Image_ImageDTO);
            Image = await ImageService.Create(Image);
            Image_ImageDTO = new Image_ImageDTO(Image);
            if (Image.IsValidated)
                return Image_ImageDTO;
            else
                return BadRequest(Image_ImageDTO);
        }

        [Route(ImageRoute.Update), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Update([FromBody] Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = ConvertDTOToEntity(Image_ImageDTO);
            Image = await ImageService.Update(Image);
            Image_ImageDTO = new Image_ImageDTO(Image);
            if (Image.IsValidated)
                return Image_ImageDTO;
            else
                return BadRequest(Image_ImageDTO);
        }

        [Route(ImageRoute.Delete), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Delete([FromBody] Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = ConvertDTOToEntity(Image_ImageDTO);
            Image = await ImageService.Delete(Image);
            Image_ImageDTO = new Image_ImageDTO(Image);
            if (Image.IsValidated)
                return Image_ImageDTO;
            else
                return BadRequest(Image_ImageDTO);
        }
        
        [Route(ImageRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter = await ImageService.ToFilter(ImageFilter);
            ImageFilter.Id = new IdFilter { In = Ids };
            ImageFilter.Selects = ImageSelect.Id;
            ImageFilter.Skip = 0;
            ImageFilter.Take = int.MaxValue;

            List<Image> Images = await ImageService.List(ImageFilter);
            Images = await ImageService.BulkDelete(Images);
            if (Images.Any(x => !x.IsValidated))
                return BadRequest(Images.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ImageRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<Image> Images = new List<Image>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Images);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int UrlColumn = 2 + StartColumn;
                int ThumbnailUrlColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i, NameColumn].Value?.ToString();
                    string UrlValue = worksheet.Cells[i, UrlColumn].Value?.ToString();
                    string ThumbnailUrlValue = worksheet.Cells[i, ThumbnailUrlColumn].Value?.ToString();
                    
                    Image Image = new Image();
                    Image.Name = NameValue;
                    Image.Url = UrlValue;
                    Image.ThumbnailUrl = ThumbnailUrlValue;
                    
                    Images.Add(Image);
                }
            }
            Images = await ImageService.Import(Images);
            if (Images.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Images.Count; i++)
                {
                    Image Image = Images[i];
                    if (!Image.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Image.Errors.ContainsKey(nameof(Image.Id)))
                            Error += Image.Errors[nameof(Image.Id)];
                        if (Image.Errors.ContainsKey(nameof(Image.Name)))
                            Error += Image.Errors[nameof(Image.Name)];
                        if (Image.Errors.ContainsKey(nameof(Image.Url)))
                            Error += Image.Errors[nameof(Image.Url)];
                        if (Image.Errors.ContainsKey(nameof(Image.ThumbnailUrl)))
                            Error += Image.Errors[nameof(Image.ThumbnailUrl)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ImageRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Image
                var ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
                ImageFilter.Skip = 0;
                ImageFilter.Take = int.MaxValue;
                ImageFilter = await ImageService.ToFilter(ImageFilter);
                List<Image> Images = await ImageService.List(ImageFilter);

                var ImageHeaders = new List<string>()
                {
                    "Id",
                    "Name",
                    "Url",
                    "ThumbnailUrl",
                };
                List<object[]> ImageData = new List<object[]>();
                for (int i = 0; i < Images.Count; i++)
                {
                    var Image = Images[i];
                    ImageData.Add(new Object[]
                    {
                        Image.Id,
                        Image.Name,
                        Image.Url,
                        Image.ThumbnailUrl,
                    });
                }
                excel.GenerateWorksheet("Image", ImageHeaders, ImageData);
                #endregion
                
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "Image.xlsx");
        }

        [Route(ImageRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Image_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Image.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter = await ImageService.ToFilter(ImageFilter);
            if (Id == 0)
            {

            }
            else
            {
                ImageFilter.Id = new IdFilter { Equal = Id };
                int count = await ImageService.Count(ImageFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Image ConvertDTOToEntity(Image_ImageDTO Image_ImageDTO)
        {
            Image_ImageDTO.TrimString();
            Image Image = new Image();
            Image.Id = Image_ImageDTO.Id;
            Image.Name = Image_ImageDTO.Name;
            Image.Url = Image_ImageDTO.Url;
            Image.ThumbnailUrl = Image_ImageDTO.ThumbnailUrl;
            Image.BaseLanguage = CurrentContext.Language;
            return Image;
        }

        private ImageFilter ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.Skip = Image_ImageFilterDTO.Skip;
            ImageFilter.Take = Image_ImageFilterDTO.Take;
            ImageFilter.OrderBy = Image_ImageFilterDTO.OrderBy;
            ImageFilter.OrderType = Image_ImageFilterDTO.OrderType;

            ImageFilter.Id = Image_ImageFilterDTO.Id;
            ImageFilter.Name = Image_ImageFilterDTO.Name;
            ImageFilter.Url = Image_ImageFilterDTO.Url;
            ImageFilter.ThumbnailUrl = Image_ImageFilterDTO.ThumbnailUrl;
            ImageFilter.CreatedAt = Image_ImageFilterDTO.CreatedAt;
            ImageFilter.UpdatedAt = Image_ImageFilterDTO.UpdatedAt;
            return ImageFilter;
        }
    }
}

