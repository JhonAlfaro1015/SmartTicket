namespace SmartTicket.Application.Common;

public static class PermisosSolicitud
{
    public static bool PuedeVer(string rol, int usuarioId, int clienteIdSolicitud, int? usuarioAsignadoId) => rol switch
    {
        RolesSistema.Administrador or RolesSistema.Supervisor => true,
        RolesSistema.Agente => usuarioAsignadoId == usuarioId,
        RolesSistema.Cliente => clienteIdSolicitud == usuarioId,
        _ => false
    };

    public static bool PuedeCambiarEstado(string rol, int usuarioId, int? usuarioAsignadoId) => rol switch
    {
        RolesSistema.Administrador or RolesSistema.Supervisor => true,
        RolesSistema.Agente => usuarioAsignadoId == usuarioId,
        _ => false
    };

    public static bool PuedeComentar(string rol, int usuarioId, int clienteIdSolicitud, int? usuarioAsignadoId) => rol switch
    {
        RolesSistema.Administrador or RolesSistema.Supervisor => true,
        RolesSistema.Agente => usuarioAsignadoId == usuarioId,
        RolesSistema.Cliente => clienteIdSolicitud == usuarioId,
        _ => false
    };

    public static bool PuedeAsignar(string rol) =>
        rol == RolesSistema.Administrador || rol == RolesSistema.Supervisor;
}
