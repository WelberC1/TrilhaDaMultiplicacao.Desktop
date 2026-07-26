using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class RetanguloViewModel : ViewModelBase
{
    private const int TotalPerguntas = 5;
    private const int TamanhoMaximo = 9;

    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;
    private readonly Random _random = new();
    private readonly List<PerguntaRetangulo> _perguntas = [];

    private int _numeroFase;
    private int _cliquesNaRodada;

    [ObservableProperty]
    public partial string TituloFase { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int IndicePergunta { get; set; }

    [ObservableProperty]
    public partial int Acertos { get; set; }

    [ObservableProperty]
    public partial int LinhasAtuais { get; set; } = 1;

    [ObservableProperty]
    public partial int ColunasAtuais { get; set; } = 1;

    [ObservableProperty]
    public partial bool Resolvido { get; set; }

    [ObservableProperty]
    public partial bool JogoConcluido { get; set; }

    [ObservableProperty]
    public partial int EstrelasConquistadas { get; set; }

    public PerguntaRetangulo PerguntaAtual => _perguntas[IndicePergunta];
    public int LinhasAlvo => PerguntaAtual.Linhas;
    public int ColunasAlvo => PerguntaAtual.Colunas;
    public string IconeAtual => PerguntaAtual.Icone;
    public int NumeroExibido => IndicePergunta + 1;
    public bool UltimaPergunta => IndicePergunta == TotalPerguntas - 1;

    public int TotalAtual => LinhasAtuais * ColunasAtuais;
    public int TotalAlvo => LinhasAlvo * ColunasAlvo;

    public IEnumerable<int> Celulas => Enumerable.Range(0, TotalAtual);

    public bool PodeDiminuirLinhas => LinhasAtuais > 1 && !Resolvido;
    public bool PodeAumentarLinhas => LinhasAtuais < TamanhoMaximo && !Resolvido;
    public bool PodeDiminuirColunas => ColunasAtuais > 1 && !Resolvido;
    public bool PodeAumentarColunas => ColunasAtuais < TamanhoMaximo && !Resolvido;

    public string TextoDesafio => $"Monte {LinhasAlvo} × {ColunasAlvo}";
    public string TextoSucesso => $"🎉 Isso mesmo! {LinhasAlvo} × {ColunasAlvo} = {TotalAlvo}";
    public string TextoBotaoContinuar => UltimaPergunta ? "Ver resultado 🏆" : "Próximo desafio →";
    public string Estrelinhas => new string('★', EstrelasConquistadas) + new string('☆', 3 - EstrelasConquistadas);

    partial void OnIndicePerguntaChanged(int value)
    {
        OnPropertyChanged(nameof(PerguntaAtual));
        OnPropertyChanged(nameof(LinhasAlvo));
        OnPropertyChanged(nameof(ColunasAlvo));
        OnPropertyChanged(nameof(IconeAtual));
        OnPropertyChanged(nameof(NumeroExibido));
        OnPropertyChanged(nameof(UltimaPergunta));
        OnPropertyChanged(nameof(TotalAlvo));
        OnPropertyChanged(nameof(TextoDesafio));
        OnPropertyChanged(nameof(TextoSucesso));
        OnPropertyChanged(nameof(TextoBotaoContinuar));
    }

    partial void OnLinhasAtuaisChanged(int value)
    {
        OnPropertyChanged(nameof(TotalAtual));
        OnPropertyChanged(nameof(Celulas));
        OnPropertyChanged(nameof(PodeDiminuirLinhas));
        OnPropertyChanged(nameof(PodeAumentarLinhas));
        ValidarResposta();
    }

    partial void OnColunasAtuaisChanged(int value)
    {
        OnPropertyChanged(nameof(TotalAtual));
        OnPropertyChanged(nameof(Celulas));
        OnPropertyChanged(nameof(PodeDiminuirColunas));
        OnPropertyChanged(nameof(PodeAumentarColunas));
        ValidarResposta();
    }

    partial void OnResolvidoChanged(bool value)
    {
        OnPropertyChanged(nameof(PodeDiminuirLinhas));
        OnPropertyChanged(nameof(PodeAumentarLinhas));
        OnPropertyChanged(nameof(PodeDiminuirColunas));
        OnPropertyChanged(nameof(PodeAumentarColunas));
    }

    partial void OnEstrelasConquistadasChanged(int value) => OnPropertyChanged(nameof(Estrelinhas));

    public RetanguloViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    public void Configurar(int numeroFase, string titulo)
    {
        _numeroFase = numeroFase;
        TituloFase = titulo;

        var icones = new[] { "🍬", "⭐", "🍎", "🎈", "🧁" };
        _perguntas.Clear();
        for (var i = 0; i < TotalPerguntas; i++)
        {
            _perguntas.Add(new PerguntaRetangulo
            {
                Linhas = _random.Next(2, 7),
                Colunas = _random.Next(2, 7),
                Icone = icones[i % icones.Length]
            });
        }

        IndicePergunta = 0;
        Acertos = 0;
        JogoConcluido = false;
        EstrelasConquistadas = 0;
        IniciarRodada();
    }

    private void IniciarRodada()
    {
        _cliquesNaRodada = 0;
        Resolvido = false;
        LinhasAtuais = 1;
        ColunasAtuais = 1;
    }

    [RelayCommand]
    private void AumentarLinhas()
    {
        if (!PodeAumentarLinhas) return;
        _cliquesNaRodada++;
        LinhasAtuais++;
    }

    [RelayCommand]
    private void DiminuirLinhas()
    {
        if (!PodeDiminuirLinhas) return;
        _cliquesNaRodada++;
        LinhasAtuais--;
    }

    [RelayCommand]
    private void AumentarColunas()
    {
        if (!PodeAumentarColunas) return;
        _cliquesNaRodada++;
        ColunasAtuais++;
    }

    [RelayCommand]
    private void DiminuirColunas()
    {
        if (!PodeDiminuirColunas) return;
        _cliquesNaRodada++;
        ColunasAtuais--;
    }

    private void ValidarResposta()
    {
        if (Resolvido) return;

        var bateu = (LinhasAtuais == LinhasAlvo && ColunasAtuais == ColunasAlvo) ||
                    (LinhasAtuais == ColunasAlvo && ColunasAtuais == LinhasAlvo);
        if (!bateu) return;

        Resolvido = true;
        var minimo = LinhasAlvo - 1 + (ColunasAlvo - 1);
        if (_cliquesNaRodada <= minimo) Acertos++;
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
        IniciarRodada();
    }

    [RelayCommand]
    private void VoltarTrilha() => _navigation.NavigateTo(_services.GetRequiredService<ShellViewModel>());
}
