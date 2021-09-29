using Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Repositories;

namespace Utils.Service
{
    public interface IAppUserService : IServiceScoped
    {
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<bool> CreateToken(FirebaseToken FirebaseToken);
    }
    public class AppUserService : IAppUserService
    {
        private IUOW UOW;
        public AppUserService(IUOW UOW)
        {
            this.UOW = UOW;
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            return await UOW.AppUserRepository.List(AppUserFilter);
        }

        public async Task<bool> CreateToken(FirebaseToken FirebaseToken)
        {
            return await UOW.AppUserRepository.CreateToken(FirebaseToken);
        }
    }
}
