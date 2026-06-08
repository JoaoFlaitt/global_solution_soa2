using OrbitGuardAI.Domain.Enums;

namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Satélite de observação terrestre — fornece imagens e dados orbitais (NDVI, temperatura de superfície, umidade).
/// Herda de Dispositivo.
/// </summary>
public class Satelite : Dispositivo
{
    public string Operadora { get; private set; } = string.Empty;
    public double AltitudeKm { get; private set; }
    public string TipoOrbita { get; private set; } = "LEO";
    public List<string> SensoresEmbarcados { get; private set; } = new();

    private Satelite() : base() { }

    public Satelite(string nome, string operadora, double altitudeKm, string tipoOrbita)
        : base(nome)
    {
        Operadora = operadora;
        AltitudeKm = altitudeKm;
        TipoOrbita = tipoOrbita;
        RegistrarInstancia();
    }

    public void AdicionarSensor(string sensor) => SensoresEmbarcados.Add(sensor);

    /// <summary>
    /// Polimorfismo — coleta uma leitura simulada de imagem orbital.
    /// </summary>
    public override LeituraTelemetrica Coletar()
    {
        UltimaLeitura = DateTime.UtcNow;
        var rnd = Random.Shared;
        return new LeituraTelemetrica
        {
            DispositivoId = Id,
            DataHora = DateTime.UtcNow,
            Indicadores = new Dictionary<string, double>
            {
                ["ndvi"] = Math.Round(rnd.NextDouble(), 3),
                ["temperatura_superficie_c"] = Math.Round(20 + rnd.NextDouble() * 25, 2),
                ["umidade_solo_pct"] = Math.Round(rnd.NextDouble() * 100, 2),
                ["cobertura_nuvens_pct"] = Math.Round(rnd.NextDouble() * 100, 2)
            }
        };
    }
}