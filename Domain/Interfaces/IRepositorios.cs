using OrbitGuardAI.Domain.Entities;

namespace OrbitGuardAI.Domain.Interfaces;

public interface IRepositorio<T> where T : class
{
    Task<IEnumerable<T>> ListarAsync();
    Task<T?> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task RemoverAsync(Guid id);
}

public interface IAreaRiscoRepositorio : IRepositorio<AreaRisco> { }
public interface IAlertaRepositorio : IRepositorio<Alerta>
{
    Task<IEnumerable<Alerta>> ListarAtivosAsync();
}
public interface ILeituraRepositorio : IRepositorio<LeituraTelemetrica>
{
    Task<IEnumerable<LeituraTelemetrica>> ListarPorDispositivoAsync(Guid dispositivoId, DateTime desde);
}
public interface IUsuarioRepositorio : IRepositorio<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
}