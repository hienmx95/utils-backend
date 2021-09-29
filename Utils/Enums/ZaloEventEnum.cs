using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;

namespace Utils.Enums
{
    public static class ZaloEventEnum
    {
        public static GenericEnum add_user_to_tag = new GenericEnum { Id = 1, Code = "add_user_to_tag", Name = "add_user_to_tag" };
        public static GenericEnum follow = new GenericEnum { Id = 2, Code = "follow", Name = "follow" };
        public static GenericEnum oa_send_file = new GenericEnum { Id = 3, Code = "oa_send_file", Name = "oa_send_file" };
        public static GenericEnum oa_send_gif = new GenericEnum { Id = 4, Code = "oa_send_gif", Name = "oa_send_gif" };
        public static GenericEnum oa_send_image = new GenericEnum { Id = 5, Code = "oa_send_image", Name = "oa_send_image" };
        public static GenericEnum oa_send_list = new GenericEnum { Id = 6, Code = "oa_send_list", Name = "oa_send_list" };
        public static GenericEnum oa_send_text = new GenericEnum { Id = 7, Code = "oa_send_text", Name = "oa_send_text" };
        public static GenericEnum shop_has_order = new GenericEnum { Id = 8, Code = "shop_has_order", Name = "shop_has_order" };
        public static GenericEnum unfollow = new GenericEnum { Id = 9, Code = "unfollow", Name = "unfollow" };
        public static GenericEnum user_asking_product = new GenericEnum { Id = 10, Code = "user_asking_product", Name = "user_asking_product" };
        public static GenericEnum user_received_message = new GenericEnum { Id = 11, Code = "user_received_message", Name = "user_received_message" };
        public static GenericEnum user_seen_message = new GenericEnum { Id = 12, Code = "user_seen_message", Name = "user_seen_message" };
        public static GenericEnum user_send_audio = new GenericEnum { Id = 13, Code = "user_send_audio", Name = "user_send_audio" };
        public static GenericEnum user_send_file = new GenericEnum { Id = 14, Code = "user_send_file", Name = "useuser_send_filer_received_message" };
        public static GenericEnum user_send_gif = new GenericEnum { Id = 15, Code = "user_send_gif", Name = "user_send_gif" };
        public static GenericEnum user_send_image = new GenericEnum { Id = 16, Code = "user_send_image", Name = "user_send_image" };
        public static GenericEnum user_send_link = new GenericEnum { Id = 17, Code = "user_send_link", Name = "user_send_link" };
        public static GenericEnum user_send_location = new GenericEnum { Id = 18, Code = "user_send_location", Name = "user_send_location" };
        public static GenericEnum user_send_sticker = new GenericEnum { Id = 19, Code = "user_send_sticker", Name = "user_send_sticker" };
        public static GenericEnum user_send_text = new GenericEnum { Id = 20, Code = "user_send_text", Name = "user_send_text" };
        public static GenericEnum user_send_video = new GenericEnum { Id = 21, Code = "user_send_video", Name = "user_send_video" };
        public static GenericEnum user_submit_info = new GenericEnum { Id = 22, Code = "user_submit_info", Name = "user_submit_info" };
               
        public static GenericEnum remove_tag = new GenericEnum { Id = 100, Code = "remove_tag", Name = "remove_tag" };
        public static GenericEnum remove_user_from_tag = new GenericEnum { Id = 101, Code = "remove_user_from_tag", Name = "remove_user_from_tag" };
        public static GenericEnum user_authentication = new GenericEnum { Id = 102, Code = "user_authentication", Name = "user_authentication" };
    }
}
