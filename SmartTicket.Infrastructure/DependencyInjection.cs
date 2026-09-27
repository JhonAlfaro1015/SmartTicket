using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartTicket.Application.Interface;
using SmartTicket.Infrastructure.Persistence;
using SmartTicket.Infrastructure.Repositories;
using SmartTicket.Infrastructure.Security;

namespace SmartTicket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<DapperContext>();

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IClienteRepository, ClienteRepository>();

        return services;
    }
}