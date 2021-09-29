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
    public class ZaloMiddleware
    {
        private readonly RequestDelegate _next;
        private IConfiguration Configuration;
        public ZaloMiddleware(RequestDelegate next, IConfiguration Configuration)
        {
            _next = next;
            this.Configuration = Configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments("/rpc/utils/zalo/web-hook"))
            {
                string appId = httpContext.Request.Query["appId"];
                if (string.IsNullOrEmpty(appId))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    return;
                }

                var originalRequestBody = httpContext.Request.Body;
                httpContext.Request.EnableBuffering();
                MemoryStream MemoryStream = new MemoryStream();
                await httpContext.Request.Body.CopyToAsync(MemoryStream);
                MemoryStream.Position = 0;
                httpContext.Request.Body.Position = 0;
                using (var stream = new StreamReader(MemoryStream))
                {
                    string headerStr = httpContext.Request.Headers["X-ZEvent-Signature"];
                    if (headerStr == null)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        return;
                    }
                    else if (headerStr.StartsWith("mac="))
                        headerStr = headerStr.Substring(4);

                    string bodyStr = await stream.ReadToEndAsync();

                    string connectionString = Configuration.GetConnectionString("DataContext");
                    var options = new DbContextOptionsBuilder<DataContext>().UseSqlServer(connectionString).Options;
                    DataContext DataContext = new DataContext(options);
                    ZaloConfigurationDAO ZaloConfigurationDAO = await DataContext.ZaloConfiguration
                        .Where(x => x.AppId == appId).FirstOrDefaultAsync();
                    if (ZaloConfigurationDAO == null)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        return;
                    }
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(bodyStr);
                    string timestamp = obj["timestamp"];
                    string combine = ZaloConfigurationDAO.AppId + bodyStr + timestamp + ZaloConfigurationDAO.OASecretKey;
                    string sha256 = ComputeSha256Hash(combine);

                    if (sha256 != headerStr)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        return;
                    }

                    ZaloPayloadDAO ZaloPayloadDAO = new ZaloPayloadDAO
                    {
                        ComputedHeader = sha256,
                        Content = bodyStr,
                        Header = headerStr,
                    };
                    DataContext.ZaloPayload.Add(ZaloPayloadDAO);
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

    public static class ZaloMiddlewareExtensions
    {
        public static IApplicationBuilder UseZaloMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ZaloMiddleware>();
        }
    }
}
