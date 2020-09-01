using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SortThineLetters.Storage.MongoDB;
using System;

namespace SortThineLetters.Server
{
    public class Startup
    {
        private readonly string Version = $"v{typeof(Startup).Assembly.GetName().Version}";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Version, new OpenApiInfo()
                {
                    Title = "SortThineLetters Web API",
                    Version = Version,
                    Contact = new OpenApiContact()
                    {
                        Name = "Raphael \"rGunti\" Guntersweiler",
                        Email = "raphael+sortthineletters@guntersweiler.net"
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://github.com/rGunti/SortThineLetters/blob/master/LICENSE")
                    }
                });
                c.EnableAnnotations();
            });

            services.AddMongoRepositories(
                Configuration.GetConnectionString(Configuration.GetValue<string>("UseConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    $"swagger/{Version}/swagger.json",
                    $"SortThineLetters Web API {Version}");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
