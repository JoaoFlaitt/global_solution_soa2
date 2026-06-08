using OrbitGuardAI.Domain.Enums;

namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Alerta climático gerado pela IA preditiva e enviado a usuários e gestores públicos.
/// </summary>
public class Alerta
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AreaRiscoId { get; set; }
    public TipoEventoClimatico Tipo { get; set; }
    public NivelSeveridade Severidade { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public double Probabilidade { get; set; }
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public DateTime? DataExpiracao { get; set; }
    public string RecomendacaoAcao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public TimeSpan? TempoRestante() =>
        DataExpiracao.HasValue ? DataExpiracao.Value - DateTime.UtcNow : null;

    public void Encerrar() => Ativo = false;
}