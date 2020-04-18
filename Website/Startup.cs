using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NPoco;
using System;
using System.Data.SqlClient;
using System.Text;
using Website.Services;

namespace Website
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
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Configuration["DomainName"],
                    ValidAudience = Configuration["DomainName"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SigningKey"]))
                };
            });

            if(Configuration["IsProduction"] == "true")
            {
                Console.WriteLine("Using lets encrypt production: " + Configuration["IsProduction"]);

                services.AddLetsEncrypt(x => {
                    x.AcceptTermsOfService = true;
                    x.DomainNames = new string[] { "automate.ml", "www.automate.ml" };
                    x.EmailAddress = "dylanstrohschein@gmail.com";
                });
            }
            else
            {
                Console.WriteLine("Not using lets encrypt production: " + Configuration["IsProduction"]);
            }

            var db = new Database(
                    $"Data Source={Configuration["DatabaseServer"]};Initial Catalog={Configuration["DatabaseName"]};User ID={Configuration["DatabaseUsername"]};Password={Configuration["DatabasePassword"]};MultipleActiveResultSets=True;",
                    DatabaseType.SqlServer2012,
                    SqlClientFactory.Instance
                    );

            db.OpenSharedConnection();

            services.AddSingleton<IDatabase>(db);

            services.AddSingleton<ITokenBuilder>(new TokenBuilder(
                Configuration["DomainName"],
                Encoding.UTF8.GetBytes(Configuration["SigningKey"])
                ));

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddResponseCompression();
            services.AddResponseCaching();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseResponseCompression();
            app.UseResponseCaching();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
