using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils.Entities;
using Utils.Models;
using Utils.Service;

namespace Utils.Rpc.system_log
{
    public class SystemLogRoute : Root
    {
        public const string Base = Rpc + Module + "/system-log";
        public const string Count = Base + "/count";
        public const string List = Base + "/list";
        public const string Get = Base + "/get";
        public const string Delete = Base + "/delete";
        public const string BulkDelete = Base + "/bulk-delete";
        public const string FilterListModuleName = Base + "/filter-list-module-name";
    }

    public class SystemLogController : RpcController
    {
        private ISystemLogService SystemLogService;
        public SystemLogController(ISystemLogService SystemLogService)
        {
            this.SystemLogService = SystemLogService;
        }

        [Route(SystemLogRoute.Count), HttpPost]
        public async Task<long> Count([FromBody] SystemLog_SystemLogFilterDTO SystemLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SystemLogFilter SystemLogFilter = ConvertFilterDTOToFilterEntity(SystemLogFilterDTO);
            return await SystemLogService.Count(SystemLogFilter);
        }

        [Route(SystemLogRoute.List), HttpPost]
        public async Task<List<SystemLog_SystemLogDTO>> List([FromBody] SystemLog_SystemLogFilterDTO SystemLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            SystemLogFilter SystemLogFilter = ConvertFilterDTOToFilterEntity(SystemLogFilterDTO);
            List<MongoSystemLog> MongoSystemLogs = await SystemLogService.List(SystemLogFilter);
            List<SystemLog_SystemLogDTO> SystemLog_SystemLogDTOs = MongoSystemLogs.Select(x => new SystemLog_SystemLogDTO(x)).ToList();
            return SystemLog_SystemLogDTOs;
        }

        [Route(SystemLogRoute.Get), HttpPost]
        public async Task<SystemLog_SystemLogDTO> Get([FromBody] SystemLog_SystemLogDTO SystemLog_SystemLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MongoSystemLog MongoSystemLog = await SystemLogService.Get(SystemLog_SystemLogDTO.Id);
            return new SystemLog_SystemLogDTO(MongoSystemLog);
        }

        [Route(SystemLogRoute.Delete), HttpPost]
        public async Task<ActionResult<SystemLog_SystemLogDTO>> Delete([FromBody] SystemLog_SystemLogDTO SystemLog_SystemLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MongoSystemLog MongoSystemLog = ConvertDTOToEntity(SystemLog_SystemLogDTO);
            MongoSystemLog = await SystemLogService.Delete(MongoSystemLog);
            SystemLog_SystemLogDTO = new SystemLog_SystemLogDTO(MongoSystemLog);
            if (MongoSystemLog.IsValidated)
                return SystemLog_SystemLogDTO;
            else
                return BadRequest(SystemLog_SystemLogDTO);
        }

        [Route(SystemLogRoute.BulkDelete), HttpPost]
        public async Task<bool> BulkDelete([FromBody] SystemLog_SystemLogFilterDTO SystemLog_SystemLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (SystemLog_SystemLogFilterDTO.HasValue)
            {
                SystemLogFilter SystemLogFilter = ConvertFilterDTOToFilterEntity(SystemLog_SystemLogFilterDTO);
                SystemLogFilter.Skip = 0;
                SystemLogFilter.Take = int.MaxValue;
                return await SystemLogService.BulkDelete(SystemLogFilter);
            }
            return false;
        }
        private SystemLogFilter ConvertFilterDTOToFilterEntity(SystemLog_SystemLogFilterDTO SystemLogFilterDTO)
        {
            SystemLogFilter SystemLogFilter = new SystemLogFilter()
            {
                Selects = SystemLogSelect.ALL,
                Skip = SystemLogFilterDTO.Skip,
                Take = SystemLogFilterDTO.Take,
                OrderType = SystemLogFilterDTO.OrderType,
                OrderBy = SystemLogFilterDTO.OrderBy,

                AppUserId = SystemLogFilterDTO.AppUserId,
                AppUser = SystemLogFilterDTO.AppUser,
                Exception = SystemLogFilterDTO.Exception,
                ModuleName = SystemLogFilterDTO.ModuleName,
                ClassName = SystemLogFilterDTO.ClassName,
                MethodName = SystemLogFilterDTO.MethodName,
                Time = SystemLogFilterDTO.Time,
            };
            return SystemLogFilter;
        }
        private MongoSystemLog ConvertDTOToEntity(SystemLog_SystemLogDTO SystemLog_SystemLogDTO)
        {
            MongoSystemLog MongoSystemLog = new MongoSystemLog()
            {
                Id = SystemLog_SystemLogDTO.Id,
                AppUserId = SystemLog_SystemLogDTO.AppUserId,
                AppUser = SystemLog_SystemLogDTO.AppUser,
                Exception = SystemLog_SystemLogDTO.Exception,
                ModuleName = SystemLog_SystemLogDTO.ModuleName,
                ClassName = SystemLog_SystemLogDTO.ClassName,
                MethodName = SystemLog_SystemLogDTO.MethodName,
                Time = SystemLog_SystemLogDTO.Time
            };
            return MongoSystemLog;
        }

        [Route(SystemLogRoute.FilterListModuleName), HttpPost]
        public async Task<List<string>> FilterListModuleName()
        {
            List<string> results = new List<string>
            {
                "AMS",
                "CMS_Export",
                "CRM",
                "DMS",
                "MDM",
                "Portal",
                "PPF",
                "Report.CRM",
                "Report.DMS",
                "Report.PPF",
                "SAMS",
                "Utils"
            };
            return results;
        }
    }
}