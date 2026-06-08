using OrbitGuardAI.Application.DTOs;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Enums;
using OrbitGuardAI.Domain.Interfaces;

namespace OrbitGuardAI.Application.Services;

/// <summary>
/// Implementação heurística de IA preditiva — combina indicadores orbitais e IoT
/// usando pesos calibrados (representa ML/visão computacional embarcados).
/// Em produção, encapsularia um modelo TensorFlow / ONNX.
/// </summary>
public class PreditorClimaticoService : IPreditorClimaticoService
{
    public Task<PrevisaoDTO> PreverAsync(AreaRisco area, IEnumerable<LeituraTelemetrica> historico)
    {
        if (area is null) throw new ArgumentNullException(nameof(area));

        var leituras = historico?.ToList() ?? new List<LeituraTelemetrica>();
        if (leituras.Count == 0)
        {
            return Task.FromResult(new PrevisaoDTO(
                TipoEventoClimatico.Enchente, 0.0, NivelSeveridade.Informativo,
                "Sem dados suficientes para inferência.",
                DateTime.UtcNow, DateTime.UtcNow.AddHours(24)));
        }

        double chuvaAcum = leituras.Sum(l => l.Indicadores.GetValueOrDefault("chuva_mm_h"));
        double nivelRio = leituras.Select(l => l.Indicadores.GetValueOrDefault("nivel_rio_m")).DefaultIfEmpty(0).Max();
        double inclMax = leituras.Select(l => l.Indicadores.GetValueOrDefault("inclinacao_graus")).DefaultIfEmpty(0).Max();
        double umidadeSolo = leituras.Select(l => l.Indicadores.GetValueOrDefault("umidade_solo_pct")).DefaultIfEmpty(50).Average();
        double tempSup = leituras.Select(l => l.Indicadores.GetValueOrDefault("temperatura_superficie_c")).DefaultIfEmpty(25).Average();
        double ndvi = leituras.Select(l => l.Indicadores.GetValueOrDefault("ndvi")).DefaultIfEmpty(0.4).Average();

        // Heurística de scoring multi-evento
        double scoreEnchente = Math.Clamp(chuvaAcum / 200.0 + nivelRio / 12.0, 0, 1);
        double scoreDeslizamento = Math.Clamp((inclMax / 45.0) * 0.6 + (umidadeSolo / 100.0) * 0.4, 0, 1);
        double scoreQueimada = Math.Clamp((tempSup / 50.0) * 0.5 + (1 - ndvi) * 0.5, 0, 1);
        double scoreSeca = Math.Clamp((1 - umidadeSolo / 100.0) * 0.7 + (tempSup / 50.0) * 0.3, 0, 1);

        var ranking = new (TipoEventoClimatico tipo, double score)[]
        {
            (TipoEventoClimatico.Enchente, scoreEnchente),
            (TipoEventoClimatico.Deslizamento, scoreDeslizamento),
            (TipoEventoClimatico.Queimada, scoreQueimada),
            (TipoEventoClimatico.Seca, scoreSeca)
        }.OrderByDescending(x => x.score).First();

        var severidade = ranking.score switch
        {
            >= 0.85 => NivelSeveridade.Critico,
            >= 0.65 => NivelSeveridade.Alto,
            >= 0.45 => NivelSeveridade.Moderado,
            >= 0.25 => NivelSeveridade.Baixo,
            _ => NivelSeveridade.Informativo
        };

        var justificativa =
            $"Indicadores chave: chuva_acum={chuvaAcum:F1}mm, nivel_rio={nivelRio:F1}m, " +
            $"inclinação_max={inclMax:F1}°, umidade_solo={umidadeSolo:F1}%, " +
            $"temp_sup={tempSup:F1}°C, NDVI={ndvi:F2}.";

        var previsao = new PrevisaoDTO(
            ranking.tipo,
            Math.Round(ranking.score, 3),
            severidade,
            justificativa,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(24));

        return Task.FromResult(previsao);
    }
}