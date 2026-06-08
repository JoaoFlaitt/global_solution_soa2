using Microsoft.EntityFrameworkCore;
using OrbitGuardAI.Domain.Entities;

namespace OrbitGuardAI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AreaRisco> AreasRisco => Set<AreaRisco>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<LeituraTelemetrica> Leituras => Set<LeituraTelemetrica>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Satelite> Satelites => Set<Satelite>();
    public DbSet<SensorIoT> Sensores => Set<SensorIoT>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<LeituraTelemetrica>().Ignore(l => l.Indicadores);
        mb.Entity<LeituraTelemetrica>().Property(l => l.IndicadoresJson).HasColumnName("Indicadores");

        mb.Entity<Satelite>().Ignore(s => s.SensoresEmbarcados);
        mb.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
    }
}