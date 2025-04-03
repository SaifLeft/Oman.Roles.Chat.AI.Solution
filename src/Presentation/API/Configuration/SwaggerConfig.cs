using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace API.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Smart Lawyer API",
                    Version = "v1",
                    Description = "API for the Smart Lawyer project - an AI-powered legal consultation system for Oman",
                    Contact = new OpenApiContact
                    {
                        Name = "Smart Lawyer Team",
                        Email = "support@smartlawyer.om",
                        Url = new Uri("https://smartlawyer.om")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Proprietary",
                        Url = new Uri("https://smartlawyer.om/license")
                    }
                });

                // Add JWT Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // Group endpoints by controller
                options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
                options.DocInclusionPredicate((name, api) => true);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Lawyer API v1");
                c.RoutePrefix = string.Empty;
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.DefaultModelsExpandDepth(-1); // Hide models section by default
                c.EnableDeepLinking();
                c.DisplayRequestDuration();
                
                // Add languages selector
                c.InjectStylesheet("/swagger-ui/custom.css");
                c.InjectJavascript("/swagger-ui/custom.js");
            });

            return app;
        }
    }
} 