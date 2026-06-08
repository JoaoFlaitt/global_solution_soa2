using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrbitGuardAI.Application.DTOs;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Infrastructure.Data;

namespace OrbitGuardAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DispositivosController : ControllerBase
{
    private readonly AppDbContext _ctx;
    public DispositivosController(AppDbContext ctx) => _ctx = ctx;

    [HttpGet("satelites")]
    public async Task<IEnumerable<Satelite>> ListarSatelites() => await _ctx.Satelites.ToListAsync();

    [HttpGet("sensores")]
    public async Task<IEnumerable<SensorIoT>> ListarSensores() => await _ctx.Sensores.ToListAsync();

    [HttpPost("satelites")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Satelite>> AdicionarSatelite([FromBody] SateliteDTO dto)
    {
        var s = new Satelite(dto.Nome, dto.Operadora, dto.AltitudeKm, dto.TipoOrbita);
        _ctx.Satelites.Add(s);
        await _ctx.SaveChangesAsync();
        return Ok(s);
    }

    [HttpPost("sensores")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SensorIoT>> AdicionarSensor([FromBody] SensorDTO dto)
    {
        var s = new SensorIoT(dto.Nome, dto.Tipo, dto.Latitude, dto.Longitude, dto.Regiao);
        _ctx.Sensores.Add(s);
        await _ctx.SaveChangesAsync();
        return Ok(s);
    }

    /// <summary>Health-check polimórfico — usa Dispositivo.EstaSaudavel().</summary>
    [HttpGet("saude")]
    public async Task<object> SaudeFrota()
    {
        var sat = await _ctx.Satelites.ToListAsync();
        var sen = await _ctx.Sensores.ToListAsync();
        var todos = sat.Cast<Dispositivo>().Concat(sen).ToList();
        return new
        {
            total = todos.Count,
            saudaveis = todos.Count(d => d.EstaSaudavel()),
            instanciadosNaSessao = Dispositivo.TotalInstanciados,
            timestamp = DateTime.UtcNow
        };
    }
}