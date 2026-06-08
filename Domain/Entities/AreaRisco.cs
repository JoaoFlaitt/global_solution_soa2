namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Região vulnerável monitorada (bairro, encosta, bacia hidrográfica, área de cerrado).
/// </summary>
public class AreaRisco
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RaioKm { get; set; } = 5;
    public int PopulacaoEstimada { get; set; }
    public string TiposRiscoJson { get; set; } = "[]";

    public Coordenada AsCoordenada() => new(Latitude, Longitude);
}