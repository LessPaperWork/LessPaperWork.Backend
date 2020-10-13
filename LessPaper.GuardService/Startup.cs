using System.Reflection;
using LessPaper.Guard.Database.MongoDb.Interfaces;
using LessPaper.Guard.Database.MongoDb.Models;
using LessPaper.GuardService.Options;
using LessPaper.Shared.Interfaces.Database;
using LessPaper.Shared.Interfaces.Database.Manager;
using LessPaper.Shared.Interfaces.WriteApi;
using LessPaper.Shared.Rest.Models.DtoSwaggerExamples;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.GuardService
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
            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("CustomSettings"));

            services.AddSingleton<IDatabaseSettings>(provider =>
                new DatabaseSettings(provider.GetService<IOptions<AppSettings>>()));
            services.AddSingleton<IDatabaseManager, DatabaseManager>();
            services.AddSingleton<IDbUserManager, DbUserManager>();
            services.AddSingleton<IDbDirectoryManager, DbDirectoryManager>();
            services.AddSingleton<IDbFileManager, DbFileManager>();

           
            services.RegisterSwaggerSharedDtoExamples();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Guard API", 
                    Version = "v1"
                });
                c.EnableAnnotations();
                c.ExampleFilters();
                c.OperationFilter<SwaggerParameterAttributeFilter>();
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guard API v1");
            });
        }
    }
}
