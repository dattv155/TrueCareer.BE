using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrueCareer.Entities;
using TrueCareer.Service;
using Microsoft.AspNetCore.Authorization;
using TrueCareer.Rpc;

namespace TrueCareer.Rpc.file
{
    public class FileRoute : Root
    {
        private const string Default = Rpc + Module;
        public const string Upload = Default + "/file/upload";
        public const string GetPath01 = Default + "/file/get/{path01}";
        public const string GetPath02 = Default + "/file/get/{path01}/{path02}";
        public const string GetPath03 = Default + "/file/get/{path01}/{path02}/{path03}";
        public const string GetPath04 = Default + "/file/get/{path01}/{path02}/{path03}/{path04}";
        public const string GetPath05 = Default + "/file/get/{path01}/{path02}/{path03}/{path04}/{path05}";
        public const string GetPath06 = Default + "/file/get/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}";
        public const string GetPath07 = Default + "/file/get/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}";
        public const string GetPath08 = Default + "/file/get/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}";
        public const string GetPath09 = Default + "/file/get/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}/{path09}";
        public const string GetPath10 = Default + "/file/get/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}/{path09}/{path10}";

        public const string DownloadPath01 = Default + "/file/download/{path01}";
        public const string DownloadPath02 = Default + "/file/download/{path01}/{path02}";
        public const string DownloadPath03 = Default + "/file/download/{path01}/{path02}/{path03}";
        public const string DownloadPath04 = Default + "/file/download/{path01}/{path02}/{path03}/{path04}";
        public const string DownloadPath05 = Default + "/file/download/{path01}/{path02}/{path03}/{path04}/{path05}";
        public const string DownloadPath06 = Default + "/file/download/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}";
        public const string DownloadPath07 = Default + "/file/download/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}";
        public const string DownloadPath08 = Default + "/file/download/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}";
        public const string DownloadPath09 = Default + "/file/download/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}/{path09}";
        public const string DownloadPath10 = Default + "/file/download/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}/{path09}/{path10}";

        public const string DeletePath01 = Default + "/file/delete/{path01}";
        public const string DeletePath02 = Default + "/file/delete/{path01}/{path02}";
        public const string DeletePath03 = Default + "/file/delete/{path01}/{path02}/{path03}";
        public const string DeletePath04 = Default + "/file/delete/{path01}/{path02}/{path03}/{path04}";
        public const string DeletePath05 = Default + "/file/delete/{path01}/{path02}/{path03}/{path04}/{path05}";
        public const string DeletePath06 = Default + "/file/delete/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}";
        public const string DeletePath07 = Default + "/file/delete/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}";
        public const string DeletePath08 = Default + "/file/delete/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}";
        public const string DeletePath09 = Default + "/file/delete/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}/{path09}";
        public const string DeletePath10 = Default + "/file/delete/{path01}/{path02}/{path03}/{path04}/{path05}/{path06}/{path07}/{path08}/{path09}/{path10}";
    }
    public class FileController : RpcController
    {
        private readonly IFileService FileService;
        public FileController(IFileService FileService)
        {
            this.FileService = FileService;
        }

        [HttpPost, HttpGet, AllowAnonymous]
        [Route(FileRoute.DownloadPath01)]
        [Route(FileRoute.DownloadPath02)]
        [Route(FileRoute.DownloadPath03)]
        [Route(FileRoute.DownloadPath04)]
        [Route(FileRoute.DownloadPath05)]
        [Route(FileRoute.DownloadPath06)]
        [Route(FileRoute.DownloadPath07)]
        [Route(FileRoute.DownloadPath08)]
        [Route(FileRoute.DownloadPath09)]
        [Route(FileRoute.DownloadPath10)]
        public async Task<ActionResult> Download(string path01, string path02, string path03, string path04, string path05,
            string path06, string path07, string path08, string path09, string path10)
        {
            List<string> paths = new List<string>();
            paths.Add("");
            paths.Add("rpc");
            paths.Add("truecareer");
            paths.Add("file");
            paths.Add("download");
            if (!string.IsNullOrWhiteSpace(path01)) paths.Add(path01.ToLower());
            if (!string.IsNullOrWhiteSpace(path02)) paths.Add(path02.ToLower());
            if (!string.IsNullOrWhiteSpace(path03)) paths.Add(path03.ToLower());
            if (!string.IsNullOrWhiteSpace(path04)) paths.Add(path04.ToLower());
            if (!string.IsNullOrWhiteSpace(path05)) paths.Add(path05.ToLower());
            if (!string.IsNullOrWhiteSpace(path06)) paths.Add(path06.ToLower());
            if (!string.IsNullOrWhiteSpace(path07)) paths.Add(path07.ToLower());
            if (!string.IsNullOrWhiteSpace(path08)) paths.Add(path08.ToLower());
            if (!string.IsNullOrWhiteSpace(path09)) paths.Add(path09.ToLower());
            if (!string.IsNullOrWhiteSpace(path10)) paths.Add(path10.ToLower());
            string path = string.Join("/", paths);
            FileFilter fileFilter = new FileFilter
            {
                Path = new StringFilter { Equal = path },
                Skip = 0,
                Take = 1,
            };
            Entities.File file = (await FileService.List(fileFilter)).FirstOrDefault();
            if (file == null)
                return BadRequest();
            file = await FileService.Download(file.Id);
            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition();
            if (file.MimeType.Contains("jpeg") || file.MimeType.Contains("jpg") || file.MimeType.Contains("png") || file.MimeType.Contains("pdf"))
            {
                cd.FileName = file.Name;
                cd.Inline = true;
            }
            else
            {
                cd.FileName = file.Name;
                cd.Inline = false;
            }
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return File(file.Content, file.MimeType);
        }

        [HttpPost, HttpGet, AllowAnonymous]
        [Route(FileRoute.GetPath01)]
        [Route(FileRoute.GetPath02)]
        [Route(FileRoute.GetPath03)]
        [Route(FileRoute.GetPath04)]
        [Route(FileRoute.GetPath05)]
        [Route(FileRoute.GetPath06)]
        [Route(FileRoute.GetPath07)]
        [Route(FileRoute.GetPath08)]
        [Route(FileRoute.GetPath09)]
        [Route(FileRoute.GetPath10)]
        public async Task<ActionResult<Entities.File>> Get(string path01, string path02, string path03, string path04, string path05,
            string path06, string path07, string path08, string path09, string path10)
        {
            List<string> paths = new List<string>();
            paths.Add("");
            if (!string.IsNullOrWhiteSpace(path01)) paths.Add(path01.ToLower());
            if (!string.IsNullOrWhiteSpace(path02)) paths.Add(path02.ToLower());
            if (!string.IsNullOrWhiteSpace(path03)) paths.Add(path03.ToLower());
            if (!string.IsNullOrWhiteSpace(path04)) paths.Add(path04.ToLower());
            if (!string.IsNullOrWhiteSpace(path05)) paths.Add(path05.ToLower());
            if (!string.IsNullOrWhiteSpace(path06)) paths.Add(path06.ToLower());
            if (!string.IsNullOrWhiteSpace(path07)) paths.Add(path07.ToLower());
            if (!string.IsNullOrWhiteSpace(path08)) paths.Add(path08.ToLower());
            if (!string.IsNullOrWhiteSpace(path09)) paths.Add(path09.ToLower());
            if (!string.IsNullOrWhiteSpace(path10)) paths.Add(path10.ToLower());
            string path = string.Join("/", paths);
            FileFilter fileFilter = new FileFilter
            {
                Path = new StringFilter { Equal = path },
                Skip = 0,
                Take = 1,
            };
            Entities.File file = (await FileService.List(fileFilter)).FirstOrDefault();
            return Ok(file);
        }

        [HttpPost]
        [Route(FileRoute.Upload)]
        public async Task<Entities.File> Upload(IFormFile file, [FromForm] string path)
        {
            Entities.File File = new Entities.File();
            File.Path = path;
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            File.Content = memoryStream.ToArray();
            File = await FileService.Create(File);
            return File;
        }
    }
}