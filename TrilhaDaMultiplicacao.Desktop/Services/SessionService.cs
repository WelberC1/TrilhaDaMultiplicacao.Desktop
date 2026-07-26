using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Models.Api;

namespace TrilhaDaMultiplicacao.Desktop.Services;

public class SessionService(ApiClient api) : IProgressoRepository
{
    private readonly Dictionary<int, int> _estrelasPorFase = new();
    private string? _token;

    public string? AlunoNome { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string AvatarEmoji { get; private set; } = "🦉";
    public int PontosTotais { get; private set; }

    public IReadOnlyDictionary<int, int> TodasEstrelas => _estrelasPorFase;

    // -------- Autenticação e perfil: fala com a API de verdade --------

    public async Task LoginAsync(string nomeUsuario, string senha)
    {
        var resultado = await api.PostAsync<LoginRequest, AuthResponse>(
            "/api/auth/login", new LoginRequest(nomeUsuario, senha));

        AplicarSessao(resultado);
        await CarregarProgressoAsync();
    }

    public async Task RegistrarAsync(string nome, string nomeUsuario, string email, string senha)
    {
        await api.PostAsync<RegistrarRequest, AuthResponse>(
            "/api/auth/registrar", new RegistrarRequest(nome, nomeUsuario, email, senha));
    }

    public async Task EsqueciSenhaAsync(string email) =>
        await api.PostAsync("/api/auth/esqueci-senha", new EsqueciSenhaRequest(email));

    public async Task RedefinirSenhaAsync(string email, string codigo, string novaSenha) =>
        await api.PostAsync("/api/auth/redefinir-senha", new RedefinirSenhaRequest(email, codigo, novaSenha));

    public async Task AtualizarPerfilAsync(string nome, string email, string avatarEmoji)
    {
        var resultado = await api.PutAsync<AtualizarPerfilRequest, AlunoResponseDto>(
            "/api/alunos/me", new AtualizarPerfilRequest(nome, email, avatarEmoji), _token!);

        AplicarPerfil(resultado);
    }

    public async Task AlterarSenhaAsync(string senhaAtual, string novaSenha) =>
        await api.PutAsync("/api/alunos/me/senha", new AlterarSenhaRequest(senhaAtual, novaSenha), _token!);

    public async Task SairAsync()
    {
        try
        {
            if (_token is not null)
                await api.PostAsync("/api/auth/logout", _token);
        }
        catch (Exception)
        {
            // Best-effort de propósito: nada que dê errado ao avisar o servidor (rede fora,
            // token já revogado, etc.) deve impedir a sessão local de ser limpa — o pior caso
            // é o token continuar tecnicamente válido no servidor até expirar em até 24h.
        }

        _token = null;
        AlunoNome = null;
        Email = string.Empty;
        AvatarEmoji = "🦉";
        PontosTotais = 0;
        _estrelasPorFase.Clear();
    }

    // -------- Progresso: agora persiste de verdade na API --------

    public async Task RegistrarConclusaoFaseAsync(int numeroFase, int estrelas)
    {
        var resultado = await api.PostAsync<RegistrarConclusaoRequest, FaseProgressoResponseDto>(
            $"/api/progresso/fases/{numeroFase}", new RegistrarConclusaoRequest(estrelas), _token!);

        _estrelasPorFase[numeroFase] = resultado.Estrelas;

        var perfilAtualizado = await api.GetAsync<AlunoResponseDto>("/api/alunos/me", _token!);
        AplicarPerfil(perfilAtualizado);
    }

    public int? EstrelasDaFase(int numeroFase) =>
        _estrelasPorFase.TryGetValue(numeroFase, out var estrelas) ? estrelas : null;

    private async Task CarregarProgressoAsync()
    {
        var progresso = await api.GetAsync<List<FaseProgressoResponseDto>>("/api/progresso", _token!);

        _estrelasPorFase.Clear();
        foreach (var fase in progresso)
        {
            _estrelasPorFase[fase.NumeroFase] = fase.Estrelas;
        }
    }

    // -------- Ranking e conquistas: agora vêm de verdade da API --------

    public async Task<IReadOnlyList<RankingEntrada>> ObterRankingAsync()
    {
        var entradas = await api.GetAsync<List<RankingEntradaResponseDto>>("/api/ranking", _token!);

        return entradas.Select(e => new RankingEntrada
        {
            Posicao = e.Posicao,
            Nome = e.Nome,
            AvatarEmoji = e.AvatarEmoji,
            Pontos = e.Pontos,
            EhVoce = e.EhVoce
        }).ToList();
    }

    public async Task<IReadOnlyList<Conquista>> ObterConquistasAsync()
    {
        var conquistas = await api.GetAsync<List<ConquistaResponseDto>>("/api/conquistas", _token!);

        return conquistas.Select(c => new Conquista
        {
            Titulo = c.Titulo,
            Descricao = c.Descricao,
            Icone = c.Icone,
            Desbloqueada = c.Desbloqueada
        }).ToList();
    }

    private void AplicarSessao(AuthResponse resposta)
    {
        _token = resposta.Token;
        AplicarPerfil(resposta.Aluno);
    }

    private void AplicarPerfil(AlunoResponseDto aluno)
    {
        AlunoNome = aluno.Nome;
        Email = aluno.Email;
        AvatarEmoji = aluno.AvatarEmoji;
        PontosTotais = aluno.PontosTotais;
    }
}
