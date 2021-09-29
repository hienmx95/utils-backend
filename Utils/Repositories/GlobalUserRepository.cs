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
    public interface IGlobalUserRepository
    {
        Task<int> Count(GlobalUserFilter GlobalUserFilter);
        Task<List<GlobalUser>> List(GlobalUserFilter GlobalUserFilter);
        Task<GlobalUser> Get(long Id);
        Task<GlobalUser> Get(Guid RowId);
        Task<bool> BulkMerge(List<GlobalUser> GlobalUsers);
    }
    public class GlobalUserRepository : IGlobalUserRepository
    {
        private DataContext DataContext;
        public GlobalUserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<GlobalUserDAO> DynamicFilter(IQueryable<GlobalUserDAO> query, GlobalUserFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.RowId != null)
                query = query.Where(q => q.RowId, filter.RowId);
            if (filter.Username != null)
                query = query.Where(q => q.Username, filter.Username);
            if (filter.DisplayName != null)
                query = query.Where(q => q.DisplayName, filter.DisplayName);
            return query;
        }

        private IQueryable<GlobalUserDAO> DynamicOrder(IQueryable<GlobalUserDAO> query, GlobalUserFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case GlobalUserOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case GlobalUserOrder.RowId:
                            query = query.OrderBy(q => q.RowId);
                            break;
                        case GlobalUserOrder.Username:
                            query = query.OrderBy(q => q.Username);
                            break;
                        case GlobalUserOrder.DisplayName:
                            query = query.OrderBy(q => q.DisplayName);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case GlobalUserOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case GlobalUserOrder.RowId:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                        case GlobalUserOrder.Username:
                            query = query.OrderByDescending(q => q.Username);
                            break;
                        case GlobalUserOrder.DisplayName:
                            query = query.OrderByDescending(q => q.DisplayName);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<GlobalUser>> DynamicSelect(IQueryable<GlobalUserDAO> query, GlobalUserFilter filter)
        {
            List<GlobalUser> GlobalUsers = await query.Select(q => new GlobalUser()
            {
                Id = filter.Selects.Contains(GlobalUserSelect.Id) ? q.Id : default(long),
                RowId = filter.Selects.Contains(GlobalUserSelect.RowId) ? q.RowId : default(Guid),
                Username = filter.Selects.Contains(GlobalUserSelect.Username) ? q.Username : default(string),
                DisplayName = filter.Selects.Contains(GlobalUserSelect.DisplayName) ? q.DisplayName : default(string)
            }).ToListAsync();
            return GlobalUsers;
        }

        public async Task<int> Count(GlobalUserFilter filter)
        {
            IQueryable<GlobalUserDAO> GlobalUsers = DataContext.GlobalUser;
            GlobalUsers = DynamicFilter(GlobalUsers, filter);
            return await GlobalUsers.CountAsync();
        }

        public async Task<List<GlobalUser>> List(GlobalUserFilter filter)
        {
            if (filter == null) return new List<GlobalUser>();
            IQueryable<GlobalUserDAO> GlobalUserDAOs = DataContext.GlobalUser;
            GlobalUserDAOs = DynamicFilter(GlobalUserDAOs, filter);
            GlobalUserDAOs = DynamicOrder(GlobalUserDAOs, filter);
            List<GlobalUser> GlobalUsers = await DynamicSelect(GlobalUserDAOs, filter);
            return GlobalUsers;
        }

        public async Task<GlobalUser> Get(long Id)
        {
            GlobalUser GlobalUser = await DataContext.GlobalUser.Where(x => x.Id == Id)
                .Select(x => new GlobalUser()
                {
                    Id = x.Id,
                    RowId = x.RowId,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                }).FirstOrDefaultAsync();

            if (GlobalUser == null)
                return null;

            return GlobalUser;
        }

        public async Task<GlobalUser> Get(Guid RowId)
        {
            GlobalUser GlobalUser = await DataContext.GlobalUser.Where(x => x.RowId == RowId)
                .Select(x => new GlobalUser()
                {
                    Id = x.Id,
                    RowId = x.RowId,
                    Username = x.Username,
                    DisplayName = x.DisplayName,
                }).FirstOrDefaultAsync();

            if (GlobalUser == null)
                return null;

            return GlobalUser;
        }

        public async Task<bool> BulkMerge(List<GlobalUser> GlobalUsers)
        {
            List<Guid> RowIds = GlobalUsers.Select(x => x.RowId).ToList();
            List<GlobalUserDAO> GlobalUserDAOs = await DataContext.GlobalUser.AsNoTracking()
                .Where(x => RowIds.Contains(x.RowId))
                .ToListAsync();
            foreach(GlobalUser GlobalUser in GlobalUsers)
            {
                GlobalUserDAO GlobalUserDAO = GlobalUserDAOs
                    .Where(x => x.RowId == GlobalUser.RowId)
                    .FirstOrDefault();
                if(GlobalUserDAO == null)
                {
                    GlobalUserDAO = new GlobalUserDAO();
                    GlobalUserDAOs.Add(GlobalUserDAO);
                }
                GlobalUserDAO.Username = GlobalUser.Username;
                GlobalUserDAO.DisplayName = GlobalUser.DisplayName;
                GlobalUserDAO.RowId = GlobalUser.RowId;
            }
            await DataContext.BulkMergeAsync(GlobalUserDAOs);
            return true;
        }
    }
}
