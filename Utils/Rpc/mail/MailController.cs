using Utils.Entities;
using Utils.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using System.IO;
using Utils.Common;

namespace Utils.Rpc.mail
{
    public class MailRoute : Root
    {
        private const string Default = Rpc + Module;
        public const string Create = Default + "/mail/create";
        public const string Resend = Default + "/mail/resend";
    }
    public class MailController : RpcController
    {
        private readonly IMailService MailService;
        public MailController(IMailService MailService)
        {
            this.MailService = MailService;
        }

        [Route(MailRoute.Create), HttpPost]
        public async Task<bool> Create([FromForm] MailDTO MailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();

            var Mail = new Mail();
            Mail.Recipients = MailDTO.Recipients;
            Mail.Subject = MailDTO.Subject;
            Mail.Body = MailDTO.Content;
            Mail.Attachments = new List<Attachment>();
            foreach (var file in files)
            {
                MemoryStream stream = new MemoryStream();
                file.CopyTo(stream);

                Attachment Attachment = new Attachment
                {
                    Content = stream.ToArray(),
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                };
                Mail.Attachments.Add(Attachment);
            }
            await MailService.Create(Mail);

            return true;
        }

        [Route(MailRoute.Resend), HttpPost]
        public async Task<bool> ReSend()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            await MailService.ReSend();

            return true;
        }



    }
}
