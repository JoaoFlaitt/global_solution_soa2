using OrbitGuardAI.Domain.Enums;

namespace OrbitGuardAI.Application.DTOs;

public record LoginDTO(string Email, string Senha);
public record TokenDTO(string Token, DateTime Expiracao, string Perfil);

public record RegistroUsuarioDTO(string Nome, string Email, string Senha, string Perfil);

public record AreaRiscoDTO(
    Guid? Id,
    string Nome,
    string Municipio,
    string Estado,
    double Latitude,
    double Longitude,
    double RaioKm,
    int PopulacaoEstimada);

public record SensorDTO(
    Guid? Id,
    string Nome,
    TipoSensor Tipo,
    double Latitude,
    double Longitude,
    string Regiao);

public record SateliteDTO(
    Guid? Id,
    string Nome,
    string Operadora,
    double AltitudeKm,
    string TipoOrbita);

public record LeituraDTO(
    Guid DispositivoId,
    DateTime DataHora,
    Dictionary<string, double> Indicadores);

public record AlertaDTO(
    Guid Id,
    Guid AreaRiscoId,
    TipoEventoClimatico Tipo,
    NivelSeveridade Severidade,
    string Titulo,
    string Descricao,
    double Probabilidade,
    DateTime DataEmissao,
    DateTime? DataExpiracao,
    string RecomendacaoAcao,
    bool Ativo);

/// <summary>
/// Resultado da inferência preditiva — saída do modelo de IA.
/// </summary>
public record PrevisaoDTO(
    TipoEventoClimatico TipoEvento,
    double Probabilidade,
    NivelSeveridade Severidade,
    string Justificativa,
    DateTime JanelaPrevistaInicio,
    DateTime JanelaPrevistaFim);