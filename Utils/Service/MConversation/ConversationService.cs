using Utils.Common;
using Utils.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Utils.Repositories;
using Utils.Entities;
using Utils.Enums;

namespace Utils.Services.MConversation
{
    public interface IConversationService :  IServiceScoped
    {
        Task<int> Count(ConversationFilter ConversationFilter);
        Task<List<Conversation>> List(ConversationFilter ConversationFilter);
        Task<Conversation> Get(long Id);
        Task<Conversation> Create(Conversation Conversation);
        Task<Conversation> Update(Conversation Conversation);
        Task<Conversation> Delete(Conversation Conversation);
    }

    public class ConversationService : IConversationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IConversationValidator ConversationValidator;

        public ConversationService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IConversationValidator ConversationValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ConversationValidator = ConversationValidator;
        }
        public async Task<int> Count(ConversationFilter ConversationFilter)
        {
            try
            {
                int result = await UOW.ConversationRepository.Count(ConversationFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return 0;
        }

        public async Task<List<Conversation>> List(ConversationFilter ConversationFilter)
        {
            try
            {
                List<Conversation> Conversations = await UOW.ConversationRepository.List(ConversationFilter);
                return Conversations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }
        
        public async Task<Conversation> Get(long Id)
        {
            Conversation Conversation = await UOW.ConversationRepository.Get(Id);
            if (Conversation == null)
                return null;
            return Conversation;
        }
        public async Task<Conversation> Create(Conversation Conversation)
        {
            if (!await ConversationValidator.Create(Conversation))
                return Conversation;

            try
            {
                await UOW.ConversationRepository.Create(Conversation);
                Conversation = await UOW.ConversationRepository.Get(Conversation.Id);
                await Logging.CreateAuditLog(Conversation, new { }, nameof(ConversationService));
                return Conversation;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }

        public async Task<Conversation> Update(Conversation Conversation)
        {
            if (!await ConversationValidator.Update(Conversation))
                return Conversation;
            try
            {
                var oldData = await UOW.ConversationRepository.Get(Conversation.Id);

                await UOW.ConversationRepository.Update(Conversation);

                Conversation = await UOW.ConversationRepository.Get(Conversation.Id);
                await Logging.CreateAuditLog(Conversation, oldData, nameof(ConversationService));
                return Conversation;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }

        public async Task<Conversation> Delete(Conversation Conversation)
        {
            if (!await ConversationValidator.Delete(Conversation))
                return Conversation;

            try
            {
                await UOW.ConversationRepository.Delete(Conversation);
                await Logging.CreateAuditLog(new { }, Conversation, nameof(ConversationService));
                return Conversation;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationService));
            }
            return null;
        }

    }
}
