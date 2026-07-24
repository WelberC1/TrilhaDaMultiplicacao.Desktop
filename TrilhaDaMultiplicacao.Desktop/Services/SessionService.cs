namespace TrilhaDaMultiplicacao.Desktop.Services;

public class SessionService
{
    public string? AlunoNome { get; private set; }

    public void EntrarComo(string nome) => AlunoNome = nome;

    public void Sair() => AlunoNome = null;
}
