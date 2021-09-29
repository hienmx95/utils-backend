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
    public interface IConversationRepository
    {
        Task<int> Count(ConversationFilter ConversationFilter);
        Task<List<Conversation>> List(ConversationFilter ConversationFilter);
        Task<List<Conversation>> List(List<long> Ids);
        Task<Conversation> Get(long Id);
        Task<bool> Create(Conversation Conversation);
        Task<bool> Update(Conversation Conversation);
        Task<bool> Delete(Conversation Conversation);
        Task<bool> BulkMerge(List<Conversation> Conversations);
        Task<bool> BulkDelete(List<Conversation> Conversations);
    }
    public class ConversationRepository : IConversationRepository
    {
        private DataContext DataContext;
        public ConversationRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ConversationDAO> DynamicFilter(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            query = query.Where(q => q.Id, filter.Id);
            query = query.Where(q => q.Name, filter.Name);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ConversationDAO> OrFilter(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ConversationDAO> initQuery = query.Where(q => false);
            foreach (ConversationFilter ConversationFilter in filter.OrFilter)
            {
                IQueryable<ConversationDAO> queryable = query;
                queryable = queryable.Where(q => q.Id, filter.Id);
                queryable = queryable.Where(q => q.Name, filter.Name);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ConversationDAO> DynamicOrder(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ConversationOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ConversationOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ConversationOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ConversationOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Conversation>> DynamicSelect(IQueryable<ConversationDAO> query, ConversationFilter filter)
        {
            List<Conversation> Conversations = await query.Select(q => new Conversation()
            {
                Id = filter.Selects.Contains(ConversationSelect.Id) ? q.Id : default(long),
                Name = filter.Selects.Contains(ConversationSelect.Name) ? q.Name : default(string),
            }).ToListAsync();
            return Conversations;
        }

        public async Task<int> Count(ConversationFilter filter)
        {
            IQueryable<ConversationDAO> Conversations = DataContext.Conversation.AsNoTracking();
            Conversations = DynamicFilter(Conversations, filter);
            return await Conversations.CountAsync();
        }

        public async Task<List<Conversation>> List(ConversationFilter filter)
        {
            if (filter == null) return new List<Conversation>();
            IQueryable<ConversationDAO> ConversationDAOs = DataContext.Conversation.AsNoTracking();
            ConversationDAOs = DynamicFilter(ConversationDAOs, filter);
            ConversationDAOs = DynamicOrder(ConversationDAOs, filter);
            List<Conversation> Conversations = await DynamicSelect(ConversationDAOs, filter);
            return Conversations;
        }

        public async Task<List<Conversation>> List(List<long> Ids)
        {
            List<Conversation> Conversations = await DataContext.Conversation.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new Conversation()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();
            
            List<ConversationParticipant> ConversationParticipants = await DataContext.ConversationParticipant.AsNoTracking()
                .Where(x => Ids.Contains(x.ConversationId))
                .Select(x => new ConversationParticipant
                {
                    Id = x.Id,
                    ConversationId = x.ConversationId,
                    GlobalUserId = x.GlobalUserId,
                    GlobalUser = new GlobalUser
                    {
                        Id = x.GlobalUser.Id,
                        Username = x.GlobalUser.Username,
                        DisplayName = x.GlobalUser.DisplayName,
                        RowId = x.GlobalUser.RowId,
                    },
                }).ToListAsync();
            foreach(Conversation Conversation in Conversations)
            {
                Conversation.ConversationParticipants = ConversationParticipants
                    .Where(x => x.ConversationId == Conversation.Id)
                    .ToList();
            }

            return Conversations;
        }

        public async Task<Conversation> Get(long Id)
        {
            Conversation Conversation = await DataContext.Conversation.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new Conversation()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Name = x.Name,
            }).FirstOrDefaultAsync();

            Conversation.ConversationParticipants = await DataContext.ConversationParticipant.AsNoTracking()
                .Where(x => x.ConversationId == Conversation.Id)
                .Select(x => new ConversationParticipant
                {
                    Id = x.Id,
                    ConversationId = x.ConversationId,
                    GlobalUserId = x.GlobalUserId,
                    GlobalUser = new GlobalUser
                    {
                        Id = x.GlobalUser.Id,
                        Username = x.GlobalUser.Username,
                        DisplayName = x.GlobalUser.DisplayName,
                        RowId = x.GlobalUser.RowId,
                    },
                }).ToListAsync();

            return Conversation;
        }
        public async Task<bool> Create(Conversation Conversation)
        {
            ConversationDAO ConversationDAO = new ConversationDAO();
            ConversationDAO.Id = Conversation.Id;
            ConversationDAO.Name = Conversation.Name;
            ConversationDAO.CreatedAt = StaticParams.DateTimeNow;
            ConversationDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Conversation.Add(ConversationDAO);
            await DataContext.SaveChangesAsync();
            Conversation.Id = ConversationDAO.Id;
            await SaveReference(Conversation);
            return true;
        }

        public async Task<bool> Update(Conversation Conversation)
        {
            ConversationDAO ConversationDAO = DataContext.Conversation.Where(x => x.Id == Conversation.Id).FirstOrDefault();
            if (ConversationDAO == null)
                return false;
            ConversationDAO.Id = Conversation.Id;
            ConversationDAO.Name = Conversation.Name;
            ConversationDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Conversation);
            return true;
        }

        public async Task<bool> Delete(Conversation Conversation)
        {
            await DataContext.Conversation.Where(x => x.Id == Conversation.Id).UpdateFromQueryAsync(x => new ConversationDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Conversation> Conversations)
        {
            List<ConversationDAO> ConversationDAOs = new List<ConversationDAO>();
            foreach (Conversation Conversation in Conversations)
            {
                ConversationDAO ConversationDAO = new ConversationDAO();
                ConversationDAO.Id = Conversation.Id;
                ConversationDAO.Name = Conversation.Name;
                ConversationDAO.CreatedAt = StaticParams.DateTimeNow;
                ConversationDAO.UpdatedAt = StaticParams.DateTimeNow;
                ConversationDAOs.Add(ConversationDAO);
            }
            await DataContext.BulkMergeAsync(ConversationDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Conversation> Conversations)
        {
            List<long> Ids = Conversations.Select(x => x.Id).ToList();
            await DataContext.Conversation
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ConversationDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Conversation Conversation)
        {
            await DataContext.ConversationParticipant
                .Where(x => x.ConversationId == Conversation.Id)
                .DeleteFromQueryAsync();
            List<ConversationParticipantDAO> ConversationParticipantDAOs = new List<ConversationParticipantDAO>();
            if (Conversation.ConversationParticipants != null)
            {
                foreach (ConversationParticipant ConversationParticipant in Conversation.ConversationParticipants)
                {
                    ConversationParticipantDAO ConversationParticipantDAO = new ConversationParticipantDAO();
                    ConversationParticipantDAO.Id = ConversationParticipant.Id;
                    ConversationParticipantDAO.ConversationId = Conversation.Id;
                    ConversationParticipantDAO.GlobalUserId = ConversationParticipant.GlobalUserId;
                    ConversationParticipantDAOs.Add(ConversationParticipantDAO);
                }
                await DataContext.ConversationParticipant.BulkMergeAsync(ConversationParticipantDAOs);
            }
        }
        
    }
}
