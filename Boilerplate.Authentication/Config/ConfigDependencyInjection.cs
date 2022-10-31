using Boilerplate.Authentication.Data;
using Boilerplate.Authentication.Models.StartupModels;
using Boilerplate.Authentication.Repositories;
using Boilerplate.Authentication.Repositories.Interfaces;
using Boilerplate.Authentication.Services;
using Boilerplate.Authentication.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Authentication.Config
{
    public static class ConfigDependencyInjection
    {
        public static void ConfigureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(
                opt => opt.UseSqlServer(configuration.GetSection("AuthenticationApiDatabase").Get<string>()));

            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}
