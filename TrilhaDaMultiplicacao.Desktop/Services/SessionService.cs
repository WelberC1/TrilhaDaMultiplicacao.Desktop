using TrilhaDaMultiplicacao.Desktop.Models;

namespace TrilhaDaMultiplicacao.Desktop.Services;

public class SessionService : IProgressoRepository
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
    private readonly Dictionary<int, int> _pontosPorFase = new();
    private List<RankingEntrada>? _competidoresMock;

    public string? AlunoNome { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string AvatarEmoji { get; private set; } = "🦉";
    public int PontosTotais { get; private set; }

    public IReadOnlyDictionary<int, int> TodasEstrelas => _estrelasPorFase;

    public void EntrarComo(string nome)
    {
        AlunoNome = nome;

        if (_estrelasPorFase.Count == 0)
        {
            _estrelasPorFase[3] = 3;
            _pontosPorFase[3] = PontosDeEstrelas(3);
            PontosTotais = _pontosPorFase.Values.Sum();
        }

        _competidoresMock = null;
    }

    public void Sair()
    {
        AlunoNome = null;
        Email = string.Empty;
        AvatarEmoji = "🦉";
        PontosTotais = 0;
        _estrelasPorFase.Clear();
        _pontosPorFase.Clear();
        _competidoresMock = null;
    }

    public void AtualizarPerfil(string nome, string email, string avatarEmoji)
    {
        AlunoNome = nome;
        Email = email;
        AvatarEmoji = avatarEmoji;
    }

    public void RegistrarConclusaoFase(int numeroFase, int estrelas)
    {
        _estrelasPorFase[numeroFase] = Math.Max(estrelas, _estrelasPorFase.GetValueOrDefault(numeroFase));

        var pontos = PontosDeEstrelas(estrelas);
        _pontosPorFase[numeroFase] = Math.Max(pontos, _pontosPorFase.GetValueOrDefault(numeroFase));
        PontosTotais = _pontosPorFase.Values.Sum();
    }

    public int? EstrelasDaFase(int numeroFase) =>
        _estrelasPorFase.TryGetValue(numeroFase, out var estrelas) ? estrelas : null;

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

    private static int PontosDeEstrelas(int estrelas) => 20 + estrelas * 30;

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
}
