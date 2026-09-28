using Microsoft.Extensions.DependencyInjection;
using SmartTicket.Application.Interface;
using SmartTicket.Application.Services.Clasificacion;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;
using SmartTicket.Infrastructure.MachineLearning;
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
        services.AddScoped<ISolicitudRepository, SolicitudRepository>();
        services.AddScoped<IEstadoRepository, EstadoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<IPrioridadRepository, PrioridadRepository>();
        services.AddScoped<ISubcategoriaRepository, SubcategoriaRepository>();
        services.AddSingleton<IMotorClasificacionTexto, MotorClasificacionTexto>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        var rutaModelo = Path.Combine(AppContext.BaseDirectory, "MachineLearning", "modelo_categoria.zip");
        Directory.CreateDirectory(Path.GetDirectoryName(rutaModelo)!);

        services.AddSingleton<IEntrenadorModeloML>(_ => new EntrenadorModeloML(rutaModelo));
        services.AddSingleton<IMotorClasificacionML>(_ => new MotorClasificacionML(rutaModelo));

        return services;
    }
}