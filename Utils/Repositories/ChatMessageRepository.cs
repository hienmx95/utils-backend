using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Models;
using Utils.Common;

namespace Utils.Repositories
{
    public interface IChatMessageRepository
    {
        Task<int> Count(ChatMessageFilter filter);
        Task<List<ChatMessage>> List(ChatMessageFilter filter);
        Task<ChatMessage> Get(long Id);
        Task<bool> Create(ChatMessage message);
        Task<bool> Delete(long Id);
    }
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly DataContext DataContext;
        public ChatMessageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ChatMessageDAO> DynamicFilter(IQueryable<ChatMessageDAO> query, ChatMessageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.SenderId != null)
                query = query.Where(q => q.SenderId, filter.SenderId);
            if (filter.RecipientId != null)
                query = query.Where(q => q.RecipientId, filter.RecipientId);
            if (filter.CreatedAt != null)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            return query;
        }

        private IQueryable<ChatMessageDAO> DynamicOrder(IQueryable<ChatMessageDAO> query, ChatMessageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ChatMessageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ChatMessageOrder.CreatedAt:
                            query = query.OrderBy(q => q.CreatedAt);
                            break;
                        case ChatMessageOrder.UpdatedAt:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                        default:
                            query = query.OrderBy(q => q.Id);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ChatMessageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ChatMessageOrder.CreatedAt:
                            query = query.OrderByDescending(q => q.CreatedAt);
                            break;
                        case ChatMessageOrder.UpdatedAt:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                        default:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                    }
                    break;
                default:
                    query = query.OrderBy(q => q.Id);
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        public async Task<int> Count(ChatMessageFilter filter)
        {
            if (filter == null) return 0;
            IQueryable<ChatMessageDAO> ChatMessageDAOs = DataContext.ChatMessage;
            ChatMessageDAOs = DynamicFilter(ChatMessageDAOs, filter);
            return await ChatMessageDAOs.CountAsync();
        }

        public async Task<List<ChatMessage>> List(ChatMessageFilter filter)
        {
            if (filter == null) return new List<ChatMessage>();
            IQueryable<ChatMessageDAO> ChatMessageDAOs = DataContext.ChatMessage;
            ChatMessageDAOs = DynamicFilter(ChatMessageDAOs, filter);
            ChatMessageDAOs = DynamicOrder(ChatMessageDAOs, filter);
            List<ChatMessage> ChatMessages = ChatMessageDAOs.Select(x => new ChatMessage
            {
                Id = x.Id,
                RecipientId = x.RecipientId,
                SenderId = x.SenderId,
                Content = x.Content,
                FileId = x.FileId,
                ChatMessageTypeId = x.ChatMessageTypeId,
                ChatMessageType = new ChatMessageType
                {
                    Id = x.ChatMessageType.Id,
                    Code = x.ChatMessageType.Code,
                    Name = x.ChatMessageType.Name,
                },
                File = x.FileId == null ? null : new File
                {
                    Id = x.File.Id,
                    Name = x.File.Name,
                    Path = x.File.Path,
                    RowId = x.File.RowId,
                    CreatedAt = x.File.CreatedAt,
                }
            }).ToList();
            return ChatMessages;
        }

        public async Task<ChatMessage> Get(long Id)
        {
            ChatMessage ChatMessage = await DataContext.ChatMessage.AsNoTracking()
                .Where(x => x.Id == Id)
                .Select(x => new ChatMessage
                {
                    Id = x.Id,
                    RecipientId = x.RecipientId,
                    SenderId = x.SenderId,
                    Content = x.Content,
                    FileId = x.FileId,
                    ChatMessageTypeId = x.ChatMessageTypeId,
                    ChatMessageType = new ChatMessageType
                    {
                        Id = x.ChatMessageType.Id,
                        Code = x.ChatMessageType.Code,
                        Name = x.ChatMessageType.Name,
                    },
                    File = x.FileId == null ? null : new File
                    {
                        Id = x.File.Id,
                        Name = x.File.Name,
                        Path = x.File.Path,
                        RowId = x.File.RowId,
                        CreatedAt = x.File.CreatedAt,
                    }
                }).FirstOrDefaultAsync();
            return ChatMessage;
        }

        public async Task<bool> Create(ChatMessage ChatMessage)
        {
            ChatMessageDAO ChatMessageDAO = new ChatMessageDAO
            {
                RecipientId = ChatMessage.RecipientId,
                SenderId = ChatMessage.SenderId,
                ChatMessageTypeId = ChatMessage.ChatMessageTypeId,
                CreatedAt = ChatMessage.CreatedAt,
                UpdatedAt = ChatMessage.UpdatedAt,
                FileId = ChatMessage.FileId,
                Content = ChatMessage.Content,
            };
            DataContext.ChatMessage.Add(ChatMessageDAO);
            await DataContext.SaveChangesAsync();
            ChatMessage.Id = ChatMessageDAO.Id;
            return true;
        }

        public async Task<bool> Delete(long Id)
        {
            await DataContext.ChatMessage.Where(n => n.Id == Id).DeleteFromQueryAsync();
            return true;
        }
    }
}
