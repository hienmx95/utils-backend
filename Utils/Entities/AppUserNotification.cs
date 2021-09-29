using Utils.Common;
using HtmlAgilityPack;
using System;

namespace Utils.Entities
{
    public class AppUserNotification
    {
        public long Id { get; set; }
        public long SiteId { get; set; }
        public string TitleWeb { get; set; }
        public string ContentWeb { get; set; }
        public string TitleMobile
        {
            get
            {
                var pageDoc = new HtmlDocument();
                pageDoc.LoadHtml(TitleWeb);
                return pageDoc.DocumentNode.InnerText;
            }
        }
        public string ContentMobile
        {
            get
            {
                var pageDoc = new HtmlDocument();
                pageDoc.LoadHtml(ContentWeb);
                return pageDoc.DocumentNode.InnerText;
            }
        }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public bool Unread { get; set; }
        public string LinkWebsite { get; set; }
        public string LinkMobile { get; set; }
        public Guid RowId { get; set; }
        public DateTime Time { get; set; }
        public AppUser Sender { get; set; }
        public AppUser Recipient { get; set; }
    }
    public class UserNotificationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter SenderId { get; set; }
        public IdFilter RecipientId { get; set; }
        public IdFilter SiteId { get; set; }
        public bool? Unread { get; set; }
        public DateFilter Time { get; set; }
        public UserNotificationOrder OrderBy { get; set; }
    }

    public enum UserNotificationOrder
    {
        Id,
        Content,
        Time
    }
}
