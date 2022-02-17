using TrueSight.Common;
using TrueCareer.Common;
using TrueCareer.Helpers;
using TrueCareer.Entities;
using TrueCareer.Models;
using TrueCareer.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace TrueCareer.Repositories
{
    public interface IImageRepository
    {
        Task<int> CountAll(ImageFilter ImageFilter);
        Task<int> Count(ImageFilter ImageFilter);
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<List<Image>> List(List<long> Ids);
        Task<Image> Get(long Id);
        Task<bool> Create(Image Image);
        Task<bool> Update(Image Image);
        Task<bool> Delete(Image Image);
        Task<bool> BulkMerge(List<Image> Images);
        Task<bool> BulkDelete(List<Image> Images);
    }
    public class ImageRepository : IImageRepository
    {
        private DataContext DataContext;
        public ImageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<ImageDAO>> DynamicFilter(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = query.Where(q => q.Url, filter.Url);
            query = query.Where(q => q.ThumbnailUrl, filter.ThumbnailUrl);
            return query;
        }

        private async Task<IQueryable<ImageDAO>> OrFilter(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ImageDAO> initQuery = query.Where(q => false);
            foreach (ImageFilter ImageFilter in filter.OrFilter)
            {
                IQueryable<ImageDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, ImageFilter.Id);
                queryable = queryable.Where(q => q.Name, ImageFilter.Name);
                queryable = queryable.Where(q => q.Url, ImageFilter.Url);
                queryable = queryable.Where(q => q.ThumbnailUrl, ImageFilter.ThumbnailUrl);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ImageDAO> DynamicOrder(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ImageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ImageOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ImageOrder.Url:
                            query = query.OrderBy(q => q.Url);
                            break;
                        case ImageOrder.ThumbnailUrl:
                            query = query.OrderBy(q => q.ThumbnailUrl);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ImageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ImageOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ImageOrder.Url:
                            query = query.OrderByDescending(q => q.Url);
                            break;
                        case ImageOrder.ThumbnailUrl:
                            query = query.OrderByDescending(q => q.ThumbnailUrl);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Image>> DynamicSelect(IQueryable<ImageDAO> query, ImageFilter filter)
        {
            List<Image> Images = await query.Select(q => new Image()
            {
                Id = filter.Selects.Contains(ImageSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ImageSelect.Name) ? q.Name : default(string),
                Url = filter.Selects.Contains(ImageSelect.Url) ? q.Url : default(string),
                ThumbnailUrl = filter.Selects.Contains(ImageSelect.ThumbnailUrl) ? q.ThumbnailUrl : default(string),
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return Images;
        }

        public async Task<int> CountAll(ImageFilter filter)
        {
            IQueryable<ImageDAO> ImageDAOs = DataContext.Image.AsNoTracking();
            ImageDAOs = await DynamicFilter(ImageDAOs, filter);
            return await ImageDAOs.CountAsync();
        }

        public async Task<int> Count(ImageFilter filter)
        {
            IQueryable<ImageDAO> ImageDAOs = DataContext.Image.AsNoTracking();
            ImageDAOs = await DynamicFilter(ImageDAOs, filter);
            ImageDAOs = await OrFilter(ImageDAOs, filter);
            return await ImageDAOs.CountAsync();
        }

        public async Task<List<Image>> List(ImageFilter filter)
        {
            if (filter == null) return new List<Image>();
            IQueryable<ImageDAO> ImageDAOs = DataContext.Image.AsNoTracking();
            ImageDAOs = await DynamicFilter(ImageDAOs, filter);
            ImageDAOs = await OrFilter(ImageDAOs, filter);
            ImageDAOs = DynamicOrder(ImageDAOs, filter);
            List<Image> Images = await DynamicSelect(ImageDAOs, filter);
            return Images;
        }

        public async Task<List<Image>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<ImageDAO> query = DataContext.Image.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<Image> Images = await query.AsNoTracking()
            .Select(x => new Image()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Name = x.Name,
                Url = x.Url,
                ThumbnailUrl = x.ThumbnailUrl,
            }).ToListAsync();
            

            return Images;
        }

        public async Task<Image> Get(long Id)
        {
            Image Image = await DataContext.Image.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Image()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Name = x.Name,
                Url = x.Url,
                ThumbnailUrl = x.ThumbnailUrl,
            }).FirstOrDefaultAsync();

            if (Image == null)
                return null;

            return Image;
        }
        
        public async Task<bool> Create(Image Image)
        {
            ImageDAO ImageDAO = new ImageDAO();
            ImageDAO.Id = Image.Id;
            ImageDAO.Name = Image.Name;
            ImageDAO.Url = Image.Url;
            ImageDAO.ThumbnailUrl = Image.ThumbnailUrl;
            ImageDAO.CreatedAt = StaticParams.DateTimeNow;
            ImageDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Image.Add(ImageDAO);
            await DataContext.SaveChangesAsync();
            Image.Id = ImageDAO.Id;
            await SaveReference(Image);
            return true;
        }

        public async Task<bool> Update(Image Image)
        {
            ImageDAO ImageDAO = DataContext.Image
                .Where(x => x.Id == Image.Id)
                .FirstOrDefault();
            if (ImageDAO == null)
                return false;
            ImageDAO.Id = Image.Id;
            ImageDAO.Name = Image.Name;
            ImageDAO.Url = Image.Url;
            ImageDAO.ThumbnailUrl = Image.ThumbnailUrl;
            ImageDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Image);
            return true;
        }

        public async Task<bool> Delete(Image Image)
        {
            await DataContext.Image
                .Where(x => x.Id == Image.Id)
                .UpdateFromQueryAsync(x => new ImageDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Image> Images)
        {
            IdFilter IdFilter = new IdFilter { In = Images.Select(x => x.Id).ToList() };
            List<ImageDAO> ImageDAOs = new List<ImageDAO>();
            List<ImageDAO> DbImageDAOs = await DataContext.Image
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (Image Image in Images)
            {
                ImageDAO ImageDAO = DbImageDAOs
                        .Where(x => x.Id == Image.Id)
                        .FirstOrDefault();
                if (ImageDAO == null)
                {
                    ImageDAO = new ImageDAO();
                    ImageDAO.CreatedAt = StaticParams.DateTimeNow;
                }
                ImageDAO.Name = Image.Name;
                ImageDAO.Url = Image.Url;
                ImageDAO.ThumbnailUrl = Image.ThumbnailUrl;
                ImageDAO.UpdatedAt = StaticParams.DateTimeNow;
                ImageDAOs.Add(ImageDAO);
            }
            await DataContext.BulkMergeAsync(ImageDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Image> Images)
        {
            List<long> Ids = Images.Select(x => x.Id).ToList();
            await DataContext.Image
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new ImageDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(Image Image)
        {
        }
        
    }
}
