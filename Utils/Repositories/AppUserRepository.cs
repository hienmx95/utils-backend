using Utils.Common;
using Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Models;

namespace Utils.Repositories
{
    public interface IAppUserRepository
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<bool> BulkMerge(List<AppUser> AppUsers);
        Task<bool> CreateToken(FirebaseToken FirebaseToken);
    }
    public class AppUserRepository : IAppUserRepository
    {
        private DataContext DataContext;
        public AppUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<AppUserDAO> DynamicFilter(IQueryable<AppUserDAO> query, AppUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Username != null)
                query = query.Where(q => q.Username, filter.Username);
            if (filter.DisplayName != null)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);

            return query;
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
                        case AppUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
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
                        case AppUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
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
                DisplayName = filter.Selects.Contains(AppUserSelect.DisplayName) ? q.DisplayName : default(string),
                Avatar = filter.Selects.Contains(AppUserSelect.Avatar) ? q.Avatar : default(string),
                RowId = q.RowId,
            }).ToListAsync();
            return AppUsers;
        }

        public async Task<int> Count(AppUserFilter filter)
        {
            IQueryable<AppUserDAO> AppUsers = DataContext.AppUser;
            AppUsers = DynamicFilter(AppUsers, filter);
            return await AppUsers.CountAsync();
        }

        public async Task<List<AppUser>> List(AppUserFilter filter)
        {
            if (filter == null) return new List<AppUser>();
            IQueryable<AppUserDAO> AppUserDAOs = DataContext.AppUser;
            AppUserDAOs = DynamicFilter(AppUserDAOs, filter);
            AppUserDAOs = DynamicOrder(AppUserDAOs, filter);
            List<AppUser> AppUsers = await DynamicSelect(AppUserDAOs, filter);
            return AppUsers;
        }

        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await DataContext.AppUser.Where(x => x.Id == Id).Select(x => new AppUser()
            {
                Id = x.Id,
                Username = x.Username,
                DisplayName = x.DisplayName,
                RowId = x.RowId,
            }).FirstOrDefaultAsync();

            if (AppUser == null)
                return null;

            return AppUser;
        }
        public async Task<bool> CreateToken(FirebaseToken FirebaseToken)
        {
            FirebaseTokenDAO FirebaseTokenDAO = await DataContext.FirebaseToken.Where(x => x.Token == FirebaseToken.Token).FirstOrDefaultAsync();
            if (FirebaseTokenDAO == null)
            {
                FirebaseTokenDAO = new FirebaseTokenDAO
                {
                    AppUserId = FirebaseToken.AppUserId,
                    Token = FirebaseToken.Token,
                    DeviceModel = FirebaseToken.DeviceModel,
                    OsName = FirebaseToken.OsName,
                    OsVersion = FirebaseToken.OsVersion,
                    UpdatedAt = StaticParams.DateTimeNow,
                };
                DataContext.FirebaseToken.Add(FirebaseTokenDAO);

            }
            else
            {
                FirebaseTokenDAO.AppUserId = FirebaseToken.AppUserId;
                FirebaseTokenDAO.DeviceModel = FirebaseToken.DeviceModel;
                FirebaseTokenDAO.OsName = FirebaseToken.OsName;
                FirebaseTokenDAO.OsVersion = FirebaseToken.OsVersion;
                FirebaseTokenDAO.UpdatedAt = StaticParams.DateTimeNow;
            }
            await DataContext.SaveChangesAsync();

            await DataContext.FirebaseToken.Where(x => x.UpdatedAt < StaticParams.DateTimeNow.AddDays(-7)).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<AppUser> AppUsers)
        {
            List<AppUserDAO> AppUserDAOs = new List<AppUserDAO>();
            foreach (AppUser AppUser in AppUsers)
            {
                AppUserDAO AppUserDAO = new AppUserDAO();
                AppUserDAO.Id = AppUser.Id;

                AppUserDAO.Address = AppUser.Address;
                AppUserDAO.Avatar = AppUser.Avatar;
                AppUserDAO.CreatedAt = AppUser.CreatedAt;
                AppUserDAO.UpdatedAt = AppUser.UpdatedAt;
                AppUserDAO.DeletedAt = AppUser.DeletedAt;
                AppUserDAO.Department = AppUser.Department;
                AppUserDAO.DisplayName = AppUser.DisplayName;
                AppUserDAO.Email = AppUser.Email;
                AppUserDAO.Id = AppUser.Id;
                AppUserDAO.OrganizationId = AppUser.OrganizationId;
                AppUserDAO.Phone = AppUser.Phone;
                AppUserDAO.PositionId = AppUser.PositionId;
                AppUserDAO.ProvinceId = AppUser.ProvinceId;
                AppUserDAO.RowId = AppUser.RowId;
                AppUserDAO.StatusId = AppUser.StatusId;
                AppUserDAO.Username = AppUser.Username;
                AppUserDAO.SexId = AppUser.SexId;
                AppUserDAO.Birthday = AppUser.Birthday;
                AppUserDAOs.Add(AppUserDAO);
            }
            await DataContext.BulkMergeAsync(AppUserDAOs);
            return true;
        }
    }
}
