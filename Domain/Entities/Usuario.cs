namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Usuário do sistema (cidadão, gestor público, defesa civil).
/// </summary>
public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Perfil { get; set; } = "Cidadao"; // Cidadao | Gestor | Admin
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}