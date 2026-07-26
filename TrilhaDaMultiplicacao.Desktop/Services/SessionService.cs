using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Models.Api;

namespace TrilhaDaMultiplicacao.Desktop.Services;

public class SessionService(ApiClient api) : IProgressoRepository
{
    private static readonly string[] NomesMockRanking =
    [
        "Larissa", "Pedro", "Beatriz", "Gabriel", "Sofia", "Lucas",
        "Isabela", "Miguel", "Alice", "Davi", "Laura", "Enzo", "Manuela", "Heitor"
    ];

    private static readonly string[] AvataresMockRanking =
        ["🦊", "🐱", "🐶", "🐼", "🦁", "🐯", "🐰", "🐵", "🐨", "🦄", "🐸", "🐷", "🐔", "🦋"];

    private readonly Random _random = new();
    private readonly Dictionary<int, int> _estrelasPorFase = new();
    private List<RankingEntrada>? _competidoresMock;
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
        _competidoresMock = null;
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

    // -------- Ranking: continua mockado até a próxima fase --------

    public IReadOnlyList<RankingEntrada> ObterRanking()
    {
        _competidoresMock ??= GerarCompetidoresMock();

        var entradas = _competidoresMock
            .Select(c => (Nome: c.Nome, AvatarEmoji: c.AvatarEmoji, Pontos: c.Pontos, EhVoce: false))
            .Append((Nome: AlunoNome ?? "Você", AvatarEmoji, Pontos: PontosTotais, EhVoce: true))
            .OrderByDescending(e => e.Pontos)
            .Select((e, indice) => new RankingEntrada
            {
                Posicao = indice + 1,
                Nome = e.Nome,
                AvatarEmoji = e.AvatarEmoji,
                Pontos = e.Pontos,
                EhVoce = e.EhVoce
            })
            .ToList();

        return entradas;
    }

    private List<RankingEntrada> GerarCompetidoresMock()
    {
        var nomes = NomesMockRanking.OrderBy(_ => _random.Next()).Take(9).ToList();

        return nomes.Select(nome => new RankingEntrada
        {
            Posicao = 0,
            Nome = nome,
            AvatarEmoji = AvataresMockRanking[_random.Next(AvataresMockRanking.Length)],
            Pontos = _random.Next(20, 620)
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
