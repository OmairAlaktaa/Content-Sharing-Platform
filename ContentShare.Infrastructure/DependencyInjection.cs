using System.Text;
using ContentShare.Application.Interfaces.Repositories;
using ContentShare.Application.Interfaces.Services;
using ContentShare.Infrastructure.Identity;
using ContentShare.Infrastructure.Persistence;
using ContentShare.Infrastructure.Repositories;
using ContentShare.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ContentShare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<AppDbContext>(opt =>
        {
            var conn = config.GetConnectionString("Default");
            opt.UseNpgsql(conn, npg => npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        });

        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequiredLength = 6;
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing from configuration");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

        services.AddScoped<IContentRepository, ContentRepository>();

        services.AddScoped<IContentService, ContentService>();

        return services;
    }
}
