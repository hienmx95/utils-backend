using Utils.Common;
using Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Models;

namespace Utils.Repositories
{
    public interface IMailRepository
    {
        Task<int> Count(MailFilter filter);
        Task<List<Mail>> List(MailFilter filter);
        Task<Mail> Get(long Id);
        Task<bool> Create(Mail mail);
        Task<bool> Update(Mail mail);
        Task<bool> Delete(long Id);
        Task<bool> BulkMerge(List<Mail> Mails);
    }
    public class MailRepository : IMailRepository
    {
        private readonly DataContext DataContext;
        public MailRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<MailDAO> DynamicFilter(IQueryable<MailDAO> query, MailFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);

            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Body != null)
                query = query.Where(q => q.Body, filter.Body);
            if (filter.Recipients != null)
                query = query.Where(q => q.Recipients, filter.Recipients);
            if (filter.Subject != null)
                query = query.Where(q => q.Subject, filter.Subject);
            if (filter.RetryCount != null)
                query = query.Where(q => q.RetryCount, filter.RetryCount);
            return query;
        }

        public async Task<int> Count(MailFilter filter)
        {
            if (filter == null) return 0;
            IQueryable<MailDAO> mailDAOs = DataContext.Mail;
            mailDAOs = DynamicFilter(mailDAOs, filter);
            int count = await mailDAOs.CountAsync();
            return count;
        }

        public async Task<List<Mail>> List(MailFilter filter)
        {
            if (filter == null) return new List<Mail>();
            IQueryable<MailDAO> mailDAOs = DataContext.Mail;
            mailDAOs = DynamicFilter(mailDAOs, filter);

            var mails = await mailDAOs.ToListAsync();

            var mailIds = mails.Select(x => x.Id).ToList();
            var attachments = await DataContext.Attachment.Where(x => mailIds.Contains(x.MailId)).ToListAsync();

            List<Mail> Mails = new List<Mail>();
            foreach (var mail in mails)
            {
                Mail Mail = new Mail
                {
                    Id = mail.Id,
                    Body = mail.Body,
                    Recipients = new List<string>(),
                    Subject = mail.Subject,
                    RetryCount = mail.RetryCount,
                    Error = mail.Error,
                    Attachments = attachments
                        .Where(x => x.MailId == mail.Id)
                        .Select(x => new Attachment
                        {
                            Id = x.Id,
                            MailId = x.MailId,
                            FileName = x.FileName,
                            ContentType = x.ContentType,
                            Url = x.Url,
                        }).ToList(),
                };

                Mails.Add(Mail);
                try
                {
                    Mail.Recipients = JsonConvert.DeserializeObject<List<string>>(mail.Recipients);
                }
                catch (Exception ex)
                {

                }
            }
            return Mails;
        }
        public async Task<Mail> Get(long Id)
        {
            MailDAO mailDAO = await DataContext.Mail.Where(m => m.Id == Id).FirstOrDefaultAsync();
            List<AttachmentDAO> attachmentDAOs = await DataContext.Attachment.Where(a => a.MailId == Id).ToListAsync();
            List<string> recipients = JsonConvert.DeserializeObject<List<string>>(mailDAO.Recipients);
            Mail mail = new Mail
            {
                Id = mailDAO.Id,
                Body = mailDAO.Body,
                Subject = mailDAO.Subject,
                Recipients = recipients,
                RetryCount = mailDAO.RetryCount,
                Error = mailDAO.Error,
            };
            mail.Attachments = new List<Attachment>();
            foreach (AttachmentDAO attachmentDAO in attachmentDAOs)
            {
                Attachment attachment = new Attachment
                {
                    Id = attachmentDAO.Id,
                    Url = attachmentDAO.Url,
                    ContentType = attachmentDAO.ContentType,
                    FileName = attachmentDAO.FileName,
                };
                mail.Attachments.Add(attachment);
            }
            return mail;
        }
        public async Task<bool> Create(Mail mail)
        {
            MailDAO mailDAO = new MailDAO
            {
                Body = mail.Body,
                Subject = mail.Subject,
                RetryCount = 1,
                Recipients = JsonConvert.SerializeObject(mail.Recipients),
                CreatedAt = StaticParams.DateTimeNow,
            };
            DataContext.Mail.Add(mailDAO);
            await DataContext.SaveChangesAsync();
            List<AttachmentDAO> attachmentDAOs = mail.Attachments.Select(a => new AttachmentDAO
            {
                Url = a.Url,
                ContentType = a.ContentType,
                FileName = a.FileName,
                MailId = mailDAO.Id,
            }).ToList();
            DataContext.Attachment.AddRange(attachmentDAOs);
            await DataContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Update(Mail mail)
        {
            await DataContext.Mail.Where(m => m.Id == mail.Id).UpdateFromQueryAsync(n => new MailDAO
            {
                RetryCount = mail.RetryCount,
                Error = mail.Error
            });
            return true;
        }
        public async Task<bool> Delete(long Id)
        {
            await DataContext.Attachment.Where(a => a.MailId == Id).DeleteFromQueryAsync();
            await DataContext.Mail.Where(m => m.Id == Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<Mail> Mails)
        {
            Mails.ForEach(x => x.RowId = Guid.NewGuid());
            List<MailDAO> MailDAOs = Mails.Select(x => new MailDAO
            {
                Subject = x.Subject,
                Body = x.Body,
                Recipients = JsonConvert.SerializeObject(x.Recipients),
                RetryCount = 0,
                CreatedAt = StaticParams.DateTimeNow,
                RowId = x.RowId
            }).ToList();
            await DataContext.BulkMergeAsync(MailDAOs);

            List<AttachmentDAO> AttachmentDAOs = new List<AttachmentDAO>();
            foreach (var Mail in Mails)
            {
                
                if (Mail.Attachments != null && Mail.Attachments.Any())
                {
                    var MailId = MailDAOs.Where(x => x.RowId == Mail.RowId).Select(x => x.Id).FirstOrDefault();
                    var listAttchmentDAOs = new List<AttachmentDAO>();
                    listAttchmentDAOs = Mail.Attachments.Select(x => new AttachmentDAO
                    {
                        MailId = MailId,
                        ContentType = x.ContentType,
                        FileName = x.FileName,
                        Url = x.Url,
                    }).ToList();
                    AttachmentDAOs.AddRange(listAttchmentDAOs);
                }

            }
            return true;
        }
    }
}
