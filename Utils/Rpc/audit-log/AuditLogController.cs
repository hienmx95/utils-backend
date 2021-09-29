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
using OfficeOpenXml;
using Humanizer;

namespace Utils.Rpc.audit_log
{
    public class AuditLogRoute : Root
    {
        public const string Base = Rpc + Module + "/audit-log";
        public const string Count = Base + "/count";
        public const string List = Base + "/list";
        public const string Get = Base + "/get";
        public const string Delete = Base + "/delete";
        public const string BulkDelete = Base + "/bulk-delete";
        public const string Export = Base + "/export";
        public const string FilterListModuleName = Base + "/filter-list-module-name";
    }

    public class AuditLogController : RpcController
    {
        private IAuditLogService AuditLogService;
        public AuditLogController(IAuditLogService AuditLogService)
        {
            this.AuditLogService = AuditLogService;
        }

        [Route(AuditLogRoute.Count), HttpPost]
        public async Task<long> Count([FromBody] AuditLog_AuditLogFilterDTO AuditLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AuditLogFilter AuditLogFilter = ConvertFilterDTOToFilterEntity(AuditLogFilterDTO);
            return await AuditLogService.Count(AuditLogFilter);
        }

        [Route(AuditLogRoute.List), HttpPost]
        public async Task<List<AuditLog_AuditLogDTO>> List([FromBody] AuditLog_AuditLogFilterDTO AuditLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AuditLogFilter AuditLogFilter = ConvertFilterDTOToFilterEntity(AuditLogFilterDTO);
            List<MongoAuditLog> MongoAuditLogs = await AuditLogService.List(AuditLogFilter);
            List<AuditLog_AuditLogDTO> AuditLog_AuditLogDTOs = MongoAuditLogs.Select(x => new AuditLog_AuditLogDTO(x)).ToList();
            return AuditLog_AuditLogDTOs;
        }

        [Route(AuditLogRoute.Get), HttpPost]
        public async Task<AuditLog_AuditLogDTO> Get([FromBody] AuditLog_AuditLogDTO AuditLog_AuditLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MongoAuditLog MongoAuditLog = await AuditLogService.Get(AuditLog_AuditLogDTO.Id);
            return new AuditLog_AuditLogDTO(MongoAuditLog);
        }

        [Route(AuditLogRoute.Delete), HttpPost]
        public async Task<ActionResult<AuditLog_AuditLogDTO>> Delete([FromBody] AuditLog_AuditLogDTO AuditLog_AuditLogDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MongoAuditLog MongoAuditLog = ConvertDTOToEntity(AuditLog_AuditLogDTO);
            MongoAuditLog = await AuditLogService.Delete(MongoAuditLog);
            AuditLog_AuditLogDTO = new AuditLog_AuditLogDTO(MongoAuditLog);
            if (MongoAuditLog.IsValidated)
                return AuditLog_AuditLogDTO;
            else
                return BadRequest(AuditLog_AuditLogDTO);
        }

        [Route(AuditLogRoute.BulkDelete), HttpPost]
        public async Task<bool> BulkDelete([FromBody] AuditLog_AuditLogFilterDTO AuditLog_AuditLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (AuditLog_AuditLogFilterDTO.HasValue)
            {
                AuditLogFilter AuditLogFilter = ConvertFilterDTOToFilterEntity(AuditLog_AuditLogFilterDTO);
                AuditLogFilter.Skip = 0;
                AuditLogFilter.Take = int.MaxValue;
                return await AuditLogService.BulkDelete(AuditLogFilter);
            }
            return false;
        }

        [Route(AuditLogRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] AuditLog_AuditLogFilterDTO AuditLogFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                var AuditLogFilter = ConvertFilterDTOToFilterEntity(AuditLogFilterDTO);
                AuditLogFilter.Skip = 0;
                AuditLogFilter.Take = int.MaxValue;
                AuditLogFilter.Selects = AuditLogSelect.ALL;
                List<MongoAuditLog> AuditLogs = await AuditLogService.List(AuditLogFilter);

                var AuditLogHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "AppUserId",
                        "Người thay đổi",
                        "Dữ liệu trước",
                        "Dữ liệu sau",
                        "Tên phân hệ",
                        "Tên lớp",
                        "Tên phương thức",
                        "Thời gian thay đổi"
                    }
                };
                List<object[]> AuditLogData = new List<object[]>();
                for (int i = 0; i < AuditLogs.Count; i++)
                {
                    var AuditLog = AuditLogs[i];
                    AuditLogData.Add(new Object[]
                    {
                        AuditLog.Id,
                        AuditLog.AppUserId,
                        AuditLog.AppUser,
                        AuditLog.OldData,
                        AuditLog.NewData,
                        AuditLog.ModuleName,
                        AuditLog.ClassName,
                        AuditLog.MethodName,
                        AuditLog.Time.AddHours(7).ToString("HH:mm:ss dd-MM-yyyy")
                    });
                }
                excel.GenerateWorksheet("AuditLog", AuditLogHeaders, AuditLogData);
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "AuditLog.xlsx");
        }
        private MongoAuditLog ConvertDTOToEntity(AuditLog_AuditLogDTO AuditLog_AuditLogDTO)
        {
            MongoAuditLog MongoAuditLog = new MongoAuditLog()
            {
                Id = AuditLog_AuditLogDTO.Id,
                AppUserId = AuditLog_AuditLogDTO.AppUserId,
                AppUser = AuditLog_AuditLogDTO.AppUser,
                OldData = AuditLog_AuditLogDTO.OldData,
                NewData = AuditLog_AuditLogDTO.NewData,
                ModuleName = AuditLog_AuditLogDTO.ModuleName,
                ClassName = AuditLog_AuditLogDTO.ClassName,
                MethodName = AuditLog_AuditLogDTO.MethodName,
                Time = AuditLog_AuditLogDTO.Time
            };
            return MongoAuditLog;
        }

        private AuditLogFilter ConvertFilterDTOToFilterEntity(AuditLog_AuditLogFilterDTO AuditLogFilterDTO)
        {
            AuditLogFilter AuditLogFilter = new AuditLogFilter()
            {
                Selects = AuditLogSelect.Id | AuditLogSelect.AppUserId | AuditLogSelect.AppUser | AuditLogSelect.ModuleName | AuditLogSelect.ClassName 
                | AuditLogSelect.MethodName | AuditLogSelect.Time,
                Skip = AuditLogFilterDTO.Skip,
                Take = AuditLogFilterDTO.Take,
                OrderType = AuditLogFilterDTO.OrderType,
                OrderBy = AuditLogFilterDTO.OrderBy,

                AppUserId = AuditLogFilterDTO.AppUserId,
                AppUser = AuditLogFilterDTO.AppUser,
                ModuleName = AuditLogFilterDTO.ModuleName,
                ClassName = AuditLogFilterDTO.ClassName,
                MethodName = AuditLogFilterDTO.MethodName,
                Time = AuditLogFilterDTO.Time,
            };
            return AuditLogFilter;
        }

        [Route(AuditLogRoute.FilterListModuleName), HttpPost]
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