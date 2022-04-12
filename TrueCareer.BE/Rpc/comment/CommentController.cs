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
using TrueCareer.Services.MComment;
using TrueCareer.Services.MAppUser;

namespace TrueCareer.Rpc.comment
{
    public partial class CommentController : RpcController
    {
        private IAppUserService AppUserService;
        private ICommentService CommentService;
        private ICurrentContext CurrentContext;
        public CommentController(
            IAppUserService AppUserService,
            ICommentService CommentService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.CommentService = CommentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(CommentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Comment_CommentFilterDTO Comment_CommentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CommentFilter CommentFilter = ConvertFilterDTOToFilterEntity(Comment_CommentFilterDTO);
            CommentFilter = await CommentService.ToFilter(CommentFilter);
            int count = await CommentService.Count(CommentFilter);
            return count;
        }

        [Route(CommentRoute.List), HttpPost]
        public async Task<ActionResult<List<Comment_CommentDTO>>> List([FromBody] Comment_CommentFilterDTO Comment_CommentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Guid DiscussionId = Comment_CommentFilterDTO.DiscussionId?.Equal ?? Guid.Empty;
            
            List<Comment> Comments = await CommentService.List(DiscussionId, Comment_CommentFilterDTO.OrderType);
            List<Comment_CommentDTO> Comment_CommentDTOs = Comments
                .Select(c => new Comment_CommentDTO(c)).ToList();
            return Comment_CommentDTOs;
        }

        [Route(CommentRoute.Get), HttpPost]
        public async Task<ActionResult<Comment_CommentDTO>> Get([FromBody]Comment_CommentDTO Comment_CommentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Comment_CommentDTO.Id))
                return Forbid();

            Comment Comment = await CommentService.Get(Comment_CommentDTO.Id);
            return new Comment_CommentDTO(Comment);
        }

        [Route(CommentRoute.Create), HttpPost]
        public async Task<ActionResult<Comment_CommentDTO>> Create([FromBody] Comment_CommentDTO Comment_CommentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Comment_CommentDTO.Id))
                return Forbid();

            Comment Comment = ConvertDTOToEntity(Comment_CommentDTO);
            Comment = await CommentService.Create(Comment);
            Comment_CommentDTO = new Comment_CommentDTO(Comment);
            if (Comment.IsValidated)
                return Comment_CommentDTO;
            else
                return BadRequest(Comment_CommentDTO);
        }

        [Route(CommentRoute.Update), HttpPost]
        public async Task<ActionResult<Comment_CommentDTO>> Update([FromBody] Comment_CommentDTO Comment_CommentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(Comment_CommentDTO.Id))
                return Forbid();

            Comment Comment = ConvertDTOToEntity(Comment_CommentDTO);
            Comment = await CommentService.Update(Comment);
            Comment_CommentDTO = new Comment_CommentDTO(Comment);
            if (Comment.IsValidated)
                return Comment_CommentDTO;
            else
                return BadRequest(Comment_CommentDTO);
        }

        [Route(CommentRoute.Delete), HttpPost]
        public async Task<ActionResult<Comment_CommentDTO>> Delete([FromBody] Comment_CommentDTO Comment_CommentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Comment_CommentDTO.Id))
                return Forbid();

            Comment Comment = ConvertDTOToEntity(Comment_CommentDTO);
            Comment = await CommentService.Delete(Comment);
            Comment_CommentDTO = new Comment_CommentDTO(Comment);
            if (Comment.IsValidated)
                return Comment_CommentDTO;
            else
                return BadRequest(Comment_CommentDTO);
        }
        
        [Route(CommentRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            CommentFilter CommentFilter = new CommentFilter();
            CommentFilter = await CommentService.ToFilter(CommentFilter);
            CommentFilter.Id = new IdFilter { In = Ids };
            CommentFilter.Selects = CommentSelect.Id;
            CommentFilter.Skip = 0;
            CommentFilter.Take = int.MaxValue;

            List<Comment> Comments = await CommentService.List(CommentFilter);
            Comments = await CommentService.BulkDelete(Comments);
            if (Comments.Any(x => !x.IsValidated))
                return BadRequest(Comments.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(CommentRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter CreatorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Creators = await AppUserService.List(CreatorFilter);
            List<Comment> Comments = new List<Comment>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(Comments);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int ContentColumn = 1 + StartColumn;
                int CreatorIdColumn = 2 + StartColumn;
                int DiscussionIdColumn = 6 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i, IdColumn].Value?.ToString();
                    string ContentValue = worksheet.Cells[i, ContentColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i, CreatorIdColumn].Value?.ToString();
                    string DiscussionIdValue = worksheet.Cells[i, DiscussionIdColumn].Value?.ToString();
                    
                    Comment Comment = new Comment();
                    Comment.Content = ContentValue;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    Comment.CreatorId = Creator == null ? 0 : Creator.Id;
                    Comment.Creator = Creator;
                    
                    Comments.Add(Comment);
                }
            }
            Comments = await CommentService.Import(Comments);
            if (Comments.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < Comments.Count; i++)
                {
                    Comment Comment = Comments[i];
                    if (!Comment.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (Comment.Errors.ContainsKey(nameof(Comment.Id)))
                            Error += Comment.Errors[nameof(Comment.Id)];
                        if (Comment.Errors.ContainsKey(nameof(Comment.Content)))
                            Error += Comment.Errors[nameof(Comment.Content)];
                        if (Comment.Errors.ContainsKey(nameof(Comment.CreatorId)))
                            Error += Comment.Errors[nameof(Comment.CreatorId)];
                        if (Comment.Errors.ContainsKey(nameof(Comment.DiscussionId)))
                            Error += Comment.Errors[nameof(Comment.DiscussionId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(CommentRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Comment_CommentFilterDTO Comment_CommentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region Comment
                var CommentFilter = ConvertFilterDTOToFilterEntity(Comment_CommentFilterDTO);
                CommentFilter.Skip = 0;
                CommentFilter.Take = int.MaxValue;
                CommentFilter = await CommentService.ToFilter(CommentFilter);
                List<Comment> Comments = await CommentService.List(CommentFilter);

                var CommentHeaders = new List<string>()
                {
                    "Id",
                    "Content",
                    "CreatorId",
                    "DiscussionId",
                };
                List<object[]> CommentData = new List<object[]>();
                for (int i = 0; i < Comments.Count; i++)
                {
                    var Comment = Comments[i];
                    CommentData.Add(new Object[]
                    {
                        Comment.Id,
                        Comment.Content,
                        Comment.CreatorId,
                        Comment.DiscussionId,
                    });
                }
                excel.GenerateWorksheet("Comment", CommentHeaders, CommentData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "Comment.xlsx");
        }

        [Route(CommentRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] Comment_CommentFilterDTO Comment_CommentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            string path = "Templates/Comment_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Comment.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            CommentFilter CommentFilter = new CommentFilter();
            CommentFilter = await CommentService.ToFilter(CommentFilter);
            if (Id == 0)
            {

            }
            else
            {
                CommentFilter.Id = new IdFilter { Equal = Id };
                int count = await CommentService.Count(CommentFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Comment ConvertDTOToEntity(Comment_CommentDTO Comment_CommentDTO)
        {
            Comment_CommentDTO.TrimString();
            Comment Comment = new Comment();
            Comment.Id = Comment_CommentDTO.Id;
            Comment.Content = Comment_CommentDTO.Content;
            Comment.CreatorId = Comment_CommentDTO.CreatorId;
            Comment.DiscussionId = Comment_CommentDTO.DiscussionId;
            Comment.Creator = Comment_CommentDTO.Creator == null ? null : new AppUser
            {
                Id = Comment_CommentDTO.Creator.Id,
                Username = Comment_CommentDTO.Creator.Username,
                Email = Comment_CommentDTO.Creator.Email,
                Phone = Comment_CommentDTO.Creator.Phone,
                Password = Comment_CommentDTO.Creator.Password,
                DisplayName = Comment_CommentDTO.Creator.DisplayName,
                SexId = Comment_CommentDTO.Creator.SexId,
                Birthday = Comment_CommentDTO.Creator.Birthday,
                Avatar = Comment_CommentDTO.Creator.Avatar,
                CoverImage = Comment_CommentDTO.Creator.CoverImage,
            };
            Comment.BaseLanguage = CurrentContext.Language;
            return Comment;
        }

        private CommentFilter ConvertFilterDTOToFilterEntity(Comment_CommentFilterDTO Comment_CommentFilterDTO)
        {
            CommentFilter CommentFilter = new CommentFilter();
            CommentFilter.Selects = CommentSelect.ALL;
            CommentFilter.Skip = Comment_CommentFilterDTO.Skip;
            CommentFilter.Take = Comment_CommentFilterDTO.Take;
            CommentFilter.OrderBy = Comment_CommentFilterDTO.OrderBy;
            CommentFilter.OrderType = Comment_CommentFilterDTO.OrderType;

            CommentFilter.Id = Comment_CommentFilterDTO.Id;
            CommentFilter.Content = Comment_CommentFilterDTO.Content;
            CommentFilter.CreatorId = Comment_CommentFilterDTO.CreatorId;
            CommentFilter.DiscussionId = Comment_CommentFilterDTO.DiscussionId;
            CommentFilter.CreatedAt = Comment_CommentFilterDTO.CreatedAt;
            CommentFilter.UpdatedAt = Comment_CommentFilterDTO.UpdatedAt;
            return CommentFilter;
        }
    }
}

