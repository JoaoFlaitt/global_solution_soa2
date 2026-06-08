using OrbitGuardAI.Domain.Enums;

namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Classe abstrata base para qualquer dispositivo coletor de dados (satélite, sensor, drone).
/// Demonstra Abstração + Herança + Polimorfismo.
/// </summary>
public abstract class Dispositivo
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string Nome { get; protected set; } = string.Empty;
    public StatusDispositivo Status { get; protected set; } = StatusDispositivo.Ativo;
    public DateTime DataCadastro { get; protected set; } = DateTime.UtcNow;
    public DateTime? UltimaLeitura { get; protected set; }

    protected Dispositivo() { }

    protected Dispositivo(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do dispositivo é obrigatório.", nameof(nome));
        Nome = nome;
    }

    /// <summary>
    /// Cada subclasse define como coleta seus dados (polimorfismo).
    /// </summary>
    public abstract LeituraTelemetrica Coletar();

    /// <summary>
    /// Health-check padrão — pode ser sobrescrito.
    /// </summary>
    public virtual bool EstaSaudavel() =>
        Status == StatusDispositivo.Ativo &&
        UltimaLeitura.HasValue &&
        (DateTime.UtcNow - UltimaLeitura.Value).TotalMinutes < 30;

    public void AtualizarStatus(StatusDispositivo novo) => Status = novo;

    /// <summary>
    /// Contador estático global de dispositivos instanciados (uso de membro estático).
    /// </summary>
    public static int TotalInstanciados { get; private set; }

    public static void RegistrarInstancia() => TotalInstanciados++;
}