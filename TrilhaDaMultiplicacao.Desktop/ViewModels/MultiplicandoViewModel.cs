using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class MultiplicandoViewModel : ViewModelBase
{
    private const int TotalPerguntas = 5;

    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;
    private readonly Random _random = new();
    private readonly List<PerguntaMultiplicando> _perguntas = [];

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

    public PerguntaMultiplicando PerguntaAtual => _perguntas[IndicePergunta];

    public string Enunciado => PerguntaAtual.EsconderPrimeiro
        ? $"? × {PerguntaAtual.FatorB} = {PerguntaAtual.Produto}"
        : $"{PerguntaAtual.FatorA} × ? = {PerguntaAtual.Produto}";

    public string[] Opcoes => PerguntaAtual.Opcoes;
    public int NumeroExibido => IndicePergunta + 1;
    public bool UltimaPergunta => IndicePergunta == TotalPerguntas - 1;
    public bool PodeResponder => !MostrandoFeedback;

    public string FeedbackTexto => UltimaRespostaCorreta
        ? "✅ Isso mesmo! Você descobriu o número escondido! 🔍"
        : $"❌ Quase! O número que faltava era {PerguntaAtual.Opcoes[PerguntaAtual.RespostaCorreta]}.";

    public bool MostrandoAcerto => MostrandoFeedback && UltimaRespostaCorreta;
    public bool MostrandoErro => MostrandoFeedback && !UltimaRespostaCorreta;

    public string TextoBotaoContinuar => UltimaPergunta ? "Ver resultado 🏆" : "Próximo mistério →";
    public string Estrelinhas => new string('★', EstrelasConquistadas) + new string('☆', 3 - EstrelasConquistadas);

    partial void OnIndicePerguntaChanged(int value)
    {
        OnPropertyChanged(nameof(PerguntaAtual));
        OnPropertyChanged(nameof(Enunciado));
        OnPropertyChanged(nameof(Opcoes));
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

    public MultiplicandoViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    public void Configurar(int numeroFase, string titulo)
    {
        _numeroFase = numeroFase;
        TituloFase = titulo;

        _perguntas.Clear();
        for (var i = 0; i < TotalPerguntas; i++) _perguntas.Add(GerarPergunta());

        IndicePergunta = 0;
        Acertos = 0;
        JogoConcluido = false;
        MostrandoFeedback = false;
        EstrelasConquistadas = 0;
    }

    [RelayCommand]
    private void Responder(string opcao)
    {
        if (MostrandoFeedback) return;

        var indiceEscolhido = Array.IndexOf(PerguntaAtual.Opcoes, opcao);
        UltimaRespostaCorreta = indiceEscolhido == PerguntaAtual.RespostaCorreta;
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
                >= 5 => 3,
                >= 3 => 2,
                >= 1 => 1,
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

    private PerguntaMultiplicando GerarPergunta()
    {
        var a = _random.Next(2, 10);
        var b = _random.Next(2, 10);
        var produto = a * b;
        var esconderPrimeiro = _random.NextDouble() < 0.5;
        var faltante = esconderPrimeiro ? a : b;

        var opcoes = new HashSet<int> { faltante };
        while (opcoes.Count < 4)
        {
            opcoes.Add(_random.Next(2, 10));
        }

        var embaralhadas = opcoes.OrderBy(_ => _random.Next()).ToArray();
        var indiceCorreto = Array.IndexOf(embaralhadas, faltante);

        return new PerguntaMultiplicando
        {
            FatorA = a,
            FatorB = b,
            EsconderPrimeiro = esconderPrimeiro,
            Produto = produto,
            Opcoes = embaralhadas.Select(n => n.ToString()).ToArray(),
            RespostaCorreta = indiceCorreto
        };
    }
}
