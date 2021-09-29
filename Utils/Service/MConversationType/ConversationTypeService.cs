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

namespace Utils.Services.MConversationType
{
    public interface IConversationTypeService :  IServiceScoped
    {
        Task<int> Count(ConversationTypeFilter ConversationTypeFilter);
        Task<List<ConversationType>> List(ConversationTypeFilter ConversationTypeFilter);
        Task<ConversationType> Get(long Id);
    }

    public class ConversationTypeService : IConversationTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ConversationTypeService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ConversationTypeFilter ConversationTypeFilter)
        {
            try
            {
                int result = await UOW.ConversationTypeRepository.Count(ConversationTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationTypeService));
            }
            return 0;
        }

        public async Task<List<ConversationType>> List(ConversationTypeFilter ConversationTypeFilter)
        {
            try
            {
                List<ConversationType> ConversationTypes = await UOW.ConversationTypeRepository.List(ConversationTypeFilter);
                return ConversationTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ConversationTypeService));
            }
            return null;
        }
        
        public async Task<ConversationType> Get(long Id)
        {
            ConversationType ConversationType = await UOW.ConversationTypeRepository.Get(Id);
            if (ConversationType == null)
                return null;
            return ConversationType;
        }
    }
}
