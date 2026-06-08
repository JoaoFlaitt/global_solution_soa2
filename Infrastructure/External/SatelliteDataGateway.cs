using OrbitGuardAI.Application.Exceptions;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Interfaces;

namespace OrbitGuardAI.Infrastructure.External;

/// <summary>
/// Gateway simulado de integração com APIs orbitais (NASA Earthdata, INPE, Copernicus, CEMADEN).
/// Em produção, faria HttpClient typed para cada provedor.
/// </summary>
public class SatelliteDataGateway : ISatelliteDataGateway
{
    public Task<IEnumerable<LeituraTelemetrica>> ObterDadosOrbitaisAsync(double lat, double lon, DateTime desde)
    {
        try
        {
            var leituras = new List<LeituraTelemetrica>();
            var rnd = Random.Shared;
            for (int i = 0; i < 6; i++)
            {
                leituras.Add(new LeituraTelemetrica
                {
                    DispositivoId = Guid.Empty,
                    DataHora = desde.AddHours(i * 4),
                    Indicadores = new Dictionary<string, double>
                    {
                        ["ndvi"] = Math.Round(rnd.NextDouble(), 3),
                        ["temperatura_superficie_c"] = Math.Round(20 + rnd.NextDouble() * 25, 2),
                        ["umidade_solo_pct"] = Math.Round(rnd.NextDouble() * 100, 2),
                        ["cobertura_nuvens_pct"] = Math.Round(rnd.NextDouble() * 100, 2)
                    }
                });
            }
            return Task.FromResult<IEnumerable<LeituraTelemetrica>>(leituras);
        }
        catch (Exception ex)
        {
            throw new FalhaIntegracaoSateliteException(ex.Message);
        }
    }
}