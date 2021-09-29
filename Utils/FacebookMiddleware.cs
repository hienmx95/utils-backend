using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utils.Models;

namespace Utils
{
    public class FacebookMiddleware
    {
        private readonly RequestDelegate _next;
        private IConfiguration Configuration;
        public FacebookMiddleware(RequestDelegate next, IConfiguration Configuration)
        {
            _next = next;
            this.Configuration = Configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments("/rpc/utils/facebook/web-hook") && httpContext.Request.Method == "POST")
            {
                var originalRequestBody = httpContext.Request.Body;
                httpContext.Request.EnableBuffering();
                MemoryStream MemoryStream = new MemoryStream();
                await httpContext.Request.Body.CopyToAsync(MemoryStream);
                MemoryStream.Position = 0;
                httpContext.Request.Body.Position = 0;
                using (var stream = new StreamReader(MemoryStream))
                {
                    string headerStr = httpContext.Request.Headers["X-Hub-Signature"];
                    if (headerStr == null)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        return;
                    }
                    else if (headerStr.StartsWith("sha1="))
                        headerStr = headerStr.Substring(5);

                    string bodyStr = await stream.ReadToEndAsync();

                    string FACEBOOK_APPID = Configuration.GetSection("Facebook:AppId").Value;
                    string FACEBOOK_APPSECRET = Configuration.GetSection("Facebook:AppSecret").Value;
                    string connectionString = Configuration.GetConnectionString("DataContext");
                    var options = new DbContextOptionsBuilder<DataContext>().UseSqlServer(connectionString).Options;
                    DataContext DataContext = new DataContext(options);
                    string sha1 = ComputeHmac(bodyStr, FACEBOOK_APPSECRET);
                    FacebookPayloadDAO FacebookPayloadDAO = new FacebookPayloadDAO
                    {
                        ComputedHeader = sha1,
                        Content = bodyStr,
                        Header = headerStr,
                    };
                    DataContext.FacebookPayload.Add(FacebookPayloadDAO);
                    DataContext.SaveChanges();
                }
                await _next(httpContext);
                // important, need for next  middleware
                httpContext.Request.Body = originalRequestBody;
                return;
            }
            await _next(httpContext);
            return;
        }

        private string ComputeHmac(string rawData, string secret)
        {
            byte[] key = Encoding.ASCII.GetBytes(secret);
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = Encoding.ASCII.GetBytes(rawData);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);

        }
    }

    public static class FacebookMiddlewareExtensions
    {
        public static IApplicationBuilder UseFacebookMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FacebookMiddleware>();
        }
    }
}
