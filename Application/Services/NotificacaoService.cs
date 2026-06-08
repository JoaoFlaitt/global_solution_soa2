using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Interfaces;

namespace OrbitGuardAI.Application.Services;

/// <summary>
/// Mock de notificação multicanal — substituível por Twilio/Firebase/AWS SNS.
/// </summary>
public class NotificacaoService : INotificacaoService
{
    private readonly ILogger<NotificacaoService> _logger;
    public NotificacaoService(ILogger<NotificacaoService> logger) => _logger = logger;

    public Task EnviarAsync(Alerta alerta, string canal)
    {
        _logger.LogWarning("[ORBIT GUARD AI] [{Canal}] Disparando alerta {Severidade} - {Titulo}",
            canal.ToUpperInvariant(), alerta.Severidade, alerta.Titulo);
        return Task.CompletedTask;
    }
}