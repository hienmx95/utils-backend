using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Entities;
using Utils.Enums;
using Utils.Hub;
using Utils.Repositories;

namespace Utils.Service
{
    public interface IChatService
    {
        Task<int> Count(ChatMessageFilter filter);
        Task<List<ChatMessage>> List(ChatMessageFilter filter);
        Task<ChatMessage> Get(long Id);
        Task<ChatMessage> Create(ChatMessage message);
        Task<bool> Delete(long Id);
    }
    public class ChatService : IChatService
    {
        private readonly IUOW UOW;
        private ICurrentContext CurrentContext;
        protected IHubContext<ChatHub> SignalR;

        public ChatService(IUOW UOW, IHubContext<ChatHub> SignalR, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.SignalR = SignalR;
            this.CurrentContext = CurrentContext;
        }

        public async Task<int> Count(ChatMessageFilter filter)
        {
            return await UOW.ChatMessageRepository.Count(filter);
        }

        public async Task<List<ChatMessage>> List(ChatMessageFilter filter)
        {
            return await UOW.ChatMessageRepository.List(filter);
        }

        public async Task<ChatMessage> Get(long Id)
        {
            if (Id == 0) return null;
            return await UOW.ChatMessageRepository.Get(Id);
        }

        public async Task<ChatMessage> Create(ChatMessage Message)
        {
            if (Message == null) return null;
            if (Message.RecipientId == default(Guid)) return null;
            if (Message.SenderId == default(Guid)) return null;
            if (Message.ChatMessageTypeId == default(long)) return null;
            if (Message.CreatedAt == default(DateTime)) return null;
            if (Message.UpdatedAt == default(DateTime)) return null;
            try
            {
                await UOW.Begin();
                await UOW.ChatMessageRepository.Create(Message);
                await UOW.Commit();

                await SignalR.Clients.User(Message.RecipientId.ToString()).SendAsync("ReceiveMessage", "", Message); // gửi message tới RecipientId, bao gồm Id người gửi, Id người nhận, nội dung, file, ngày tạo
                return await UOW.ChatMessageRepository.Get(Message.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                throw new MessageException(ex);
            }
        }

        public async Task<bool> Delete(long Id)
        {
            if (Id == 0) return false;
            try
            {
                await UOW.Begin();
                await UOW.ChatMessageRepository.Delete(Id);
                await UOW.Commit();
                return true;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                throw new MessageException(ex);
            }
        }
    }
}
