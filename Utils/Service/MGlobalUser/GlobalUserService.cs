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

namespace Utils.Services.MGlobalUser
{
    public interface IGlobalUserService :  IServiceScoped
    {
        Task<int> Count(GlobalUserFilter GlobalUserFilter);
        Task<List<GlobalUser>> List(GlobalUserFilter GlobalUserFilter);
        Task<GlobalUser> Get(long Id);
    }

    public class GlobalUserService : IGlobalUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public GlobalUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(GlobalUserFilter GlobalUserFilter)
        {
            try
            {
                int result = await UOW.GlobalUserRepository.Count(GlobalUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(GlobalUserService));
            }
            return 0;
        }

        public async Task<List<GlobalUser>> List(GlobalUserFilter GlobalUserFilter)
        {
            try
            {
                List<GlobalUser> GlobalUsers = await UOW.GlobalUserRepository.List(GlobalUserFilter);
                return GlobalUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(GlobalUserService));
            }
            return null;
        }
        
        public async Task<GlobalUser> Get(long Id)
        {
            GlobalUser GlobalUser = await UOW.GlobalUserRepository.Get(Id);
            if (GlobalUser == null)
                return null;
            return GlobalUser;
        }
    }
}
