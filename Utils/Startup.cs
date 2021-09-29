using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using Utils.Helpers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Utils.Common;
using Utils.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Extensions;
using Microsoft.OpenApi.Models;
using Utils.Config;
using Hangfire.SqlServer;
using Hangfire;
using Utils.Service;
using Hangfire.Storage;
using OfficeOpenXml;
using Microsoft.Extensions.Primitives;
using Utils.Hub;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using Utils.Handlers;
using Utils.Rpc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.IO;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Utils
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

            Configuration = builder.Build();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            LicenseManager.AddLicense("2456;100-FPT", "3f0586d1-0216-5005-8b7a-9080b0bedb5e");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = DataEntity.ErrorResource;
            services.AddControllers().AddNewtonsoftJson(
                options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK";
                });
            //services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            //services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            //services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
            //services.AddSingleton<IRabbitManager, RabbitManager>();
            //services.AddHostedService<ConsumeRabbitMQHostedService>();

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DataContext"));
                options.AddInterceptors(new HintCommandInterceptor());
            }); 
            EntityFrameworkManager.ContextFactory = context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
                DataContext DataContext = new DataContext(optionsBuilder.Options);
                return DataContext;
            };

            services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(Configuration.GetConnectionString("DataContext"), new SqlServerStorageOptions
               {
                   SlidingInvisibilityTimeout = TimeSpan.FromMinutes(2),
                   QueuePollInterval = TimeSpan.FromSeconds(10),
                   CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                   UseRecommendedIsolationLevel = true,
                   UsePageLocksOnDequeue = true,
                   DisableGlobalLocks = true
               }));
            services.AddHangfireServer();

            services.Scan(scan => scan
             .FromAssemblyOf<IServiceScoped>()
                 .AddClasses(classes => classes.AssignableTo<IServiceScoped>())
                     .AsImplementedInterfaces()
                     .WithScopedLifetime());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["Token"];
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKeyResolver = (token, secutiryToken, kid, validationParameters) =>
                    {
                        var secretKey = Configuration["Config:SecretKey"];
                        var key = Encoding.ASCII.GetBytes(secretKey);
                        SecurityKey issuerSigningKey = new SymmetricSecurityKey(key);
                        return new List<SecurityKey>() { issuerSigningKey };
                    }
                };
            });

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission", policy =>
                    policy.Requirements.Add(new PermissionRequirement()));
            });

            Action onChange = () =>
            {
                string credential = Configuration["Firebase:Credential"];
                var base64EncodedBytes = System.Convert.FromBase64String(credential);
                credential = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var defaultApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(credential)
                });

                var emailConfig = Configuration
                 .GetSection("EmailConfig")
                 .Get<EmailConfig>();
                services.AddSingleton(emailConfig);

                JobStorage.Current = new SqlServerStorage(Configuration.GetConnectionString("DataContext"));
                using (var connection = JobStorage.Current.GetConnection())
                {
                    foreach (var recurringJob in connection.GetRecurringJobs())
                    {
                        RecurringJob.RemoveIfExists(recurringJob.Id);
                    }
                }

                RecurringJob.AddOrUpdate<MailService>("ResendMail", x => x.ReSend(), Cron.Minutely);
                RecurringJob.AddOrUpdate<MaintenanceService>("CleanHangfire", x => x.CleanHangfire(), Cron.Monthly);
            };
            onChange();
            ChangeToken.OnChange(() => Configuration.GetReloadToken(), onChange);
            string redisConnectionString = Configuration.GetSection("RedisConnectionString").Value;
            services.AddSignalR().AddStackExchangeRedis(options =>
            {
                options.Configuration.ChannelPrefix = "UTILS";
                options.Configuration.EndPoints.Add(redisConnectionString, 6379);
                options.Configuration.ClientName = "utils-backend";
                options.Configuration.AllowAdmin = true;
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseZaloMiddleware();
            app.UseFacebookMiddleware();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<UserNotificationHub>("/rpc/utils/signalr");
                endpoints.MapHub<ChatHub>("/rpc/utils/chathub");
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "rpc/utils/swagger/{documentname}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/rpc/utils/swagger/v1/swagger.json", "Utils API");
                c.RoutePrefix = "rpc/utils/swagger";
            });
            app.UseDeveloperExceptionPage();
            app.UseHangfireDashboard("/rpc/utils/hangfire");


            app.Run(async (context) =>
            {
                if (context.Request.Path.StartsWithSegments("/rpc/utils/zalo/web-hook"))
                {

                }
            });
        }


    }
}
