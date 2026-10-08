namespace SmartTicket.Application.Common.Exceptions;

/// <summary>Una regla de negocio impide la operación (HTTP 409).</summary>
public class ReglaNegocioException(string mensaje) : Exception(mensaje)
{
}

/// <summary>El recurso solicitado no existe (HTTP 404).</summary>
public class RecursoNoEncontradoException(string mensaje) : Exception(mensaje)
{
}

/// <summary>El usuario está autenticado pero no tiene permiso para esta acción (HTTP 403).</summary>
public class AccesoDenegadoException(string mensaje) : Exception(mensaje)
{
}