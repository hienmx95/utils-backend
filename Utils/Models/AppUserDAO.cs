using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public partial class AppUserDAO
    {
        public AppUserDAO()
        {
            AppUserCommentMappings = new HashSet<AppUserCommentMappingDAO>();
            Comments = new HashSet<CommentDAO>();
            FirebaseTokens = new HashSet<FirebaseTokenDAO>();
            UserNotificationRecipients = new HashSet<UserNotificationDAO>();
            UserNotificationSenders = new HashSet<UserNotificationDAO>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Địa chỉ nhà
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Địa chỉ email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Số điện thoại liên hệ
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Tỉnh thành
        /// </summary>
        public long? ProvinceId { get; set; }
        public long? PositionId { get; set; }
        /// <summary>
        /// Phòng ban
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// Đơn vị công tác
        /// </summary>
        public long? OrganizationId { get; set; }
        public long? SexId { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public long StatusId { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>
        /// Ngày xoá
        /// </summary>
        public DateTime? DeletedAt { get; set; }
        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        public string Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// Trường để đồng bộ
        /// </summary>
        public Guid RowId { get; set; }

        public virtual ICollection<AppUserCommentMappingDAO> AppUserCommentMappings { get; set; }
        public virtual ICollection<CommentDAO> Comments { get; set; }
        public virtual ICollection<FirebaseTokenDAO> FirebaseTokens { get; set; }
        public virtual ICollection<UserNotificationDAO> UserNotificationRecipients { get; set; }
        public virtual ICollection<UserNotificationDAO> UserNotificationSenders { get; set; }
    }
}
