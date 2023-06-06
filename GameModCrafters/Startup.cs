using GameModCrafters.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using GameModCrafters.Encryption;

namespace GameModCrafters
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession(); // 添加Session服務
            services.AddSingleton<IHashService, HashService>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//加入Cookie驗證, 同時設定選項
            .AddCookie(options =>
            {
                //預設登入驗證網址為Account/Login, 若想變更才需要設定LoginPath
                
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.LoginPath = new PathString("/Account/LoginPage/");
                //options.AccessDeniedPath = "/Account/Forbidden/";  //拒絕訪問!
            });
            services.AddAuthorization();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
            Configuration.GetConnectionString("ApplicationDbContext")));
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
           

             
            app.UseSession(); // 啟用Session

            app.UseRouting();
            app.UseAuthentication(); //驗證
            app.UseAuthorization(); //授權

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
