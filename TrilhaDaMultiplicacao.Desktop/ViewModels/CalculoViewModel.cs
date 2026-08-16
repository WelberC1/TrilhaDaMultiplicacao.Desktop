using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class CalculoViewModel : ViewModelBase
{
    private const double TempoPorPergunta = 6.0;
    private const double IntervaloTick = 0.1;
    private const int TotalPerguntas = 5;

    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;
    private readonly DispatcherTimer _timer;
    private readonly List<PerguntaCalculo> _perguntas = [];
    private readonly Random _random = new();

    private int _numeroFase;

    [ObservableProperty]
    public partial string TituloFase { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int IndicePergunta { get; set; }

    [ObservableProperty]
    public partial int Acertos { get; set; }

    [ObservableProperty]
    public partial double TempoRestante { get; set; }

    [ObservableProperty]
    public partial bool MostrandoFeedback { get; set; }

    [ObservableProperty]
    public partial bool UltimaRespostaCorreta { get; set; }

    [ObservableProperty]
    public partial bool Esgotou { get; set; }

    [ObservableProperty]
    public partial bool JogoConcluido { get; set; }

    [ObservableProperty]
    public partial int EstrelasConquistadas { get; set; }

    public PerguntaCalculo PerguntaAtual => _perguntas[IndicePergunta];
    public string Enunciado => $"{PerguntaAtual.FatorA} × {PerguntaAtual.FatorB}";
    public string[] Opcoes => PerguntaAtual.Opcoes;
    public int NumeroExibido => IndicePergunta + 1;
    public bool UltimaPergunta => IndicePergunta == TotalPerguntas - 1;
    public bool PodeResponder => !MostrandoFeedback;

    public string FeedbackTexto => Esgotou
        ? $"⏰ Tempo esgotado! A resposta certa era {PerguntaAtual.Opcoes[PerguntaAtual.RespostaCorreta]}."
        : UltimaRespostaCorreta
            ? "✅ Isso mesmo! Rapidinho! 🎉"
            : $"❌ Quase! A resposta certa era {PerguntaAtual.Opcoes[PerguntaAtual.RespostaCorreta]}.";

    public string TextoBotaoContinuar => UltimaPergunta ? "Ver resultado 🏆" : "Próxima conta →";

    public bool MostrandoAcerto => MostrandoFeedback && UltimaRespostaCorreta && !Esgotou;
    public bool MostrandoErro => MostrandoFeedback && (!UltimaRespostaCorreta || Esgotou);

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

    partial void OnEsgotouChanged(bool value)
    {
        OnPropertyChanged(nameof(FeedbackTexto));
        OnPropertyChanged(nameof(MostrandoAcerto));
        OnPropertyChanged(nameof(MostrandoErro));
    }

    partial void OnEstrelasConquistadasChanged(int value) => OnPropertyChanged(nameof(Estrelinhas));

    public CalculoViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(IntervaloTick) };
        _timer.Tick += (_, _) => AoTick();
    }

    public void Configurar(int numeroFase, string titulo)
    {
        _numeroFase = numeroFase;
        TituloFase = titulo;

        _perguntas.Clear();
        for (var i = 0; i < TotalPerguntas; i++)
        {
            _perguntas.Add(GerarPergunta());
        }

        IndicePergunta = 0;
        Acertos = 0;
        JogoConcluido = false;
        MostrandoFeedback = false;
        EstrelasConquistadas = 0;

        IniciarPergunta();
    }

    private void IniciarPergunta()
    {
        Esgotou = false;
        MostrandoFeedback = false;
        TempoRestante = TempoPorPergunta;
        _timer.Start();
    }

    private void AoTick()
    {
        if (MostrandoFeedback) return;

        TempoRestante = Math.Max(0, TempoRestante - IntervaloTick);
        if (TempoRestante <= 0)
        {
            _timer.Stop();
            Esgotou = true;
            UltimaRespostaCorreta = false;
            MostrandoFeedback = true;
        }
    }

    [RelayCommand]
    private void Responder(string opcao)
    {
        if (MostrandoFeedback) return;

        _timer.Stop();
        var indiceEscolhido = Array.IndexOf(PerguntaAtual.Opcoes, opcao);
        UltimaRespostaCorreta = indiceEscolhido == PerguntaAtual.RespostaCorreta;
        Esgotou = false;
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

        IndicePergunta++;
        IniciarPergunta();
    }

    [RelayCommand]
    private void VoltarTrilha()
    {
        _timer.Stop();
        _navigation.NavigateTo(_services.GetRequiredService<ShellViewModel>());
    }

    private PerguntaCalculo GerarPergunta()
    {
        // Fase 2 "Cálculo Rápido": só tabuada (1 dígito). Fase 7 "Operações com 2 Dígitos"
        // e fase 11 "Operações com 3 Dígitos" precisam de um fator com essa quantidade de
        // dígitos de verdade, senão a fase não corresponde ao próprio nome.
        var (minFatorA, maxFatorA) = _numeroFase switch
        {
            7 => (10, 99),
            11 => (100, 999),
            _ => (2, 9)
        };

        var a = _random.Next(minFatorA, maxFatorA + 1);
        var b = _random.Next(2, 10);
        var correto = a * b;

        var opcoes = new HashSet<int> { correto };
        while (opcoes.Count < 4)
        {
            var delta = _random.Next(-8, 9);
            var candidato = correto + delta;
            if (candidato > 0) opcoes.Add(candidato);
        }

        var opcoesEmbaralhadas = opcoes.OrderBy(_ => _random.Next()).ToArray();
        var indiceCorreto = Array.IndexOf(opcoesEmbaralhadas, correto);

        return new PerguntaCalculo
        {
            FatorA = a,
            FatorB = b,
            Opcoes = opcoesEmbaralhadas.Select(n => n.ToString()).ToArray(),
            RespostaCorreta = indiceCorreto
        };
    }
}
