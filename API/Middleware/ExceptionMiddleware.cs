using OrbitGuardAI.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace OrbitGuardAI.API.Middleware;

/// <summary>
/// Middleware global de tratamento de exceções — sistemas espaciais críticos
/// nunca devem responder com stack trace ou crashar.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (OrbitGuardException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio");
            await Responder(ctx, ex.StatusCode, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await Responder(ctx, 401, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado");
            await Responder(ctx, 500, "Erro interno na plataforma ORBIT GUARD AI.");
        }
    }

    private static Task Responder(HttpContext ctx, int status, string mensagem)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = status;
        var payload = JsonSerializer.Serialize(new { status, mensagem, timestamp = DateTime.UtcNow });
        return ctx.Response.WriteAsync(payload);
    }
}