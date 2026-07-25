using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class InterpretacaoViewModel : ViewModelBase
{
    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;
    private readonly List<PerguntaInterpretacao> _perguntas;

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

    public PerguntaInterpretacao PerguntaAtual => _perguntas[IndicePergunta];
    public string Enunciado => PerguntaAtual.Enunciado;
    public string[] Opcoes => PerguntaAtual.Opcoes;
    public int TotalPerguntas => _perguntas.Count;
    public int NumeroExibido => IndicePergunta + 1;
    public bool UltimaPergunta => IndicePergunta == TotalPerguntas - 1;
    public bool PodeResponder => !MostrandoFeedback;

    public string FeedbackTexto => UltimaRespostaCorreta
        ? "✅ Isso mesmo! Mandou bem! 🎉"
        : $"❌ Quase! A resposta certa era {PerguntaAtual.Opcoes[PerguntaAtual.RespostaCorreta]}.";

    public string TextoBotaoContinuar => UltimaPergunta ? "Ver resultado 🏆" : "Próxima pergunta →";

    public bool MostrandoAcerto => MostrandoFeedback && UltimaRespostaCorreta;
    public bool MostrandoErro => MostrandoFeedback && !UltimaRespostaCorreta;

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

    public InterpretacaoViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
        _perguntas = CriarPerguntas();
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

    [RelayCommand]
    private void Continuar()
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
            _session.RegistrarConclusaoFase(_numeroFase, EstrelasConquistadas);
            JogoConcluido = true;
            return;
        }

        MostrandoFeedback = false;
        IndicePergunta++;
    }

    [RelayCommand]
    private void VoltarTrilha() => _navigation.NavigateTo(_services.GetRequiredService<TrilhaViewModel>());

    private static List<PerguntaInterpretacao> CriarPerguntas() =>
    [
        new PerguntaInterpretacao
        {
            Enunciado = "Joãozinho tem 4 caixas de lápis. Cada caixa tem 6 lápis. Quantos lápis ele tem ao todo?",
            Opcoes = ["18", "20", "24", "10"],
            RespostaCorreta = 2
        },
        new PerguntaInterpretacao
        {
            Enunciado = "Na horta da escola, cada canteiro tem 5 pés de alface. São 3 canteiros. Quantos pés de alface há ao todo?",
            Opcoes = ["8", "12", "20", "15"],
            RespostaCorreta = 3
        },
        new PerguntaInterpretacao
        {
            Enunciado = "Joãozinho comprou 3 pacotes de figurinhas. Cada pacote tem 8 figurinhas. Quantas figurinhas ele comprou?",
            Opcoes = ["20", "22", "24", "26"],
            RespostaCorreta = 2
        },
        new PerguntaInterpretacao
        {
            Enunciado = "Cada fileira do cinema tem 9 cadeiras. Se há 4 fileiras, quantas cadeiras há no total?",
            Opcoes = ["13", "32", "40", "36"],
            RespostaCorreta = 3
        },
        new PerguntaInterpretacao
        {
            Enunciado = "No parquinho, tem 6 balanços. Cada balanço aguenta 2 crianças por vez. Quantas crianças podem balançar ao mesmo tempo?",
            Opcoes = ["8", "10", "12", "14"],
            RespostaCorreta = 2
        }
    ];
}
