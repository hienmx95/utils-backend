using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Entities;
using Utils.Models;
using ZaloDotNetSDK;

namespace Utils.Service
{
    public interface IZaloService : IServiceScoped
    {
        Task<ZaloFollowerProfile> GetProfileOfFollower(string UserId);
        ZaloMessage SendTextMessageToUserId(string UserId, string Message);
        ZaloMessage SendImageMessageToUserIdByUrl(string UserId, string Message, string ImageUrl);
        ZaloMessage SendGifMessageToUserIdByAttachmentId(string UserId, string Message, string AttachmentId);
        ZaloMessage SendFileToUserId(string UserId, string FileAttachmentId);
        string TagFollower(string UserId, string TagName);
        string RemoveTagFromFollower(string UserId, string TagName);
        string DeleteTag(string TagName);
        string UploadImageForOfficialAccountAPI(MemoryStream File, string Filename);
        string UploadGifForOfficialAccountAPI(MemoryStream File, string Filename);
        string UploadFileForOfficialAccountAPI(MemoryStream File, string Filename);
        string GetListConversationWithUser(string UserId, int Skip, int Take);
    }
    public class ZaloService : IZaloService
    {
        private string Token;
        private ZaloClient ZaloClient;
        private DataContext DataContext;
        public ZaloService(DataContext DataContext)
        {
            this.DataContext = DataContext;
            Token = "L4NjAb2yR4fTHEaeMRHfI5TDwYHQgpWq5pRO4nwTAWjuDkinSeqN2mKBz2TQkWy83q2844dSLHSnPBe8EVz6CaD7b2ev-dOsD4UA9JlRQnLVM8bp1l9VTLz-Z6mPqmeBILp7CKFGHIzFKxuC8j0n8L5FpKOBx2rCVtttBHl72avzESGl3u4v64azmGyQeImHK26f5LgAR08DS8W7Ojz6Vp1_gqrXvtL1EH-aMLo_IsyCHyn0UUa-OsXFs7CXy1zCQsxGVpxkFt1TPDTiC_WFGM5L-Y1btdaOFCYtLLkZQqm";
            ZaloClient = new ZaloClient(Token);
        }

        public async Task<ZaloFollowerProfile> GetProfileOfFollower(string UserId)
        {
            ZaloUserDAO ZaloUserDAO = await DataContext.ZaloUser.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
            ZaloFollowerProfile FollowerProfile = new ZaloFollowerProfile();
            if (ZaloUserDAO == null)
            {
                DateTime StartDate = new DateTime(1970, 1, 1);
                JObject result = ZaloClient.getProfileOfFollower(UserId);

                FollowerProfile.Avatar = result["data"]["avatar"].ToString();
                FollowerProfile.Gender = long.Parse(result["data"]["user_gender"].ToString());
                long seconds = long.Parse(result["data"]["birth_date"].ToString());
                FollowerProfile.BirthDate = StartDate.AddSeconds(seconds);
                FollowerProfile.DisplayName = result["data"]["display_name"].ToString();
                FollowerProfile.UserId =result["data"]["user_id"].ToString();
                FollowerProfile.UserIdByApp = long.Parse(result["data"]["user_id_by_app"].ToString());
                ZaloUserDAO = new ZaloUserDAO
                {
                    UserId = UserId,
                    Username = FollowerProfile.DisplayName,
                };
                DataContext.ZaloUser.Add(ZaloUserDAO);
                await DataContext.SaveChangesAsync();
                FollowerProfile.Id = ZaloUserDAO.Id;
            }
            else
            {
                FollowerProfile.Id = ZaloUserDAO.Id;
                FollowerProfile.UserId = ZaloUserDAO.UserId;
                FollowerProfile.DisplayName = ZaloUserDAO.Username;
            }
            return FollowerProfile;
        }

        public ZaloMessage SendTextMessageToUserId(string UserId, string Message)
        {
            JObject result = ZaloClient.sendTextMessageToUserId(UserId, Message);
            ZaloMessage ZaloMessage = new ZaloMessage();
            ZaloMessage.message_id = result["data"]["message_id"].ToString();
            ZaloMessage.user_id = long.Parse(result["data"]["user_id"].ToString());
            ZaloMessage.content = Message;
            return ZaloMessage;
        }

        public ZaloMessage SendImageMessageToUserIdByUrl(string UserId, string Message, string ImageUrl)
        {
            JObject result = ZaloClient.sendImageMessageToUserIdByUrl(UserId, Message, ImageUrl);
            ZaloMessage ZaloMessage = new ZaloMessage();
            ZaloMessage.message_id = result["data"]["message_id"].ToString();
            ZaloMessage.user_id = long.Parse(result["data"]["user_id"].ToString());
            ZaloMessage.content = Message;
            ZaloMessage.image_url = ImageUrl;
            return ZaloMessage;
        }

        public ZaloMessage SendGifMessageToUserIdByAttachmentId(string UserId, string Message, string AttachmentId)
        {
            JObject result = ZaloClient.sendGifMessageToUserIdByAttachmentId(UserId, Message, AttachmentId);
            ZaloMessage ZaloMessage = new ZaloMessage();
            ZaloMessage.message_id = result["data"]["message_id"].ToString();
            ZaloMessage.user_id = long.Parse(result["data"]["user_id"].ToString());
            ZaloMessage.attachment_id = AttachmentId;
            return ZaloMessage;
        }

        public ZaloMessage SendFileToUserId(string UserId, string FileAttachmentId)
        {
            JObject result = ZaloClient.sendFileToUserId(UserId, FileAttachmentId);
            ZaloMessage ZaloMessage = new ZaloMessage();
            ZaloMessage.message_id = result["data"]["message_id"].ToString();
            ZaloMessage.user_id = long.Parse(result["data"]["user_id"].ToString());
            ZaloMessage.file_attachment_id = FileAttachmentId;
            return ZaloMessage;
        }

        public string TagFollower(string UserId, string TagName)
        {
            JObject result = ZaloClient.tagFollower(UserId, TagName);
            return null;
        }
        public string RemoveTagFromFollower(string UserId, string TagName)
        {
            JObject result = ZaloClient.removeTagFromFollower(UserId, TagName);
            return null;
        }

        public string DeleteTag(string TagName)
        {
            JObject result = ZaloClient.deleteTag(TagName);
            return null;
        }

        public string UploadImageForOfficialAccountAPI(MemoryStream File, string Filename)
        {
            string folder = $"./Temp/{Guid.NewGuid()}";
            string path = $"{folder}/{Filename}";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            System.IO.File.WriteAllBytes(path, File.ToArray());
            ZaloFile ZaloFile = new ZaloFile(path);
            JObject result = ZaloClient.uploadImageForOfficialAccountAPI(ZaloFile);
            System.IO.File.Delete(path);
            System.IO.Directory.Delete(folder);
            string attachment_id = result["data"]["attachment_id"].ToString();
            return attachment_id;
        }

        public string UploadGifForOfficialAccountAPI(MemoryStream File, string Filename)
        {
            string folder = $"./Temp/{Guid.NewGuid()}";
            string path = $"{folder}/{Filename}";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            System.IO.File.WriteAllBytes(path, File.ToArray());
            ZaloFile ZaloFile = new ZaloFile(path);
            JObject result = ZaloClient.uploadGifForOfficialAccountAPI(ZaloFile);
            System.IO.File.Delete(path);
            System.IO.Directory.Delete(folder);
            string attachment_id = result["data"]["attachment_id"].ToString();
            return attachment_id;
        }

        public string UploadFileForOfficialAccountAPI(MemoryStream File, string Filename)
        {
            string folder = $"./Temp/{Guid.NewGuid()}";
            string path = $"{folder}/{Filename}";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            System.IO.File.WriteAllBytes(path, File.ToArray());
            ZaloFile ZaloFile = new ZaloFile(path);
            if (Filename.ToLower().EndsWith("pdf"))
                ZaloFile.setMediaTypeHeader("application/pdf");
            else if (Filename.ToLower().EndsWith("doc"))
                ZaloFile.setMediaTypeHeader("application/msword");
            else if (Filename.ToLower().EndsWith("csv"))
                ZaloFile.setMediaTypeHeader("text/csv");
            else
            {
                System.IO.File.Delete(path);
                System.IO.Directory.Delete(folder);
                return null;
            }
            JObject result = ZaloClient.uploadFileForOfficialAccountAPI(ZaloFile);
            System.IO.File.Delete(path);
            System.IO.Directory.Delete(folder);
            string token = result["data"]["token"].ToString();
            return token;
        }

        public string GetListConversationWithUser(string UserId, int Skip, int Take)
        {
            long Id = long.Parse(UserId);
            JObject result = ZaloClient.getListConversationWithUser(Id, Skip, Take);
            return null;
        }
    }
}
