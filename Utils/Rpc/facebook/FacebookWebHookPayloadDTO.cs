using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Rpc.facebook
{
   
    public class FacebookWebHookPayloadDTO
    {
        [JsonProperty("object")]
        public string objectType { get; set; }
        public List<FacebookEntryDTO> entry { get; set; }
    }
}
