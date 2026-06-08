using Microsoft.EntityFrameworkCore;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Interfaces;

namespace OrbitGuardAI.Infrastructure.Data;

public abstract class RepositorioBase<T> : IRepositorio<T> where T : class
{
    protected readonly AppDbContext _ctx;
    protected RepositorioBase(AppDbContext ctx) => _ctx = ctx;

    public virtual async Task<IEnumerable<T>> ListarAsync() => await _ctx.Set<T>().ToListAsync();
    public virtual async Task<T?> ObterPorIdAsync(Guid id) => await _ctx.Set<T>().FindAsync(id);

    public virtual async Task AdicionarAsync(T entidade)
    {
        _ctx.Set<T>().Add(entidade);
        await _ctx.SaveChangesAsync();
    }

    public virtual async Task AtualizarAsync(T entidade)
    {
        _ctx.Set<T>().Update(entidade);
        await _ctx.SaveChangesAsync();
    }

    public virtual async Task RemoverAsync(Guid id)
    {
        var ent = await ObterPorIdAsync(id);
        if (ent != null)
        {
            _ctx.Set<T>().Remove(ent);
            await _ctx.SaveChangesAsync();
        }
    }
}

public class AreaRiscoRepositorio : RepositorioBase<AreaRisco>, IAreaRiscoRepositorio
{
    public AreaRiscoRepositorio(AppDbContext ctx) : base(ctx) { }
}

public class AlertaRepositorio : RepositorioBase<Alerta>, IAlertaRepositorio
{
    public AlertaRepositorio(AppDbContext ctx) : base(ctx) { }
    public async Task<IEnumerable<Alerta>> ListarAtivosAsync() =>
        await _ctx.Alertas.Where(a => a.Ativo).OrderByDescending(a => a.DataEmissao).ToListAsync();
}

public class LeituraRepositorio : RepositorioBase<LeituraTelemetrica>, ILeituraRepositorio
{
    public LeituraRepositorio(AppDbContext ctx) : base(ctx) { }
    public async Task<IEnumerable<LeituraTelemetrica>> ListarPorDispositivoAsync(Guid dispositivoId, DateTime desde) =>
        await _ctx.Leituras
            .Where(l => l.DispositivoId == dispositivoId && l.DataHora >= desde)
            .OrderBy(l => l.DataHora).ToListAsync();
}

public class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
{
    public UsuarioRepositorio(AppDbContext ctx) : base(ctx) { }
    public async Task<Usuario?> ObterPorEmailAsync(string email) =>
        await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
}