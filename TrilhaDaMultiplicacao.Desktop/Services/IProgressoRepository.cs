using TrilhaDaMultiplicacao.Desktop.Models;

namespace TrilhaDaMultiplicacao.Desktop.Services;

/// <summary>
/// Fronteira de persistência do progresso do aluno. Hoje só existe <see cref="SessionService"/>
/// como implementação em memória; quando a API real existir, uma implementação
/// que fale com o backend pode substituir o registro em App.axaml.cs sem alterar
/// nenhuma das telas que dependem desta interface.
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
    IReadOnlyList<RankingEntrada> ObterRanking();
}
