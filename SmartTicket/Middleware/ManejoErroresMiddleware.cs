using SmartTicket.Application.Common.Exceptions;

namespace SmartTicket.Api.Middleware;

public class ManejoErroresMiddleware
{
    private readonly RequestDelegate _next;

    public ManejoErroresMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RecursoNoEncontradoException ex)
        {
            await Responder(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (AccesoDenegadoException ex)
        {
            await Responder(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (ReglaNegocioException ex)
        {
            await Responder(context, StatusCodes.Status409Conflict, ex.Message);
        }
    }

    private static async Task Responder(HttpContext context, int codigo, string mensaje)
    {
        context.Response.StatusCode = codigo;
        await context.Response.WriteAsJsonAsync(new { message = mensaje });
    }
}
