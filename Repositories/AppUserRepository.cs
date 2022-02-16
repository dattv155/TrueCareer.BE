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
    public interface IAppUserRepository
    {
        Task<int> CountAll(AppUserFilter AppUserFilter);
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(List<long> Ids);
        Task<AppUser> Get(long Id);
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkMerge(List<AppUser> AppUsers);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
    }
    public class AppUserRepository : IAppUserRepository
    {
        private DataContext DataContext;
        public AppUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<AppUserDAO>> DynamicFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Username, filter.Username);
            query = query.Where(q => q.Email, filter.Email);
            query = query.Where(q => q.Phone, filter.Phone);
            query = query.Where(q => q.Password, filter.Password);
            query = query.Where(q => q.DisplayName, filter.DisplayName);
            query = query.Where(q => q.Birthday, filter.Birthday);
            query = query.Where(q => q.Avatar, filter.Avatar);
            query = query.Where(q => q.CoverImage, filter.CoverImage);
            query = query.Where(q => q.SexId, filter.SexId);
            return query;
        }

        private async Task<IQueryable<AppUserDAO>> OrFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<AppUserDAO> initQuery = query.Where(q => false);
            foreach (AppUserFilter AppUserFilter in filter.OrFilter)
            {
                IQueryable<AppUserDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, AppUserFilter.Id);
                queryable = queryable.Where(q => q.Username, AppUserFilter.Username);
                queryable = queryable.Where(q => q.Email, AppUserFilter.Email);
                queryable = queryable.Where(q => q.Phone, AppUserFilter.Phone);
                queryable = queryable.Where(q => q.Password, AppUserFilter.Password);
                queryable = queryable.Where(q => q.DisplayName, AppUserFilter.DisplayName);
                queryable = queryable.Where(q => q.Birthday, AppUserFilter.Birthday);
                queryable = queryable.Where(q => q.Avatar, AppUserFilter.Avatar);
                queryable = queryable.Where(q => q.CoverImage, AppUserFilter.CoverImage);
                queryable = queryable.Where(q => q.SexId, AppUserFilter.SexId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<AppUserDAO> DynamicOrder(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderBy(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderBy(q => q.SexId);
                            break;
                        case AppUserOrder.Birthday:
                            query = query.OrderBy(q => q.Birthday);
                            break;
                        case AppUserOrder.Avatar:
                            query = query.OrderBy(q => q.Avatar);
                            break;
                        case AppUserOrder.CoverImage:
                            query = query.OrderBy(q => q.CoverImage);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case AppUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case AppUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case AppUserOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case AppUserOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case AppUserOrder.Password:
                            query = query.OrderByDescending(q => q.Password);
                            break;
                        case AppUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                        case AppUserOrder.Sex:
                            query = query.OrderByDescending(q => q.SexId);
                            break;
                        case AppUserOrder.Birthday:
                            query = query.OrderByDescending(q => q.Birthday);
                            break;
                        case AppUserOrder.Avatar:
                            query = query.OrderByDescending(q => q.Avatar);
                            break;
                        case AppUserOrder.CoverImage:
                            query = query.OrderByDescending(q => q.CoverImage);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<AppUser>> DynamicSelect(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            List<AppUser> AppUsers = await query.Select(q => new AppUser()
            {
                Id = filter.Selects.Contains(AppUserSelect.Id) ? q.Id : default(long),
                Username = filter.Selects.Contains(AppUserSelect.Username) ? q.Username : default(string),
                Email = filter.Selects.Contains(AppUserSelect.Email) ? q.Email : default(string),
                Phone = filter.Selects.Contains(AppUserSelect.Phone) ? q.Phone : default(string),
                Password = filter.Selects.Contains(AppUserSelect.Password) ? q.Password : default(string),
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                SexId = filter.Selects.Contains(AppUserSelect.Sex) ? q.SexId : default(long),
                Birthday = filter.Selects.Contains(AppUserSelect.Birthday) ? q.Birthday : default(DateTime?),
                Avatar = filter.Selects.Contains(AppUserSelect.Avatar) ? q.Avatar : default(string),
                CoverImage = filter.Selects.Contains(AppUserSelect.CoverImage) ? q.CoverImage : default(string),
                Sex = filter.Selects.Contains(AppUserSelect.Sex) && q.Sex != null ? new Sex
                {
                    Id = q.Sex.Id,
                    Code = q.Sex.Code,
                    Name = q.Sex.Name,
                } : null,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                DeletedAt = q.DeletedAt,
            }).ToListAsync();
            return AppUsers;
        }

        public async Task<int> CountAll(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = await DynamicFilter(AppUserDAOs, filter);
            return await AppUserDAOs.CountAsync();
        }

        public async Task<int> Count(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = await DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = await OrFilter(AppUserDAOs, filter);
            return await AppUserDAOs.CountAsync();
        }

        public async Task<List<AppUser>> List(AppUserFilter filter)
        {
            if (filter == null) return new List<AppUser>();
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser.AsNoTracking();
            AppUserDAOs = await DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = await OrFilter(AppUserDAOs, filter);
            AppUserDAOs = DynamicOrder(AppUserDAOs, filter);
            List<AppUser> AppUsers = await DynamicSelect(AppUserDAOs, filter);
            return AppUsers;
        }

        public async Task<List<AppUser>> List(List<long> Ids)
        {
            IdFilter IdFilter = new IdFilter { In = Ids };

            IQueryable<AppUserDAO> query = DataContext.AppUser.AsNoTracking();
            query = query.Where(q => q.Id, IdFilter);
            List<AppUser> AppUsers = await query.AsNoTracking()
            .Select(x => new AppUser()
            {
                RowId = x.RowId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                Phone = x.Phone,
                Password = x.Password,
                DisplayName = x.DisplayName,
                SexId = x.SexId,
                Birthday = x.Birthday,
                Avatar = x.Avatar,
                CoverImage = x.CoverImage,
                Sex = x.Sex == null ? null : new Sex
                {
                    Id = x.Sex.Id,
                    Code = x.Sex.Code,
                    Name = x.Sex.Name,
                },
            }).ToListAsync();
            
            var FavouriteMentorQuery = DataContext.FavouriteMentor.AsNoTracking()
                .Where(x => x.AppUserId, IdFilter);
            List<FavouriteMentor> FavouriteMentors = await FavouriteMentorQuery
                .Select(x => new FavouriteMentor
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MentorId = x.MentorId,
                }).ToListAsync();

            foreach(AppUser AppUser in AppUsers)
            {
                AppUser.FavouriteMentors = FavouriteMentors
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }

            var FavouriteMentorQuery = DataContext.FavouriteMentor.AsNoTracking()
                .Where(x => x.AppUserId, IdFilter);
            List<FavouriteMentor> FavouriteMentors = await FavouriteMentorQuery
                .Select(x => new FavouriteMentor
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MentorId = x.MentorId,
                }).ToListAsync();

            foreach(AppUser AppUser in AppUsers)
            {
                AppUser.FavouriteMentors = FavouriteMentors
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }

            var FavouriteNewsQuery = DataContext.FavouriteNews.AsNoTracking()
                .Where(x => x.AppUserId, IdFilter);
            List<FavouriteNews> FavouriteNews = await FavouriteNewsQuery
                .Select(x => new FavouriteNews
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    NewsId = x.NewsId,
                    News = new News
                    {
                        Id = x.News.Id,
                        CreatorId = x.News.CreatorId,
                        NewsContent = x.News.NewsContent,
                        LikeCounting = x.News.LikeCounting,
                        WatchCounting = x.News.WatchCounting,
                        NewsStatusId = x.News.NewsStatusId,
                    },
                }).ToListAsync();

            foreach(AppUser AppUser in AppUsers)
            {
                AppUser.FavouriteNews = FavouriteNews
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }

            var InformationQuery = DataContext.Information.AsNoTracking()
                .Where(x => x.AppUserId, IdFilter);
            List<Information> Information = await InformationQuery
                .Where(x => x.DeletedAt == null)
                .Select(x => new Information
                {
                    Id = x.Id,
                    InformationTypeId = x.InformationTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    StartAt = x.StartAt,
                    Role = x.Role,
                    Image = x.Image,
                    TopicId = x.TopicId,
                    UserId = x.UserId,
                    EndAt = x.EndAt,
                    InformationType = new InformationType
                    {
                        Id = x.InformationType.Id,
                        Name = x.InformationType.Name,
                        Code = x.InformationType.Code,
                    },
                    Topic = new Topic
                    {
                        Id = x.Topic.Id,
                        Title = x.Topic.Title,
                        Description = x.Topic.Description,
                        Cost = x.Topic.Cost,
                    },
                }).ToListAsync();

            foreach(AppUser AppUser in AppUsers)
            {
                AppUser.Information = Information
                    .Where(x => x.AppUserId == AppUser.Id)
                    .ToList();
            }


            return AppUsers;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await DataContext.AppUser.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new AppUser()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                Phone = x.Phone,
                Password = x.Password,
                DisplayName = x.DisplayName,
                SexId = x.SexId,
                Birthday = x.Birthday,
                Avatar = x.Avatar,
                CoverImage = x.CoverImage,
                Sex = x.Sex == null ? null : new Sex
                {
                    Id = x.Sex.Id,
                    Code = x.Sex.Code,
                    Name = x.Sex.Name,
                },
            }).FirstOrDefaultAsync();

            if (AppUser == null)
                return null;
            AppUser.FavouriteMentorMentors = await DataContext.FavouriteMentor.AsNoTracking()
                .Where(x => x.AppUserId == AppUser.Id)
                .Select(x => new FavouriteMentor
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MentorId = x.MentorId,
                }).ToListAsync();
            AppUser.FavouriteMentorUsers = await DataContext.FavouriteMentor.AsNoTracking()
                .Where(x => x.AppUserId == AppUser.Id)
                .Select(x => new FavouriteMentor
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    MentorId = x.MentorId,
                }).ToListAsync();
            AppUser.FavouriteNews = await DataContext.FavouriteNews.AsNoTracking()
                .Where(x => x.AppUserId == AppUser.Id)
                .Select(x => new FavouriteNews
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    NewsId = x.NewsId,
                    News = new News
                    {
                        Id = x.News.Id,
                        CreatorId = x.News.CreatorId,
                        NewsContent = x.News.NewsContent,
                        LikeCounting = x.News.LikeCounting,
                        WatchCounting = x.News.WatchCounting,
                        NewsStatusId = x.News.NewsStatusId,
                    },
                }).ToListAsync();
            AppUser.Information = await DataContext.Information.AsNoTracking()
                .Where(x => x.AppUserId == AppUser.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new Information
                {
                    Id = x.Id,
                    InformationTypeId = x.InformationTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    StartAt = x.StartAt,
                    Role = x.Role,
                    Image = x.Image,
                    TopicId = x.TopicId,
                    UserId = x.UserId,
                    EndAt = x.EndAt,
                    InformationType = new InformationType
                    {
                        Id = x.InformationType.Id,
                        Name = x.InformationType.Name,
                        Code = x.InformationType.Code,
                    },
                    Topic = new Topic
                    {
                        Id = x.Topic.Id,
                        Title = x.Topic.Title,
                        Description = x.Topic.Description,
                        Cost = x.Topic.Cost,
                    },
                }).ToListAsync();

            return AppUser;
        }
        
        public async Task<bool> Create(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = new AppUserDAO();
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.Avatar = AppUser.Avatar;
            AppUserDAO.CoverImage = AppUser.CoverImage;
            AppUserDAO.RowId = Guid.NewGuid();
            AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.AppUser.Add(AppUserDAO);
            await DataContext.SaveChangesAsync();
            AppUser.Id = AppUserDAO.Id;
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            AppUserDAO AppUserDAO = DataContext.AppUser
                .Where(x => x.Id == AppUser.Id)
                .FirstOrDefault();
            if (AppUserDAO == null)
                return false;
            AppUserDAO.Id = AppUser.Id;
            AppUserDAO.Username = AppUser.Username;
            AppUserDAO.Email = AppUser.Email;
            AppUserDAO.Phone = AppUser.Phone;
            AppUserDAO.Password = AppUser.Password;
            AppUserDAO.DisplayName = AppUser.DisplayName;
            AppUserDAO.SexId = AppUser.SexId;
            AppUserDAO.Birthday = AppUser.Birthday;
            AppUserDAO.Avatar = AppUser.Avatar;
            AppUserDAO.CoverImage = AppUser.CoverImage;
            AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(AppUser);
            return true;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            await DataContext.AppUser
                .Where(x => x.Id == AppUser.Id)
                .UpdateFromQueryAsync(x => new AppUserDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<AppUser> AppUsers)
        {
            IdFilter IdFilter = new IdFilter { In = AppUsers.Select(x => x.Id).ToList() };
            List<AppUserDAO> AppUserDAOs = new List<AppUserDAO>();
            List<AppUserDAO> DbAppUserDAOs = await DataContext.AppUser
                .Where(x => x.Id, IdFilter)
                .ToListAsync();
            foreach (AppUser AppUser in AppUsers)
            {
                AppUserDAO AppUserDAO = DbAppUserDAOs
                        .Where(x => x.Id == AppUser.Id)
                        .FirstOrDefault();
                if (AppUserDAO == null)
                {
                    AppUserDAO = new AppUserDAO();
                    AppUserDAO.CreatedAt = StaticParams.DateTimeNow;
                    AppUserDAO.RowId = Guid.NewGuid();
                    AppUser.RowId = AppUserDAO.RowId;
                }
                AppUserDAO.Username = AppUser.Username;
                AppUserDAO.Email = AppUser.Email;
                AppUserDAO.Phone = AppUser.Phone;
                AppUserDAO.Password = AppUser.Password;
                AppUserDAO.DisplayName = AppUser.DisplayName;
                AppUserDAO.SexId = AppUser.SexId;
                AppUserDAO.Birthday = AppUser.Birthday;
                AppUserDAO.Avatar = AppUser.Avatar;
                AppUserDAO.CoverImage = AppUser.CoverImage;
                AppUserDAO.UpdatedAt = StaticParams.DateTimeNow;
                AppUserDAOs.Add(AppUserDAO);
            }
            await DataContext.BulkMergeAsync(AppUserDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            List<long> Ids = AppUsers.Select(x => x.Id).ToList();
            await DataContext.AppUser
                .WhereBulkContains(Ids, x => x.Id)
                .UpdateFromQueryAsync(x => new AppUserDAO 
                { 
                    DeletedAt = StaticParams.DateTimeNow, 
                    UpdatedAt = StaticParams.DateTimeNow 
                });
            return true;
        }

        private async Task SaveReference(AppUser AppUser)
        {
            await DataContext.FavouriteMentor
                .Where(x => x.AppUserId == AppUser.Id)
                .DeleteFromQueryAsync();
            List<FavouriteMentorDAO> FavouriteMentorDAOs = new List<FavouriteMentorDAO>();
            if (AppUser.FavouriteMentorMentors != null)
            {
                foreach (FavouriteMentor FavouriteMentor in AppUser.FavouriteMentorMentors)
                {
                    FavouriteMentorDAO FavouriteMentorDAO = new FavouriteMentorDAO();
                    FavouriteMentorDAO.Id = FavouriteMentor.Id;
                    FavouriteMentorDAO.UserId = FavouriteMentor.UserId;
                    FavouriteMentorDAO.MentorId = FavouriteMentor.MentorId;
                    FavouriteMentorDAOs.Add(FavouriteMentorDAO);
                }
                await DataContext.FavouriteMentor.BulkMergeAsync(FavouriteMentorDAOs);
            }
            await DataContext.FavouriteMentor
                .Where(x => x.AppUserId == AppUser.Id)
                .DeleteFromQueryAsync();
            List<FavouriteMentorDAO> FavouriteMentorDAOs = new List<FavouriteMentorDAO>();
            if (AppUser.FavouriteMentorUsers != null)
            {
                foreach (FavouriteMentor FavouriteMentor in AppUser.FavouriteMentorUsers)
                {
                    FavouriteMentorDAO FavouriteMentorDAO = new FavouriteMentorDAO();
                    FavouriteMentorDAO.Id = FavouriteMentor.Id;
                    FavouriteMentorDAO.UserId = FavouriteMentor.UserId;
                    FavouriteMentorDAO.MentorId = FavouriteMentor.MentorId;
                    FavouriteMentorDAOs.Add(FavouriteMentorDAO);
                }
                await DataContext.FavouriteMentor.BulkMergeAsync(FavouriteMentorDAOs);
            }
            await DataContext.FavouriteNews
                .Where(x => x.AppUserId == AppUser.Id)
                .DeleteFromQueryAsync();
            List<FavouriteNewsDAO> FavouriteNewsDAOs = new List<FavouriteNewsDAO>();
            if (AppUser.FavouriteNews != null)
            {
                foreach (FavouriteNews FavouriteNews in AppUser.FavouriteNews)
                {
                    FavouriteNewsDAO FavouriteNewsDAO = new FavouriteNewsDAO();
                    FavouriteNewsDAO.Id = FavouriteNews.Id;
                    FavouriteNewsDAO.UserId = FavouriteNews.UserId;
                    FavouriteNewsDAO.NewsId = FavouriteNews.NewsId;
                    FavouriteNewsDAOs.Add(FavouriteNewsDAO);
                }
                await DataContext.FavouriteNews.BulkMergeAsync(FavouriteNewsDAOs);
            }
            List<InformationDAO> InformationDAOs = new List<InformationDAO>();
            List<InformationDAO> DbInformationDAOs = await DataContext.Information
                .Where(x => x.AppUserId == AppUser.Id)
                .ToListAsync();
            DbInformationDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (AppUser.Information != null)
            {
                foreach (Information Information in AppUser.Information)
                {
                    InformationDAO InformationDAO = DbInformationDAOs
                        .Where(x => x.Id == Information.Id).FirstOrDefault();
                    if (InformationDAO == null)
                    {
                        InformationDAO = new InformationDAO();
                        InformationDAO.CreatedAt = StaticParams.DateTimeNow;
                        InformationDAO.RowId = Guid.NewGuid();
                    }
                    InformationDAO.InformationTypeId = Information.InformationTypeId;
                    InformationDAO.Name = Information.Name;
                    InformationDAO.Description = Information.Description;
                    InformationDAO.StartAt = Information.StartAt;
                    InformationDAO.Role = Information.Role;
                    InformationDAO.Image = Information.Image;
                    InformationDAO.TopicId = Information.TopicId;
                    InformationDAO.UserId = Information.UserId;
                    InformationDAO.EndAt = Information.EndAt;
                    InformationDAO.UpdatedAt = StaticParams.DateTimeNow;
                    InformationDAO.DeletedAt = null;
                    InformationDAOs.Add(InformationDAO);
                }
                await DataContext.Information.BulkMergeAsync(InformationDAOs);
            }
        }
        
    }
}
