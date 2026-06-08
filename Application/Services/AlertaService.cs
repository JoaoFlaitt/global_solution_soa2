using OrbitGuardAI.Application.DTOs;
using OrbitGuardAI.Application.Exceptions;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Enums;
using OrbitGuardAI.Domain.Interfaces;

namespace OrbitGuardAI.Application.Services;

public class AlertaService : IAlertaService
{
    private readonly IAlertaRepositorio _repo;
    private readonly INotificacaoService _notificacao;

    public AlertaService(IAlertaRepositorio repo, INotificacaoService notificacao)
    {
        _repo = repo;
        _notificacao = notificacao;
    }

    public async Task<Alerta> GerarAlertaAsync(AreaRisco area, PrevisaoDTO previsao)
    {
        if (area is null) throw new ArgumentNullException(nameof(area));
        if (previsao is null) throw new ArgumentNullException(nameof(previsao));

        var alerta = new Alerta
        {
            AreaRiscoId = area.Id,
            Tipo = previsao.TipoEvento,
            Severidade = previsao.Severidade,
            Probabilidade = previsao.Probabilidade,
            Titulo = $"{previsao.Severidade} | {previsao.TipoEvento} em {area.Municipio}/{area.Estado}",
            Descricao = previsao.Justificativa,
            DataEmissao = DateTime.UtcNow,
            DataExpiracao = previsao.JanelaPrevistaFim,
            RecomendacaoAcao = ObterRecomendacao(previsao.TipoEvento, previsao.Severidade)
        };

        await _repo.AdicionarAsync(alerta);

        // Dispara notificação somente para severidades a partir de Moderado
        if (alerta.Severidade >= NivelSeveridade.Moderado)
        {
            await _notificacao.EnviarAsync(alerta, "push");
            await _notificacao.EnviarAsync(alerta, "sms");
        }

        return alerta;
    }

    public async Task<IEnumerable<Alerta>> ListarAtivosAsync() => await _repo.ListarAtivosAsync();

    public async Task EncerrarAsync(Guid alertaId)
    {
        var alerta = await _repo.ObterPorIdAsync(alertaId)
            ?? throw new RecursoNaoEncontradoException("Alerta");
        alerta.Encerrar();
        await _repo.AtualizarAsync(alerta);
    }

    private static string ObterRecomendacao(TipoEventoClimatico tipo, NivelSeveridade sev) => (tipo, sev) switch
    {
        (TipoEventoClimatico.Enchente, >= NivelSeveridade.Alto) =>
            "Evacuar áreas baixas, acionar Defesa Civil (199), levar documentos.",
        (TipoEventoClimatico.Deslizamento, >= NivelSeveridade.Alto) =>
            "Sair imediatamente de encostas, acionar Defesa Civil, observar trincas.",
        (TipoEventoClimatico.Queimada, >= NivelSeveridade.Alto) =>
            "Acionar Brigadas e Bombeiros (193), criar aceiros, evitar uso de fogo.",
        (TipoEventoClimatico.Seca, >= NivelSeveridade.Alto) =>
            "Restringir uso de água, ativar plano de contingência hídrica.",
        _ => "Monitorar a situação. Mantenha-se informado pelos canais oficiais."
    };
}