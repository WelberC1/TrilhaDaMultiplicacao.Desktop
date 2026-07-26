using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class RaciocinioViewModel : ViewModelBase
{
    private const int TotalPerguntas = 6;

    private static readonly (string Texto, bool Verdadeira)[] PoolPropriedades =
    [
        ("Multiplicar um número por 1 sempre dá o próprio número.", true),
        ("Multiplicar um número por 0 sempre dá 0.", true),
        ("Trocar a ordem dos fatores muda o resultado da multiplicação. Por exemplo, 3 × 4 é diferente de 4 × 3.", false),
        ("Multiplicar por 10 é só colocar um zero no final do número.", true),
        ("A multiplicação é uma forma rápida de somar parcelas iguais.", true),
        ("Multiplicar um número por 2 é o mesmo que somar esse número com ele mesmo.", true),
    ];

    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;
    private readonly Random _random = new();
    private readonly List<AfirmacaoLogica> _perguntas = [];

    private int _numeroFase;

    [ObservableProperty]
    public partial string TituloFase { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int IndicePergunta { get; set; }

    [ObservableProperty]
    public partial int Acertos { get; set; }

    [ObservableProperty]
    public partial bool MostrandoFeedback { get; set; }

    [ObservableProperty]
    public partial bool UltimaRespostaCorreta { get; set; }

    [ObservableProperty]
    public partial bool JogoConcluido { get; set; }

    [ObservableProperty]
    public partial int EstrelasConquistadas { get; set; }

    public AfirmacaoLogica PerguntaAtual => _perguntas[IndicePergunta];
    public string Afirmacao => PerguntaAtual.Texto;
    public int NumeroExibido => IndicePergunta + 1;
    public bool UltimaPergunta => IndicePergunta == TotalPerguntas - 1;
    public bool PodeResponder => !MostrandoFeedback;

    public string FeedbackTexto => UltimaRespostaCorreta
        ? "✅ Isso mesmo! Seu raciocínio está afiado! 🧩"
        : $"❌ Essa afirmação é {(PerguntaAtual.Verdadeira ? "VERDADEIRA" : "FALSA")}! Vamos com mais atenção na próxima. 💪";

    public bool MostrandoAcerto => MostrandoFeedback && UltimaRespostaCorreta;
    public bool MostrandoErro => MostrandoFeedback && !UltimaRespostaCorreta;

    public string TextoBotaoContinuar => UltimaPergunta ? "Ver resultado 🏆" : "Próxima afirmação →";
    public string Estrelinhas => new string('★', EstrelasConquistadas) + new string('☆', 3 - EstrelasConquistadas);

    partial void OnIndicePerguntaChanged(int value)
    {
        OnPropertyChanged(nameof(PerguntaAtual));
        OnPropertyChanged(nameof(Afirmacao));
        OnPropertyChanged(nameof(NumeroExibido));
        OnPropertyChanged(nameof(UltimaPergunta));
        OnPropertyChanged(nameof(TextoBotaoContinuar));
    }

    partial void OnMostrandoFeedbackChanged(bool value)
    {
        OnPropertyChanged(nameof(PodeResponder));
        OnPropertyChanged(nameof(MostrandoAcerto));
        OnPropertyChanged(nameof(MostrandoErro));
    }

    partial void OnUltimaRespostaCorretaChanged(bool value)
    {
        OnPropertyChanged(nameof(FeedbackTexto));
        OnPropertyChanged(nameof(MostrandoAcerto));
        OnPropertyChanged(nameof(MostrandoErro));
    }

    partial void OnEstrelasConquistadasChanged(int value) => OnPropertyChanged(nameof(Estrelinhas));

    public RaciocinioViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    public void Configurar(int numeroFase, string titulo)
    {
        _numeroFase = numeroFase;
        TituloFase = titulo;
        IndicePergunta = 0;
        Acertos = 0;
        JogoConcluido = false;
        MostrandoFeedback = false;
        EstrelasConquistadas = 0;
        _perguntas.Clear();
        _perguntas.AddRange(GerarAfirmacoes());
    }

    [RelayCommand]
    private void ResponderCerto() => Responder(true);

    [RelayCommand]
    private void ResponderErrado() => Responder(false);

    private void Responder(bool respostaCerto)
    {
        if (MostrandoFeedback) return;

        UltimaRespostaCorreta = respostaCerto == PerguntaAtual.Verdadeira;
        if (UltimaRespostaCorreta) Acertos++;
        MostrandoFeedback = true;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ContinuarAsync()
    {
        if (UltimaPergunta)
        {
            EstrelasConquistadas = Acertos switch
            {
                >= 6 => 3,
                >= 4 => 2,
                >= 2 => 1,
                _ => 0
            };

            try
            {
                await _session.RegistrarConclusaoFaseAsync(_numeroFase, EstrelasConquistadas);
            }
            catch (ApiRequestException ex)
            {
                ErrorMessage = ex.Message;
            }

            JogoConcluido = true;
            return;
        }

        MostrandoFeedback = false;
        IndicePergunta++;
    }

    [RelayCommand]
    private void VoltarTrilha() => _navigation.NavigateTo(_services.GetRequiredService<ShellViewModel>());

    private List<AfirmacaoLogica> GerarAfirmacoes()
    {
        var lista = new List<AfirmacaoLogica>();

        for (var i = 0; i < 3; i++) lista.Add(GerarEquacao());
        for (var i = 0; i < 2; i++) lista.Add(GerarComparacao());
        lista.Add(GerarPropriedade());

        return lista.OrderBy(_ => _random.Next()).ToList();
    }

    private AfirmacaoLogica GerarEquacao()
    {
        var a = _random.Next(2, 10);
        var b = _random.Next(2, 10);
        var correto = a * b;
        var verdadeira = _random.NextDouble() < 0.5;

        var valorMostrado = correto;
        if (!verdadeira)
        {
            int delta;
            do { delta = _random.Next(-6, 7); } while (delta == 0);
            valorMostrado = correto + delta;
        }

        return new AfirmacaoLogica
        {
            Texto = $"{a} × {b} = {valorMostrado}",
            Verdadeira = verdadeira
        };
    }

    private AfirmacaoLogica GerarComparacao()
    {
        int a1, b1, a2, b2, p1, p2;
        do
        {
            a1 = _random.Next(2, 10);
            b1 = _random.Next(2, 10);
            a2 = _random.Next(2, 10);
            b2 = _random.Next(2, 10);
            p1 = a1 * b1;
            p2 = a2 * b2;
        } while (p1 == p2);

        var afirmarMaior = _random.NextDouble() < 0.5;
        var relacao = afirmarMaior ? "maior" : "menor";
        var verdadeira = afirmarMaior ? p1 > p2 : p1 < p2;

        return new AfirmacaoLogica
        {
            Texto = $"{a1} × {b1} é {relacao} que {a2} × {b2}",
            Verdadeira = verdadeira
        };
    }

    private AfirmacaoLogica GerarPropriedade()
    {
        var (texto, verdadeira) = PoolPropriedades[_random.Next(PoolPropriedades.Length)];
        return new AfirmacaoLogica { Texto = texto, Verdadeira = verdadeira };
    }
}
