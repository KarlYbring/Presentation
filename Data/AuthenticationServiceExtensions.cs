using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddLocalIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUserEntity, IdentityRole>(x =>
        {
            x.User.RequireUniqueEmail = true;
            x.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<DataContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(x =>
        {
            x.LoginPath = "/Auth/Login";
            x.AccessDeniedPath = configuration["Authentication:AccessDeniedPath"];
            x.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            x.SlidingExpiration = true;
            x.Cookie.HttpOnly = true;
        });

        return services;
    }
}