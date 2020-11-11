using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoLocator.Core.DAL;
using GeoLocator.Core.DAL.ORM;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace GeoLocator.API
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
            try
            {
                //Swagger service: for API Documentation  -----------------------------

                //services.AddSwaggerGen(F => F.SwaggerDoc("v1", new Info
                //{
                //    Title = "Dominos API",
                //    Description = "Wellcome to Dominos API. API Provides Database import operations. Please contact us for further information."
                //}));

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Dominos API",
                        Description = "Dominos API - GPS Coordinate Manager",
                        //TermsOfService = new Uri("https://example.com/terms"),
                        Contact = new OpenApiContact
                        {
                            Name = "Meric YILMAZ",
                            Email = string.Empty,
                            Url = new Uri("https://github.com/mastermeric"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Use Dominos Licence under LICX",
                            Url = new Uri("https://example.com/license"),
                        }
                    });
                });


                //Redis yerine In-Memory cache kullanildi.
                services.AddMemoryCache();

                //ORM Tool :  EF Core
                services.AddDbContext<GeoCoordinateContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DominosTestDBConnString")));

                //Dominos repo Service 
                services.AddTransient<IGeoCoordinateRepository, GeoCoordinateRepository>();

                services.AddControllers();
            }
            catch (Exception ex)
            {

            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            //Swagger middleware : for API Documentation
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "File Import API"));


            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dominos}/{action=DummyInsert}");
            });
        }
    }
}
