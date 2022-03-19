using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueCareer.Repositories;
using TrueCareer.Helpers;

namespace TrueCareer.Service
{
    public interface IFileService : IServiceScoped
    {
        Task<int> Count(FileFilter FileFilter);
        Task<List<File>> List(FileFilter FileFilter);
        Task<File> Get(long Id);
        Task<File> Download(long Id);
        Task<File> Create(File File);
        Task<bool> Delete(long Id);
    }
    public class FileService : IFileService
    {
        public IUOW UOW;
        public ILogging Logging;
        public FileService(IUOW UOW, ILogging Logging)
        {
            this.UOW = UOW;
            this.Logging = Logging;
        }

        public async Task<int> Count(FileFilter FileFilter)
        {
            return await UOW.FileRepository.Count(FileFilter);
        }
        public async Task<List<File>> List(FileFilter FileFilter)
        {
            return await UOW.FileRepository.List(FileFilter);
        }
        public async Task<File> Get(long Id)
        {
            return await UOW.FileRepository.Get(Id);
        }

        public async Task<File> Download(long Id)
        {
            return await UOW.FileRepository.Download(Id);
        }

        public async Task<File> Create(File File)
        {
            File.Path = File.Path.ToLower();
            try
            {
                await UOW.FileRepository.Create(File);
                File = await UOW.FileRepository.Get(File.Id);
                return File;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(FileService));
            }
            return null;
        }

        public async Task<bool> Delete(long Id)
        {
            await UOW.FileRepository.Delete(Id);
            return true;
        }
    }
}
