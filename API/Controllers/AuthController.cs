using Microsoft.AspNetCore.Mvc;
using OrbitGuardAI.Application.DTOs;
using OrbitGuardAI.Application.Exceptions;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Interfaces;
using OrbitGuardAI.Infrastructure.Auth;

namespace OrbitGuardAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepositorio _repo;
    private readonly ITokenService _tokenSvc;

    public AuthController(IUsuarioRepositorio repo, ITokenService tokenSvc)
    {
        _repo = repo;
        _tokenSvc = tokenSvc;
    }

    /// <summary>Registra um novo usuário (cidadão, gestor ou admin).</summary>
    [HttpPost("registrar")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<TokenDTO>> Registrar([FromBody] RegistroUsuarioDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            throw new DadosInvalidosException("Email e senha são obrigatórios.");

        var existente = await _repo.ObterPorEmailAsync(dto.Email);
        if (existente != null) throw new DadosInvalidosException("E-mail já cadastrado.");

        var u = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = TokenService.HashSenha(dto.Senha),
            Perfil = dto.Perfil
        };
        await _repo.AdicionarAsync(u);
        var token = _tokenSvc.GerarToken(u);
        return Ok(new TokenDTO(token, DateTime.UtcNow.AddHours(8), u.Perfil));
    }

    /// <summary>Realiza login e devolve um JWT.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<TokenDTO>> Login([FromBody] LoginDTO dto)
    {
        var u = await _repo.ObterPorEmailAsync(dto.Email)
            ?? throw new CredenciaisInvalidasException();
        if (u.SenhaHash != TokenService.HashSenha(dto.Senha))
            throw new CredenciaisInvalidasException();

        var token = _tokenSvc.GerarToken(u);
        return Ok(new TokenDTO(token, DateTime.UtcNow.AddHours(8), u.Perfil));
    }
}