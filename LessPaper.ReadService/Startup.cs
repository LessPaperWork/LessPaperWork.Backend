using LessPaper.ReadService.Options;
using LessPaper.Shared.Interfaces.Bucket;
using LessPaper.Shared.Interfaces.GuardApi;
using LessPaper.Shared.MinIO.Interfaces;
using LessPaper.Shared.MinIO.Models;
using LessPaper.Shared.Rest;
using LessPaper.Shared.Rest.Interface;
using LessPaper.Shared.Rest.Models.DtoSwaggerExamples;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using MinioSettings = LessPaper.ReadService.Options.MinioSettings;

namespace LessPaper.ReadService
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


            services.AddSingleton<IMinioSettings>(provider => new MinioSettings(provider.GetService<IOptions<AppSettings>>()));
            services.AddSingleton<IWritableBucket, MinioBucket>();

            services.AddSingleton<IClientSettings, GuardClientSettings>();
            services.AddSingleton<IBaseClient, RestSharpBaseClient>();
            services.AddSingleton<IGuardApi, GuardServiceClient>();

            services.RegisterSwaggerSharedDtoExamples();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Read API",
                    Version = "v1"
                });
                c.EnableAnnotations();
                c.ExampleFilters();
                c.OperationFilter<SwaggerParameterExampleFilter>();
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Read API v1");
            });
        }
    }
}
