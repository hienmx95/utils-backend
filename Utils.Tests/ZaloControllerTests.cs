using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utils.Models;
using Utils.Rpc.zalo;
using Utils.Service;
using Z.EntityFramework.Extensions;

namespace Utils.Tests
{
    [TestFixture]
    public class ZaloControllerTests : BaseTests
    {
        public ZaloController ZaloController;
        [SetUp]
        public void Setup()
        {
            Init();
            IZaloService ZaloService = new ZaloService(DataContext);
            ZaloController = new ZaloController(DataContext, ZaloService);
        }

        [Test]
        [TestCase("add_user_to_tag.json")]
        [TestCase("follow.json")]
        [TestCase("oa_send_file.json")]
        [TestCase("oa_send_gif.json")]
        [TestCase("oa_send_image.json")]
        [TestCase("oa_send_list.json")]
        [TestCase("oa_send_text.json")]
        [TestCase("shop_has_order.json")]
        [TestCase("unfollow.json")]
        [TestCase("user_asking_product.json")]
        [TestCase("user_received_message.json")]
        [TestCase("user_seen_message.json")]
        [TestCase("user_send_audio.json")]
        [TestCase("user_send_file.json")]
        [TestCase("user_send_gif.json")]
        [TestCase("user_send_image.json")]
        [TestCase("user_send_link.json")]
        [TestCase("user_send_location.json")]
        [TestCase("user_send_sticker.json")]
        [TestCase("user_send_text.json")]
        [TestCase("user_send_video.json")]
        [TestCase("user_submit_info.json")]
        public async Task Webhook(string file)
        {
            string OAsecretKey = "t0wWRq9zAVBZ28TYpCRw";
            string json = System.IO.File.ReadAllText($"ZaloControllerData/{file}");
            ZaloWebHookPayloadDTO payload = JsonConvert.DeserializeObject<ZaloWebHookPayloadDTO>(json);
            string strCombine = payload.app_id + json + payload.timestamp + OAsecretKey;
            string sha256 = ComputeSha256Hash(strCombine);
            ZaloController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),

            };
            ZaloController.Request.Headers.Add("X-ZEvent-Signature", sha256);
            StatusCodeResult result =  (StatusCodeResult)(await ZaloController.Webhook(payload));
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
