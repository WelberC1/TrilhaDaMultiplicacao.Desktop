using TrilhaDaMultiplicacao.Desktop.Models;

namespace TrilhaDaMultiplicacao.Desktop.Services;

/// <summary>
/// Fronteira de persistência do progresso do aluno. Implementada por <see cref="SessionService"/>,
/// que fala com a API real (não é mais mock em memória) — mas as telas dependem desta interface,
/// não da classe concreta, então uma implementação alternativa poderia substituir o registro em
/// App.axaml.cs sem alterar nenhuma tela.
/// </summary>
public interface IProgressoRepository
{
    string? AlunoNome { get; }
    string Email { get; }
    string AvatarEmoji { get; }
    int PontosTotais { get; }
    IReadOnlyDictionary<int, int> TodasEstrelas { get; }

    Task AtualizarPerfilAsync(string nome, string email, string avatarEmoji);
    Task AlterarSenhaAsync(string senhaAtual, string novaSenha);
    Task<IReadOnlyList<RankingEntrada>> ObterRankingAsync();
    Task<IReadOnlyList<Conquista>> ObterConquistasAsync();
}
