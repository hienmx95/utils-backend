using Utils.Common;
using Utils.Helpers;
using Utils.Entities;
using Utils.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Repositories
{
    public interface IConversationMessageRepository
    {
        Task<int> Count(ConversationMessageFilter ConversationMessageFilter);
        Task<List<ConversationMessage>> List(ConversationMessageFilter ConversationMessageFilter);
        Task<List<ConversationMessage>> List(List<long> Ids);
        Task<ConversationMessage> Get(long Id);
        Task<bool> Create(ConversationMessage ConversationMessage);
        Task<bool> Update(ConversationMessage ConversationMessage);
        Task<bool> Delete(ConversationMessage ConversationMessage);
    }
    public class ConversationMessageRepository : IConversationMessageRepository
    {
        private DataContext DataContext;
        public ConversationMessageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ConversationMessageDAO> DynamicFilter(IQueryable<ConversationMessageDAO> query, ConversationMessageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.ConversationId, filter.ConversationId);
            query = query.Where(q => q.ConversationTypeId, filter.ConversationTypeId);
            query = query.Where(q => q.GlobalUserId, filter.GlobalUserId);
            query = query.Where(q => q.Content, filter.Content);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ConversationMessageDAO> OrFilter(IQueryable<ConversationMessageDAO> query, ConversationMessageFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConversationMessageDAO> initQuery = query.Where(q => false);
            foreach (ConversationMessageFilter ConversationMessageFilter in filter.OrFilter)
            {
                IQueryable<ConversationMessageDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.ConversationId, filter.ConversationId);
                queryable = queryable.Where(q => q.ConversationTypeId, filter.ConversationTypeId);
                queryable = queryable.Where(q => q.GlobalUserId, filter.GlobalUserId);
                queryable = queryable.Where(q => q.Content, filter.Content);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConversationMessageDAO> DynamicOrder(IQueryable<ConversationMessageDAO> query, ConversationMessageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConversationMessageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConversationMessageOrder.Conversation:
                            query = query.OrderBy(q => q.ConversationId);
                            break;
                        case ConversationMessageOrder.ConversationType:
                            query = query.OrderBy(q => q.ConversationTypeId);
                            break;
                        case ConversationMessageOrder.GlobalUser:
                            query = query.OrderBy(q => q.GlobalUserId);
                            break;
                        case ConversationMessageOrder.Content:
                            query = query.OrderBy(q => q.Content);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConversationMessageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConversationMessageOrder.Conversation:
                            query = query.OrderByDescending(q => q.ConversationId);
                            break;
                        case ConversationMessageOrder.ConversationType:
                            query = query.OrderByDescending(q => q.ConversationTypeId);
                            break;
                        case ConversationMessageOrder.GlobalUser:
                            query = query.OrderByDescending(q => q.GlobalUserId);
                            break;
                        case ConversationMessageOrder.Content:
                            query = query.OrderByDescending(q => q.Content);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ConversationMessage>> DynamicSelect(IQueryable<ConversationMessageDAO> query, ConversationMessageFilter filter)
        {
            List<ConversationMessage> ConversationMessages = await query.Select(q => new ConversationMessage()
            {
                Id = filter.Selects.Contains(ConversationMessageSelect.Id) ? q.Id : default(long),
                ConversationId = filter.Selects.Contains(ConversationMessageSelect.Conversation) ? q.ConversationId : default(long),
                ConversationTypeId = filter.Selects.Contains(ConversationMessageSelect.ConversationType) ? q.ConversationTypeId : default(long),
                GlobalUserId = filter.Selects.Contains(ConversationMessageSelect.GlobalUser) ? q.GlobalUserId : default(long),
                Content = filter.Selects.Contains(ConversationMessageSelect.Content) ? q.Content : default(string),
                Conversation = filter.Selects.Contains(ConversationMessageSelect.Conversation) && q.Conversation != null ? new Conversation
                {
                    Id = q.Conversation.Id,
                    Name = q.Conversation.Name,
                } : null,
                ConversationType = filter.Selects.Contains(ConversationMessageSelect.ConversationType) && q.ConversationType != null ? new ConversationType
                {
                    Id = q.ConversationType.Id,
                    Code = q.ConversationType.Code,
                    Name = q.ConversationType.Name,
                } : null,
                GlobalUser = filter.Selects.Contains(ConversationMessageSelect.GlobalUser) && q.GlobalUser != null ? new GlobalUser
                {
                    Id = q.GlobalUser.Id,
                    Username = q.GlobalUser.Username,
                    DisplayName = q.GlobalUser.DisplayName,
                    RowId = q.GlobalUser.RowId,
                } : null,
            }).ToListAsync();
            return ConversationMessages;
        }

        public async Task<int> Count(ConversationMessageFilter filter)
        {
            IQueryable<ConversationMessageDAO> ConversationMessages = DataContext.ConversationMessage.AsNoTracking();
            ConversationMessages = DynamicFilter(ConversationMessages, filter);
            return await ConversationMessages.CountAsync();
        }

        public async Task<List<ConversationMessage>> List(ConversationMessageFilter filter)
        {
            if (filter == null) return new List<ConversationMessage>();
            IQueryable<ConversationMessageDAO> ConversationMessageDAOs = DataContext.ConversationMessage.AsNoTracking();
            ConversationMessageDAOs = DynamicFilter(ConversationMessageDAOs, filter);
            ConversationMessageDAOs = DynamicOrder(ConversationMessageDAOs, filter);
            List<ConversationMessage> ConversationMessages = await DynamicSelect(ConversationMessageDAOs, filter);
            return ConversationMessages;
        }

        public async Task<List<ConversationMessage>> List(List<long> Ids)
        {
            List<ConversationMessage> ConversationMessages = await DataContext.ConversationMessage.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new ConversationMessage()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                ConversationId = x.ConversationId,
                ConversationTypeId = x.ConversationTypeId,
                GlobalUserId = x.GlobalUserId,
                Content = x.Content,
                Conversation = x.Conversation == null ? null : new Conversation
                {
                    Id = x.Conversation.Id,
                    Name = x.Conversation.Name,
                },
                ConversationType = x.ConversationType == null ? null : new ConversationType
                {
                    Id = x.ConversationType.Id,
                    Code = x.ConversationType.Code,
                    Name = x.ConversationType.Name,
                },
                GlobalUser = x.GlobalUser == null ? null : new GlobalUser
                {
                    Id = x.GlobalUser.Id,
                    Username = x.GlobalUser.Username,
                    DisplayName = x.GlobalUser.DisplayName,
                    RowId = x.GlobalUser.RowId,
                },
            }).ToListAsync();
            

            return ConversationMessages;
        }

        public async Task<ConversationMessage> Get(long Id)
        {
            ConversationMessage ConversationMessage = await DataContext.ConversationMessage.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new ConversationMessage()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                ConversationId = x.ConversationId,
                ConversationTypeId = x.ConversationTypeId,
                GlobalUserId = x.GlobalUserId,
                Content = x.Content,
                Conversation = x.Conversation == null ? null : new Conversation
                {
                    Id = x.Conversation.Id,
                    Name = x.Conversation.Name,
                },
                ConversationType = x.ConversationType == null ? null : new ConversationType
                {
                    Id = x.ConversationType.Id,
                    Code = x.ConversationType.Code,
                    Name = x.ConversationType.Name,
                },
                GlobalUser = x.GlobalUser == null ? null : new GlobalUser
                {
                    Id = x.GlobalUser.Id,
                    Username = x.GlobalUser.Username,
                    DisplayName = x.GlobalUser.DisplayName,
                    RowId = x.GlobalUser.RowId,
                },
            }).FirstOrDefaultAsync();

            if (ConversationMessage == null)
                return null;

            return ConversationMessage;
        }
        public async Task<bool> Create(ConversationMessage ConversationMessage)
        {
            ConversationMessageDAO ConversationMessageDAO = new ConversationMessageDAO();
            ConversationMessageDAO.Id = ConversationMessage.Id;
            ConversationMessageDAO.ConversationId = ConversationMessage.ConversationId;
            ConversationMessageDAO.ConversationTypeId = ConversationMessage.ConversationTypeId;
            ConversationMessageDAO.GlobalUserId = ConversationMessage.GlobalUserId;
            ConversationMessageDAO.Content = ConversationMessage.Content;
            ConversationMessageDAO.CreatedAt = StaticParams.DateTimeNow;
            ConversationMessageDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ConversationMessage.Add(ConversationMessageDAO);
            await DataContext.SaveChangesAsync();
            ConversationMessage.Id = ConversationMessageDAO.Id;
            await SaveReference(ConversationMessage);
            return true;
        }

        public async Task<bool> Update(ConversationMessage ConversationMessage)
        {
            ConversationMessageDAO ConversationMessageDAO = DataContext.ConversationMessage.Where(x => x.Id == ConversationMessage.Id).FirstOrDefault();
            if (ConversationMessageDAO == null)
                return false;
            ConversationMessageDAO.Id = ConversationMessage.Id;
            ConversationMessageDAO.ConversationId = ConversationMessage.ConversationId;
            ConversationMessageDAO.ConversationTypeId = ConversationMessage.ConversationTypeId;
            ConversationMessageDAO.GlobalUserId = ConversationMessage.GlobalUserId;
            ConversationMessageDAO.Content = ConversationMessage.Content;
            ConversationMessageDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ConversationMessage);
            return true;
        }

        public async Task<bool> Delete(ConversationMessage ConversationMessage)
        {
            await DataContext.ConversationMessage.Where(x => x.Id == ConversationMessage.Id).UpdateFromQueryAsync(x => new ConversationMessageDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        private async Task SaveReference(ConversationMessage ConversationMessage)
        {
        }
        
    }
}
