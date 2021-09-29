using Utils.Common;
using Utils.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Config;
using Utils.Entities;
using Utils.Repositories;
using System.Net;

namespace Utils.Service
{
    public interface IMailService : IServiceScoped
    {
        Task Create(Mail mail);
        Task Send(Mail mail);
        Task ReSend();
    }
    public class MailService : IMailService
    {
        private readonly EmailConfig emailConfig;
        private readonly IUOW UOW;
        private readonly IFileService FileService;
        public MailService(EmailConfig _emailConfig, IUOW UOW, IFileService FileService)
        {
            emailConfig = _emailConfig;
            this.UOW = UOW;
            this.FileService = FileService;
        }

        public async Task Create(Mail mail)
        {
            try
            {
                foreach(Attachment attachment in mail.Attachments)
                {
                    Entities.File File = new Entities.File
                    {
                        Path = $"/attachment/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{attachment.Extension}",
                        Content = attachment.Content,
                    };
                    File = await FileService.Create(File);
                    attachment.Url = File.Path;
                }
                await UOW.Begin();
                await UOW.MailRepository.Create(mail);
                await UOW.Commit();
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task Send(Mail mail)
        {
            var mailMessage = await CreateMail(mail);
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    try
                    {
                        await UOW.Begin();
                        await UOW.MailRepository.Create(mail);
                        await UOW.Commit();
                    }
                    catch (Exception ex)
                    {
                        await UOW.Rollback();
                    }
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public async Task ReSend()
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    List<Mail> mails = await UOW.MailRepository.List(new MailFilter
                    {
                        Skip = 0,
                        Take = 10,
                        RetryCount = new LongFilter { LessEqual = StaticParams.RetryCount },
                    });
                    await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
                    foreach (var mail in mails)
                    {
                        try
                        {
                            var mailMessage = await CreateMail(mail);
                            await client.SendAsync(mailMessage);
                            await UOW.MailRepository.Delete(mail.Id);
                        }
                        catch (Exception ex)
                        {
                            mail.RetryCount++;
                            mail.Error = ex.Message;
                            try
                            {
                                await UOW.MailRepository.Update(mail);
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception e)
                {

                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private async Task<MimeMessage> CreateMail(Mail mail)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(emailConfig.From));
            foreach (string recipient in mail.Recipients)
            {
                mailMessage.To.Add(new MailboxAddress(recipient));
            }
            mailMessage.Subject = mail.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = mail.Body };

            if (mail.Attachments != null)
            {
                foreach (var attachment in mail.Attachments)
                {
                    string url = attachment.Url;
                    url = url.Replace("/rpc/utils/file/download", "");

                    Entities.File file = (await FileService.List(new FileFilter
                    {
                        Path = new StringFilter { Equal = url },
                    })).FirstOrDefault();

                    if (file != null)
                    {
                        file = await FileService.Download(file.Id);
                        attachment.Content = file.Content;
                        bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(file.MimeType));
                    }
                }
            }
            mailMessage.Body = bodyBuilder.ToMessageBody();
            return mailMessage;
        }


    }
}
