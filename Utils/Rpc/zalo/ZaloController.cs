using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Models;
using Utils.Service;
using Utils.Entities;
using Utils.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Utils.Rpc.zalo
{
    public class ZaloRoute : Root
    {
        public const string Base = Rpc + Module + "/zalo";
        public const string WebHook = Base + "/web-hook";

    }
    public class ZaloController : ControllerBase
    {
        private DataContext DataContext;
        private IZaloService ZaloService;
        public ZaloController(DataContext DataContext, IZaloService ZaloService)
        {
            this.DataContext = DataContext;
            this.ZaloService = ZaloService;
        }

        [Route(ZaloRoute.WebHook), HttpPost]
        public async Task<IActionResult> Webhook([FromBody] ZaloWebHookPayloadDTO payload)
        {
            if (payload.event_name == ZaloEventEnum.user_send_text.Code)
                await Analyze_user_send_text(payload);
            if (payload.event_name == ZaloEventEnum.oa_send_text.Code)
                await Analyze_oa_send_text(payload);

            if (payload.event_name == ZaloEventEnum.user_send_image.Code)
                await Analyze_user_send_image(payload);
            if (payload.event_name == ZaloEventEnum.oa_send_image.Code)
                await Analyze_oa_send_image(payload);

            if (payload.event_name == ZaloEventEnum.oa_send_file.Code)
                await Analyze_oa_send_file(payload);
            if (payload.event_name == ZaloEventEnum.user_send_file.Code)
                await Analyze_user_send_file(payload);
            return Ok();
        }

        private async Task Analyze_user_send_text(ZaloWebHookPayloadDTO payload)
        {
            ZaloFollowerProfile Sender = await ZaloService.GetProfileOfFollower(payload.sender.id);
            ZaloFollowerProfile Recipient = await ZaloService.GetProfileOfFollower(payload.recipient.id);
            ZaloMessageDAO ZaloMessageDAO = new ZaloMessageDAO
            {
                MsgId = payload.message.msg_id,
                RecipientId = Recipient.Id,
                SenderId = Sender.Id,
                Text = payload.message.text,
                Timestamp = long.Parse(payload.timestamp),
            };
            DataContext.ZaloMessage.Add(ZaloMessageDAO);
            await DataContext.SaveChangesAsync();
        }

        private async Task Analyze_oa_send_text(ZaloWebHookPayloadDTO payload)
        {
            ZaloFollowerProfile Sender = await ZaloService.GetProfileOfFollower(payload.sender.id);
            ZaloFollowerProfile Recipient = await ZaloService.GetProfileOfFollower(payload.recipient.id);
            ZaloMessageDAO ZaloMessageDAO = new ZaloMessageDAO
            {
                MsgId = payload.message.msg_id,
                RecipientId = Recipient.Id,
                SenderId = Sender.Id,
                Text = payload.message.text,
                Timestamp = long.Parse(payload.timestamp),
            };
            DataContext.ZaloMessage.Add(ZaloMessageDAO);
            await DataContext.SaveChangesAsync();
        }

        private async Task Analyze_oa_send_image(ZaloWebHookPayloadDTO payload)
        {
            ZaloFollowerProfile Sender = await ZaloService.GetProfileOfFollower(payload.sender.id);
            ZaloFollowerProfile Recipient = await ZaloService.GetProfileOfFollower(payload.recipient.id);
            ZaloMessageDAO ZaloMessageDAO = new ZaloMessageDAO
            {
                MsgId = payload.message.msg_id,
                RecipientId = Recipient.Id,
                SenderId = Sender.Id,
                Text = payload.message.text,
                Timestamp = long.Parse(payload.timestamp),
            };
            DataContext.ZaloMessage.Add(ZaloMessageDAO);
            await DataContext.SaveChangesAsync();
            if (payload.message.attachments != null)
            {
                foreach(ZaloAttachmentDTO ZaloAttachmentDTO in payload.message.attachments)
                {
                    GenericEnum type = ZaloAttachmentTypeEnum.ZaloAttachmentTypeEnumList.Where(x => x.Code == ZaloAttachmentDTO.type).FirstOrDefault();

                    ZaloAttachmentDAO ZaloAttachmentDAO = new ZaloAttachmentDAO
                    {
                        ZaloMessageId = ZaloMessageDAO.Id,
                        ZaloAttachmentTypeId = type.Id,
                        Url = ZaloAttachmentDTO.payload.url,
                    };
                    DataContext.ZaloAttachment.Add(ZaloAttachmentDAO);
                }
                await DataContext.SaveChangesAsync();
            }    
        }

        private async Task Analyze_user_send_image(ZaloWebHookPayloadDTO payload)
        {
            ZaloFollowerProfile Sender = await ZaloService.GetProfileOfFollower(payload.sender.id);
            ZaloFollowerProfile Recipient = await ZaloService.GetProfileOfFollower(payload.recipient.id);
            ZaloMessageDAO ZaloMessageDAO = new ZaloMessageDAO
            {
                MsgId = payload.message.msg_id,
                RecipientId = Recipient.Id,
                SenderId = Sender.Id,
                Text = payload.message.text,
                Timestamp = long.Parse(payload.timestamp),
            };
            DataContext.ZaloMessage.Add(ZaloMessageDAO);
            await DataContext.SaveChangesAsync();
            if (payload.message.attachments != null)
            {
                foreach (ZaloAttachmentDTO ZaloAttachmentDTO in payload.message.attachments)
                {
                    GenericEnum type = ZaloAttachmentTypeEnum.ZaloAttachmentTypeEnumList.Where(x => x.Code == ZaloAttachmentDTO.type).FirstOrDefault();

                    ZaloAttachmentDAO ZaloAttachmentDAO = new ZaloAttachmentDAO
                    {
                        ZaloMessageId = ZaloMessageDAO.Id,
                        ZaloAttachmentTypeId = type.Id,
                        Url = ZaloAttachmentDTO.payload.url,
                        Thumbnail = ZaloAttachmentDTO.payload.thumbnail,
                    };
                    DataContext.ZaloAttachment.Add(ZaloAttachmentDAO);
                }
                await DataContext.SaveChangesAsync();
            }
        }

        private async Task Analyze_oa_send_file(ZaloWebHookPayloadDTO payload)
        {
            ZaloFollowerProfile Sender = await ZaloService.GetProfileOfFollower(payload.sender.id);
            ZaloFollowerProfile Recipient = await ZaloService.GetProfileOfFollower(payload.recipient.id);
            ZaloMessageDAO ZaloMessageDAO = new ZaloMessageDAO
            {
                MsgId = payload.message.msg_id,
                RecipientId = Recipient.Id,
                SenderId = Sender.Id,
                Text = payload.message.text,
                Timestamp = long.Parse(payload.timestamp),
            };
            DataContext.ZaloMessage.Add(ZaloMessageDAO);
            await DataContext.SaveChangesAsync();
            if (payload.message.attachments != null)
            {
                foreach (ZaloAttachmentDTO ZaloAttachmentDTO in payload.message.attachments)
                {
                    GenericEnum type = ZaloAttachmentTypeEnum.ZaloAttachmentTypeEnumList.Where(x => x.Code == ZaloAttachmentDTO.type).FirstOrDefault();

                    ZaloAttachmentDAO ZaloAttachmentDAO = new ZaloAttachmentDAO
                    {
                        ZaloMessageId = ZaloMessageDAO.Id,
                        ZaloAttachmentTypeId = type.Id,
                        Url = ZaloAttachmentDTO.payload.url,
                        Size = ZaloAttachmentDTO.payload.size,
                        Name = ZaloAttachmentDTO.payload.name,
                        Checksum = ZaloAttachmentDTO.payload.checksum,
                        Type = ZaloAttachmentDTO.payload.type,
                    };
                    DataContext.ZaloAttachment.Add(ZaloAttachmentDAO);
                }
                await DataContext.SaveChangesAsync();
            }
        }
        
        private async Task Analyze_user_send_file(ZaloWebHookPayloadDTO payload)
        {
            ZaloFollowerProfile Sender = await ZaloService.GetProfileOfFollower(payload.sender.id);
            ZaloFollowerProfile Recipient = await ZaloService.GetProfileOfFollower(payload.recipient.id);
            ZaloMessageDAO ZaloMessageDAO = new ZaloMessageDAO
            {
                MsgId = payload.message.msg_id,
                RecipientId = Recipient.Id,
                SenderId = Sender.Id,
                Text = payload.message.text,
                Timestamp = long.Parse(payload.timestamp),
            };
            DataContext.ZaloMessage.Add(ZaloMessageDAO);
            await DataContext.SaveChangesAsync();
            if (payload.message.attachments != null)
            {
                foreach (ZaloAttachmentDTO ZaloAttachmentDTO in payload.message.attachments)
                {
                    GenericEnum type = ZaloAttachmentTypeEnum.ZaloAttachmentTypeEnumList.Where(x => x.Code == ZaloAttachmentDTO.type).FirstOrDefault();

                    ZaloAttachmentDAO ZaloAttachmentDAO = new ZaloAttachmentDAO
                    {
                        ZaloMessageId = ZaloMessageDAO.Id,
                        ZaloAttachmentTypeId = type.Id,
                        Url = ZaloAttachmentDTO.payload.url,
                        Size = ZaloAttachmentDTO.payload.size,
                        Name = ZaloAttachmentDTO.payload.name,
                        Checksum = ZaloAttachmentDTO.payload.checksum,
                        Type = ZaloAttachmentDTO.payload.type,
                    };
                    DataContext.ZaloAttachment.Add(ZaloAttachmentDAO);
                }
                await DataContext.SaveChangesAsync();
            }
        }
    }
}
