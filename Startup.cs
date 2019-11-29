using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FinancialEntries.Extensions;
using Microsoft.OpenApi.Models;
using FinancialEntries.Services.ValidationAttributes;
using FinancialEntries.Services;

namespace FinancialEntries
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddControllers();
            services.AddDependencyInjection();
            services.AddCors(options => 
            {
                options.AddPolicy("Cors", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<SwaggerExcludeFilter>();
                options.IgnoreObsoleteProperties();
                options.CustomSchemaIds(schema => schema.FullName);
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo 
                    { 
                        Title = "FinancialEntries API", 
                        Description = "A simple example for generating FinancialEntries",
                        Version = "v1" 
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseCors("Cors");
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "docs/{documentName}/docs.json";
            });
            app.UseSwaggerUI(options => 
            {
                options.SwaggerEndpoint("/docs/v1/docs.json", "Web API v1");
                options.RoutePrefix = "docs";
            });
            StaticServiceProvider.Provider = app.ApplicationServices;
        }
    }
}
