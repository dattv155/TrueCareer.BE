using TrueSight.Common;
using TrueSight.Handlers;
using TrueCareer.Common;
using TrueCareer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TrueCareer.Repositories;
using TrueCareer.Entities;
using TrueCareer.Enums;

namespace TrueCareer.Services.MImage
{
    public interface IImageService :  IServiceScoped
    {
        Task<int> Count(ImageFilter ImageFilter);
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<Image> Get(long Id);
        Task<Image> Create(Image Image);
        Task<Image> Update(Image Image);
        Task<Image> Delete(Image Image);
        Task<List<Image>> BulkDelete(List<Image> Images);
        Task<List<Image>> Import(List<Image> Images);
        Task<ImageFilter> ToFilter(ImageFilter ImageFilter);
    }

    public class ImageService : BaseService, IImageService
    {
        private IUOW UOW;
        private IRabbitManager RabbitManager;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        
        private IImageValidator ImageValidator;

        public ImageService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IRabbitManager RabbitManager,
            IImageValidator ImageValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.RabbitManager = RabbitManager;
            this.CurrentContext = CurrentContext;
            this.Logging = Logging;
           
            this.ImageValidator = ImageValidator;
        }
        public async Task<int> Count(ImageFilter ImageFilter)
        {
            try
            {
                int result = await UOW.ImageRepository.Count(ImageFilter);
                return result;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return 0;
        }

        public async Task<List<Image>> List(ImageFilter ImageFilter)
        {
            try
            {
                List<Image> Images = await UOW.ImageRepository.List(ImageFilter);
                return Images;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<Image> Get(long Id)
        {
            Image Image = await UOW.ImageRepository.Get(Id);
            await ImageValidator.Get(Image);
            if (Image == null)
                return null;
            return Image;
        }
        
        public async Task<Image> Create(Image Image)
        {
            if (!await ImageValidator.Create(Image))
                return Image;

            try
            {
                await UOW.ImageRepository.Create(Image);
                Image = await UOW.ImageRepository.Get(Image.Id);
                Logging.CreateAuditLog(Image, new { }, nameof(ImageService));
                return Image;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<Image> Update(Image Image)
        {
            if (!await ImageValidator.Update(Image))
                return Image;
            try
            {
                var oldData = await UOW.ImageRepository.Get(Image.Id);

                await UOW.ImageRepository.Update(Image);

                Image = await UOW.ImageRepository.Get(Image.Id);
                Logging.CreateAuditLog(Image, oldData, nameof(ImageService));
                return Image;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<Image> Delete(Image Image)
        {
            if (!await ImageValidator.Delete(Image))
                return Image;

            try
            {
                await UOW.ImageRepository.Delete(Image);
                Logging.CreateAuditLog(new { }, Image, nameof(ImageService));
                return Image;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }

        public async Task<List<Image>> BulkDelete(List<Image> Images)
        {
            if (!await ImageValidator.BulkDelete(Images))
                return Images;

            try
            {
                await UOW.ImageRepository.BulkDelete(Images);
                Logging.CreateAuditLog(new { }, Images, nameof(ImageService));
                return Images;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;

        }
        
        public async Task<List<Image>> Import(List<Image> Images)
        {
            if (!await ImageValidator.Import(Images))
                return Images;
            try
            {
                await UOW.ImageRepository.BulkMerge(Images);

                Logging.CreateAuditLog(Images, new { }, nameof(ImageService));
                return Images;
            }
            catch (Exception ex)
            {
                Logging.CreateSystemLog(ex, nameof(ImageService));
            }
            return null;
        }     
        
        public async Task<ImageFilter> ToFilter(ImageFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ImageFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ImageFilter subFilter = new ImageFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Url))
                        subFilter.Url = FilterBuilder.Merge(subFilter.Url, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ThumbnailUrl))
                        subFilter.ThumbnailUrl = FilterBuilder.Merge(subFilter.ThumbnailUrl, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private void Sync(List<Image> Images)
        {
            
        }

    }
}
