using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Enums;
using Utils.Models;

namespace Utils.Rpc
{
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        public SetupController(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        [HttpGet, Route("rpc/utils/setup/init")]
        public ActionResult Init()
        {
            InitEnum();
            return Ok();
        }

        [HttpGet, Route("rpc/utils/setup/init-enum")]
        public ActionResult InitEnum()
        {
            InitZaloAttachmentTypeEnum();
            InitConversationTypeEnum();
            return Ok();
        }

        public void InitZaloAttachmentTypeEnum()
        {
            List<ZaloAttachmentTypeDAO> ZaloAttachmentTypeEnumList = ZaloAttachmentTypeEnum.ZaloAttachmentTypeEnumList.Select(item => new ZaloAttachmentTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ZaloAttachmentType.BulkSynchronize(ZaloAttachmentTypeEnumList);
        }

        public void InitConversationTypeEnum()
        {
            List<ConversationTypeDAO> ConversationTypeEnumList = ConversationTypeEnum.ConversationTypeEnumList.Select(item => new ConversationTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ConversationType.BulkSynchronize(ConversationTypeEnumList);
        }
    }
}
