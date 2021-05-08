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
using SAM_Backend.Hubs;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM_Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment env;

        public void ConfigureServices(IServiceCollection services)
        {
            #region Services
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SAM_Backend", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });
            services.AddSignalR();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IMinIOService, MinIOService>();
            #endregion default
            #region CORS
            services.AddCors(o => o.AddPolicy(Constants.CORSPolicyName, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            #endregion CORS
            #region Auth Policies
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>(Constants.TokenSignKey)));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser().Build();
                });
                options.DefaultPolicy = options.GetPolicy(JwtBearerDefaults.AuthenticationScheme);
            }
            );
            #endregion Auth Policies
            #region Password Complexity
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = Constants.PasswordAllowedUserNameCharacters;
                if (env.IsDevelopment())
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireUppercase = false;
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                }
                else
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Password.RequiredLength = 10;
                    options.Password.RequiredUniqueChars = 3;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                }

            });
            #endregion
            #region Db
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = Constants.RequireConfirmedEmail;
                options.Lockout.MaxFailedAccessAttempts = Constants.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Constants.DefaultLockoutTimeSpan);
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.AddDbContextPool<AppDbContext>(
            options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString(Constants.ConnectionStringKey)));
            #endregion Db
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Ordered Middlewares
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SAM_Backend v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            if (!env.IsProduction())
            {
                app.UseCors(Constants.CORSPolicyName);
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatRoomHub>(Constants.ChatRoomHubRoute);
                endpoints.MapControllerRoute(Constants.RouteName, Constants.RoutePattern);
            });
            #endregion Ordered Middlewares
        }
    }
}
