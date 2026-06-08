using OrbitGuardAI.Application.DTOs;
using OrbitGuardAI.Domain.Entities;

namespace OrbitGuardAI.Domain.Interfaces;

/// <summary>
/// Contrato do motor preditivo de IA — abstrai modelos de ML/visão computacional.
/// </summary>
public interface IPreditorClimaticoService
{
    Task<PrevisaoDTO> PreverAsync(AreaRisco area, IEnumerable<LeituraTelemetrica> historico);
}

/// <summary>
/// Contrato do orquestrador de alertas — gera, persiste e dispara alertas.
/// </summary>
public interface IAlertaService
{
    Task<Alerta> GerarAlertaAsync(AreaRisco area, PrevisaoDTO previsao);
    Task<IEnumerable<Alerta>> ListarAtivosAsync();
    Task EncerrarAsync(Guid alertaId);
}

/// <summary>
/// Contrato do gateway de integração com APIs externas (NASA, INPE, CEMADEN).
/// </summary>
public interface ISatelliteDataGateway
{
    Task<IEnumerable<LeituraTelemetrica>> ObterDadosOrbitaisAsync(double lat, double lon, DateTime desde);
}

/// <summary>
/// Contrato de autenticação JWT.
/// </summary>
public interface ITokenService
{
    string GerarToken(Usuario usuario);
}

/// <summary>
/// Contrato de notificação multicanal (push, SMS, e-mail).
/// </summary>
public interface INotificacaoService
{
    Task EnviarAsync(Alerta alerta, string canal);
}