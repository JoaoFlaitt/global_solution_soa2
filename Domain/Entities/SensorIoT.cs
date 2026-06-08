using OrbitGuardAI.Domain.Enums;

namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Sensor IoT instalado em campo — pluviômetro, inclinômetro, etc.
/// </summary>
public class SensorIoT : Dispositivo
{
    public TipoSensor Tipo { get; private set; }
    public double LatitudeInstalacao { get; private set; }
    public double LongitudeInstalacao { get; private set; }
    public string RegiaoCobertura { get; private set; } = string.Empty;

    private SensorIoT() : base() { }

    public SensorIoT(string nome, TipoSensor tipo, double lat, double lon, string regiao) : base(nome)
    {
        Tipo = tipo;
        LatitudeInstalacao = lat;
        LongitudeInstalacao = lon;
        RegiaoCobertura = regiao;
        RegistrarInstancia();
    }

    /// <summary>
    /// Coleta polimórfica — cada tipo de sensor produz indicadores específicos.
    /// </summary>
    public override LeituraTelemetrica Coletar()
    {
        UltimaLeitura = DateTime.UtcNow;
        var rnd = Random.Shared;
        var indicadores = new Dictionary<string, double>();

        switch (Tipo)
        {
            case TipoSensor.Pluviometro:
                indicadores["chuva_mm_h"] = Math.Round(rnd.NextDouble() * 80, 2);
                break;
            case TipoSensor.Umidade:
                indicadores["umidade_pct"] = Math.Round(40 + rnd.NextDouble() * 60, 2);
                break;
            case TipoSensor.Temperatura:
                indicadores["temperatura_c"] = Math.Round(15 + rnd.NextDouble() * 30, 2);
                break;
            case TipoSensor.NivelRio:
                indicadores["nivel_rio_m"] = Math.Round(rnd.NextDouble() * 12, 2);
                break;
            case TipoSensor.Inclinometro:
                indicadores["inclinacao_graus"] = Math.Round(rnd.NextDouble() * 45, 2);
                break;
            case TipoSensor.QualidadeAr:
                indicadores["pm25_ugm3"] = Math.Round(rnd.NextDouble() * 250, 2);
                break;
        }

        return new LeituraTelemetrica
        {
            DispositivoId = Id,
            DataHora = DateTime.UtcNow,
            Indicadores = indicadores
        };
    }

    /// <summary>
    /// Sensores em campo são considerados saudáveis com janela menor de 10 min.
    /// </summary>
    public override bool EstaSaudavel() =>
        Status == StatusDispositivo.Ativo &&
        UltimaLeitura.HasValue &&
        (DateTime.UtcNow - UltimaLeitura.Value).TotalMinutes < 10;
}