using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Common;

public static class CapacidadesSolicitud
{
    public static void Aplicar(
        SolicitudDetalleDto solicitud,
        string rol,
        int usuarioId,
        IEnumerable<EstadoSiguienteDto> destinosDelEstadoActual)
    {
        solicitud.PuedeAsignar = PermisosSolicitud.PuedeAsignar(rol) && !solicitud.EstadoEsFinal;

        solicitud.PuedeComentar = !solicitud.EstadoEsFinal &&
            PermisosSolicitud.PuedeComentar(rol, usuarioId, solicitud.ClienteId, solicitud.UsuarioAsignadoId);

        solicitud.EstadosSiguientes =
            PermisosSolicitud.PuedeCambiarEstado(rol, usuarioId, solicitud.UsuarioAsignadoId)
                ? destinosDelEstadoActual.ToList()
                : [];
    }
}
