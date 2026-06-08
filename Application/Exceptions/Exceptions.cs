namespace OrbitGuardAI.Application.Exceptions;

/// <summary>
/// Exceção base do domínio — capturada pelo middleware global.
/// </summary>
public abstract class OrbitGuardException : Exception
{
    public int StatusCode { get; }
    protected OrbitGuardException(string mensagem, int statusCode) : base(mensagem) => StatusCode = statusCode;
}

public class RecursoNaoEncontradoException : OrbitGuardException
{
    public RecursoNaoEncontradoException(string recurso) : base($"{recurso} não encontrado.", 404) { }
}

public class DadosInvalidosException : OrbitGuardException
{
    public DadosInvalidosException(string msg) : base(msg, 400) { }
}

public class CredenciaisInvalidasException : OrbitGuardException
{
    public CredenciaisInvalidasException() : base("E-mail ou senha inválidos.", 401) { }
}

public class FalhaIntegracaoSateliteException : OrbitGuardException
{
    public FalhaIntegracaoSateliteException(string msg) : base($"Falha ao consultar dados orbitais: {msg}", 502) { }
}