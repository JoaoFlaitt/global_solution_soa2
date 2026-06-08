namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Leitura genérica de telemetria persistida no banco — chave/valor flexível.
/// </summary>
public class LeituraTelemetrica
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DispositivoId { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public Dictionary<string, double> Indicadores { get; set; } = new();

    /// <summary>
    /// Serializa indicadores para persistência (EF Core não mapeia Dictionary diretamente em todos os providers).
    /// </summary>
    public string IndicadoresJson
    {
        get => System.Text.Json.JsonSerializer.Serialize(Indicadores);
        set => Indicadores = string.IsNullOrWhiteSpace(value)
            ? new Dictionary<string, double>()
            : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, double>>(value) ?? new();
    }
}