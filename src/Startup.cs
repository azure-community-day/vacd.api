using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using luis.azure.api.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace luis.azure.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Environment = env;

            if (env.EnvironmentName == "Test")
            {
                var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
                var relativePathToHostProject = @"..\..\..\..\src";

                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(testProjectPath, relativePathToHostProject))
                    .AddJsonFile("appsettings.json", optional: true)
                    //.AddJsonFile("appsettings.Development.json", optional: true)
                    .AddUserSecrets<Startup>()
                    .Build();
            }
            else
                Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddTransient<IResourceBuilder, ResourceBuilder>();
            services.AddTransient<ITokenBuilder, TokenBuilder>();

            services.AddMemoryCache();

            
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        var authority = $"{Configuration["AzureAd:Instance"].ToString()}{Configuration["AzureAd:TenantId"].ToString()}";
                        var audience = Configuration["ServicePrincipalManagerApp:APIId"];
                        options.Authority = authority;
                        options.Audience = audience;
                        options.TokenValidationParameters.ValidateLifetime = true;
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "LUIS", Description = "A Resource Builder API", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.IncludeXmlComments(Path.Combine(Directory.GetCurrentDirectory(), "luis.azure.api.xml"));
            });

            services.AddMvcCore(c =>
            {

                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                c.Filters.Add(new AuthorizeFilter(policy));
                c.Filters.Add(new ConsumesAttribute("application/json"));
            })
                .AddApiExplorer()
                .AddAuthorization()
                .AddFormatterMappings()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddCors()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service Principal Manager V1");
            });

            app.UseMvc();
        }
    }
}
