using Thunders.Api.Filters;
using Thunders.Api.Monitoring;
using Thunders.Application.AutoMapper;
using Thunders.Application.Service.Interfaces;
using Thunders.Application.Services;
using Thunders.Domain;
using Thunders.Domain.Core.Interfaces.Repositories;
using Thunders.Domain.Core.Interfaces.Services;
using Thunders.Domain.Services.Services;
using Thunders.Infra.Data.Context;
using Thunders.Infra.Data.Repositories;
using Thunders.Infra.Data.UoW;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using Serilog.Debugging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using DocumentFormat.OpenXml.Office2010.Excel;
using Thunders.Application.Dtos;
using System.Text.Json;
using System.ServiceModel;
using StackExchange.Redis;

namespace Thunders.Api
{
    public class Startup
    {
        private readonly string CorsPolicy = "All";
        public IConfiguration Configuration { get; }
        public AppSettings AppSettings { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(configuration)
                            .CreateLogger();

            SelfLog.Enable(Console.Out);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<Filters.ExceptionHandlerMiddleware>();
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseRouting();
            app.UseHttpMetrics();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(CorsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = StartupHealthCheck.WriteResponse
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

 

        public void ConfigureServices(IServiceCollection services)
        {
            // Configurar Redis
            var redisConnection = Configuration.GetValue<string>("Redis:ConnectionString");
            var redis = ConnectionMultiplexer.Connect(redisConnection);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            // Adicionar a duração do cache como um serviço
            var cacheDuration = Configuration.GetValue<int>("CacheSettings:ProdutoCacheDuration");
            services.AddSingleton(new CacheSettings { ProdutoCacheDuration = cacheDuration });

            services.AddControllers();
            services.AddSystemMetrics();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddHealthChecks()
                .AddCheck("Db Check", new SqlConnectionHealthCheck(connectionString))
                .ForwardToPrometheus();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var mvc = services.AddMvc(setupAction =>
            {
                setupAction.Filters.Add(new ProducesAttribute("application/json", "application/xml"));

                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            });

            mvc.AddJsonOptions(
                options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.WriteIndented = false;
                    options.JsonSerializerOptions.AllowTrailingCommas = false;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            AppSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(AppSettings);

            var siteURL = AppSettings.SiteUrl;

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                builder =>
                {
                    builder.WithOrigins(siteURL)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // Registro correto do DbContext
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            ConfigureContainerServices(services);
        }

        private void ConfigureContainerServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<IDbConnection>(p => new SqlConnection(connectionString));

            services.AddHttpClient();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IMemoryCache>(p => new MemoryCache(new MemoryCacheOptions() { SizeLimit = 5000 }));

            services.AddScoped<IProdutoAppService, ProdutoAppService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            var config = AutoMapperConfig.RegisterMappings();
            services.AddSingleton(config);
            services.AddTransient(p => config.CreateMapper(services.BuildServiceProvider().GetService));
        }
    }
}
