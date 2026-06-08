namespace OrbitGuardAI.Domain.Enums;

/// <summary>
/// Tipos de eventos climáticos extremos monitorados pela plataforma.
/// </summary>
public enum TipoEventoClimatico
{
    Enchente = 1,
    Deslizamento = 2,
    Queimada = 3,
    Seca = 4,
    Tempestade = 5
}

/// <summary>
/// Níveis de severidade de um alerta — alinhados ao protocolo CAP (Common Alerting Protocol).
/// </summary>
public enum NivelSeveridade
{
    Informativo = 0,
    Baixo = 1,
    Moderado = 2,
    Alto = 3,
    Critico = 4
}

/// <summary>
/// Status operacional de um dispositivo (satélite ou sensor IoT).
/// </summary>
public enum StatusDispositivo
{
    Ativo = 1,
    Inativo = 2,
    Manutencao = 3,
    Falha = 4
}

/// <summary>
/// Tipos de sensores IoT em campo.
/// </summary>
public enum TipoSensor
{
    Pluviometro = 1,
    Umidade = 2,
    Temperatura = 3,
    NivelRio = 4,
    Inclinometro = 5,
    QualidadeAr = 6
}