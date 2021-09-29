using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils.Service;
using Utils.Entities;

namespace Utils.Rpc.user
{
    public class AppUserRoute : Root
    {
        public const string Base = Rpc + Module + "/app-user";
        public const string CreateToken = Base + "/create-token";
    }
    public class AppUserController : RpcController
    {
        private IAppUserService AppUserService;
        private ICurrentContext CurrentContext;
        public AppUserController(IAppUserService AppUserService,ICurrentContext CurrentContext)
        {
            this.AppUserService = AppUserService;
            this.CurrentContext = CurrentContext;
        }
        [Route(AppUserRoute.CreateToken), HttpPost]
        public async Task<ActionResult<bool>> CreateToken([FromBody] AppUser_FirebaseTokenDTO UserNotification_FirebaseTokenDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(UserNotification_FirebaseTokenDTO);

            FirebaseToken FirebaseToken = new FirebaseToken
            {
                AppUserId = CurrentContext.UserId,
                Token = UserNotification_FirebaseTokenDTO.Token,
                DeviceModel = UserNotification_FirebaseTokenDTO.DeviceModel,
                OsName = UserNotification_FirebaseTokenDTO.OsName,
                OsVersion = UserNotification_FirebaseTokenDTO.OsVersion,
            };
            return await AppUserService.CreateToken(FirebaseToken);
        }
    }
}
