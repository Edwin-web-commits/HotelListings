using AspNetCoreRateLimit;
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using HotelListing.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing
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

                services.AddDbContext<DatabaseContext>(options =>

                     options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
                );

            services.AddMemoryCache();

            services.ConfigureRateLimiting();  //calling the method for Configuring the limit of requests that can be made to an End point
            services.AddHttpContextAccessor();

            //configure cashing
            services.ConfigureHttpCacheHeaders();
            //configuring Identity core framework and Authentication
            services.AddAuthentication();
            services.ConfigureIdentity(); //This method ConfigureIdentity() is in the ServiceExtentions.cs ,thats where the Identity Framework is configured
           
            services.ConfigureJWT(Configuration); // configuring JWT. The method ConfigureJWT() is in the ServiceExtensions.cs class

          
            //Cors enable us to restrict other unknown consumers from using our Api resources
            //Adding Cors policy.In this case We are allowing anyone(builder.AllowAnyOrigin) to use the Api resources
            services.AddCors( o => {
                o.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
                });

            services.AddAutoMapper(typeof(MapperInitializer)); //configurating AutoMapper 

            services.AddTransient<IUnitOfWork, UnitOfWork>(); //AddTransient means a new instance of IUnitOfWork will be created whenever it is needed

            services.AddScoped<IAuthManager, AuthManager>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });


            services.AddControllers(config => {
                config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
                {
                    Duration = 120
                });
            }).AddNewtonsoftJson(op => op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore) ;

            services.ConfigureVersioning(); //configure Api versioning
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline...
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));

            app.ConfigureExceptionHandler(); //configure global error handler

            app.UseHttpsRedirection();
            app.UseCors("AllowAll"); //configure Cors

            app.UseResponseCaching(); //configure cashing middleware
            app.UseHttpCacheHeaders();//configure cashing middleware
            app.UseIpRateLimiting();//configure request limit middleware 
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
           




            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
