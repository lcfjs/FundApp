using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FundApp.Filters;
using Hangfire;
using Hangfire.MySql.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;

namespace FundApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            DapperHelper.ConnectionString = Configuration.GetSection("ConnectionStrings:Default").Value;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton<DapperHelper>();

            services.AddScoped<FundCodeService>();
            services.AddScoped<FundDataDayService>();
            services.AddScoped<FundDataMinService>();

            var storageOptions = new MySqlStorageOptions();
            storageOptions.TablePrefix = "Job";
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(DapperHelper.ConnectionString, storageOptions)));
            services.AddHangfireServer();

            HangfireTask.Instance.Services = services;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(x =>
            {
                x.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                x.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
            });

            services.AddCors(options => options.AddPolicy("CorsBoard",
                                p => p.WithOrigins("http://localhost:55161"
                                , "http://localhost:8081"
                                , "http://localhost:5000"
                                )
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                            )
                        );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // 跨域
            app.UseCors("CorsBoard");

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new DashboardAuthorizationFilter() }
            });

            HangfireTask.Instance.AddTasks();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
