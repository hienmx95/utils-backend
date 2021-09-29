using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Models;

namespace Utils.Rpc.facebook
{
    public class FacebookRoute : Root
    {
        public const string Base = Rpc + Module + "/facebook";
        public const string WebHook = Base + "/web-hook";

    }
    public class FacebookController : ControllerBase
    {
        private DataContext DataContext;
        public FacebookController(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        [Route(FacebookRoute.WebHook), HttpGet]
        public async Task<IActionResult> WebhookGet()
        {
            string VERIFY_TOKEN = "Test";
            string mode = Request.Query["hub.mode"];
            string token = Request.Query["hub.verify_token"];
            string challenge = Request.Query["hub.challenge"];
            if (mode == "subscribe" && token == VERIFY_TOKEN)
            {
                return Ok(challenge);
            }
            else
            {
                return BadRequest();
            }
        }
        [Route(FacebookRoute.WebHook), HttpPost]
        public async Task<IActionResult> Webhook([FromBody] FacebookWebHookPayloadDTO payload)
        {

            return Ok();
        }
    }
}
