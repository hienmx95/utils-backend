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

namespace Utils.Services.MConversationMessage
{
    public interface IConversationMessageService :  IServiceScoped
    {
        Task<int> Count(ConversationMessageFilter ConversationMessageFilter);
        Task<List<ConversationMessage>> List(ConversationMessageFilter ConversationMessageFilter);
        Task<ConversationMessage> Get(long Id);
        Task<ConversationMessage> Create(ConversationMessage ConversationMessage);
        Task<ConversationMessage> Update(ConversationMessage ConversationMessage);
        Task<ConversationMessage> Delete(ConversationMessage ConversationMessage);
    }

    public class ConversationMessageService : IConversationMessageService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IConversationMessageValidator ConversationMessageValidator;

        public ConversationMessageService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IConversationMessageValidator ConversationMessageValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ConversationMessageValidator = ConversationMessageValidator;
        }
        public async Task<int> Count(ConversationMessageFilter ConversationMessageFilter)
        {
            try
            {
                int result = await UOW.ConversationMessageRepository.Count(ConversationMessageFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return 0;
        }

        public async Task<List<ConversationMessage>> List(ConversationMessageFilter ConversationMessageFilter)
        {
            try
            {
                List<ConversationMessage> ConversationMessages = await UOW.ConversationMessageRepository.List(ConversationMessageFilter);
                return ConversationMessages;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }
        
        public async Task<ConversationMessage> Get(long Id)
        {
            ConversationMessage ConversationMessage = await UOW.ConversationMessageRepository.Get(Id);
            if (ConversationMessage == null)
                return null;
            return ConversationMessage;
        }
        public async Task<ConversationMessage> Create(ConversationMessage ConversationMessage)
        {
            if (!await ConversationMessageValidator.Create(ConversationMessage))
                return ConversationMessage;

            try
            {
                await UOW.ConversationMessageRepository.Create(ConversationMessage);
                ConversationMessage = await UOW.ConversationMessageRepository.Get(ConversationMessage.Id);
                await Logging.CreateAuditLog(ConversationMessage, new { }, nameof(ConversationMessageService));
                return ConversationMessage;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }

        public async Task<ConversationMessage> Update(ConversationMessage ConversationMessage)
        {
            if (!await ConversationMessageValidator.Update(ConversationMessage))
                return ConversationMessage;
            try
            {
                var oldData = await UOW.ConversationMessageRepository.Get(ConversationMessage.Id);

                await UOW.ConversationMessageRepository.Update(ConversationMessage);

                ConversationMessage = await UOW.ConversationMessageRepository.Get(ConversationMessage.Id);
                await Logging.CreateAuditLog(ConversationMessage, oldData, nameof(ConversationMessageService));
                return ConversationMessage;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }

        public async Task<ConversationMessage> Delete(ConversationMessage ConversationMessage)
        {
            if (!await ConversationMessageValidator.Delete(ConversationMessage))
                return ConversationMessage;

            try
            {
                await UOW.ConversationMessageRepository.Delete(ConversationMessage);
                await Logging.CreateAuditLog(new { }, ConversationMessage, nameof(ConversationMessageService));
                return ConversationMessage;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationMessageService));
            }
            return null;
        }
    }
}
