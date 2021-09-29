using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils.Service;
using Utils.Entities;
using System.Threading.Tasks;
using Utils.Models;
using System.Linq;

namespace Utils.Tests
{
    [TestFixture]
    public class ZaloServiceTests : BaseTests
    {
        public ZaloService ZaloService;
        [SetUp]
        public void Setup()
        {
            Init();
            ZaloService = new ZaloService(DataContext);
        }

        [Test]
        public async Task GetProfileOfFollower_NewUser()
        {
            // Arrange
            string UserId = "9049318283933813530";
            // Act
            {
                ZaloFollowerProfile FollowerProfile = await ZaloService.GetProfileOfFollower(UserId);
            }
            // Assert
            {
                ZaloUserDAO ZaloUserDAO = DataContext.ZaloUser.Where(x => x.UserId == UserId).FirstOrDefault();
                Assert.IsTrue(ZaloUserDAO != null);
            }
        }

        [Test]
        public async Task GetProfileOfFollower_OAUser()
        {
            // Arrange
            string UserId = "939273697747818078";
            {
                ZaloUserDAO ZaloUserDAO = new ZaloUserDAO { UserId = UserId, Username = "OAUser"};
                DataContext.ZaloUser.Add(ZaloUserDAO);
                DataContext.SaveChanges();
            }
            // Act
            {
                ZaloFollowerProfile FollowerProfile = await ZaloService.GetProfileOfFollower(UserId);
            }
            // Assert
            {
                ZaloUserDAO ZaloUserDAO = DataContext.ZaloUser.Where(x => x.UserId == UserId).FirstOrDefault();
                Assert.IsTrue(ZaloUserDAO != null);
            }
        }
        [Test]
        [TestCase("9049318283933813530", "Hello")]
        public void SendTextMessageToUserId(string UserId, string Message)
        {
            ZaloService.SendTextMessageToUserId(UserId, Message);
        }
        [Test]
        [TestCase("9049318283933813530", "Hello", "https://zpsocial-f40-org.zadn.vn/7b00666dc1162d487407.jpg")]
        public void SendImageMessageToUserIdByUrl(string UserId, string Message, string ImageUrl)
        {
            ZaloService.SendImageMessageToUserIdByUrl(UserId, Message, ImageUrl);
        }
        //[Test]
        //[TestCase("9049318283933813530", "Hello", "Resources/anhgif.gif")]
        //public void SendGifMessageToUserIdByAttachmentId(string UserId, string Message, string Filename)
        //{
        //    byte[] arr = System.IO.File.ReadAllBytes(Filename);
        //    FileInfo fileInfo = new FileInfo(Filename);
        //    MemoryStream MemoryStream = new MemoryStream(arr);
        //    string attachment_id = ZaloService.UploadGifForOfficialAccountAPI(MemoryStream, fileInfo.Name);
        //    ZaloService.SendGifMessageToUserIdByAttachmentId(UserId, Message, attachment_id);
        //}

        [Test]
        [TestCase("9049318283933813530", "Resources/file.pdf")]
        public void SendFileToUserId(string UserId, string Filename)
        {
            byte[] arr = System.IO.File.ReadAllBytes(Filename);
            FileInfo fileInfo = new FileInfo(Filename);
            MemoryStream MemoryStream = new MemoryStream(arr);
            string token = ZaloService.UploadFileForOfficialAccountAPI(MemoryStream, fileInfo.Name);
            ZaloService.SendFileToUserId(UserId, token);
        }
        [Test]
        [TestCase("9049318283933813530", "ABC")]
        public void TagFollower(string UserId, string TagName) { }
        [Test]
        [TestCase("9049318283933813530", "ABC")]
        public void RemoveTagFromFollower(string UserId, string TagName) { }

        [Test]
        [TestCase("ABC")]
        public void DeleteTag(string TagName) { }

        [Test]
        [TestCase("Resources/anh.jpeg")]
        public void UploadImageForOfficialAccountAPI(string Filename)
        {
            byte[] arr = System.IO.File.ReadAllBytes(Filename);
            FileInfo fileInfo = new FileInfo(Filename);
            MemoryStream MemoryStream = new MemoryStream(arr);
            ZaloService.UploadImageForOfficialAccountAPI(MemoryStream, fileInfo.Name);

        }

        [Test]
        [TestCase("Resources/anhgif.gif")]
        public void UploadGifForOfficialAccountAPI(string Filename)
        {
            byte[] arr = System.IO.File.ReadAllBytes(Filename);
            FileInfo fileInfo = new FileInfo(Filename);
            MemoryStream MemoryStream = new MemoryStream(arr);
            ZaloService.UploadGifForOfficialAccountAPI(MemoryStream, fileInfo.Name);
        }

        [Test]
        [TestCase("Resources/file.pdf")]
        public void UploadFileForOfficialAccountAPI(string Filename)
        {
            byte[] arr = System.IO.File.ReadAllBytes(Filename);
            FileInfo fileInfo = new FileInfo(Filename);
            MemoryStream MemoryStream = new MemoryStream(arr);
            ZaloService.UploadFileForOfficialAccountAPI(MemoryStream, fileInfo.Name);
        }
        [Test]
        [TestCase("9049318283933813530", 0, 10)]
        public void GetListConversationWithUser(string UserId, int Skip, int Take)
        {
            ZaloService.GetListConversationWithUser(UserId, Skip, Take);
        }
    }
}
