using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class MemoriaViewModel : ViewModelBase
{
    private const int TotalPares = 6;
    private const int TempoPreviewInicial = 5;

    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;
    private readonly Random _random = new();

    private int _numeroFase;
    private CartaMemoria? _primeiraSelecionada;

    [ObservableProperty]
    public partial string TituloFase { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<CartaMemoria> Cartas { get; set; } = [];

    [ObservableProperty]
    public partial int ParesEncontrados { get; set; }

    [ObservableProperty]
    public partial int Erros { get; set; }

    [ObservableProperty]
    public partial bool AguardandoComparacao { get; set; }

    [ObservableProperty]
    public partial bool MostrandoPreview { get; set; }

    [ObservableProperty]
    public partial int TempoPreview { get; set; }

    [ObservableProperty]
    public partial bool JogoConcluido { get; set; }

    [ObservableProperty]
    public partial int EstrelasConquistadas { get; set; }

    public int TotalParesConst => TotalPares;
    public string Estrelinhas => new string('★', EstrelasConquistadas) + new string('☆', 3 - EstrelasConquistadas);

    partial void OnEstrelasConquistadasChanged(int value) => OnPropertyChanged(nameof(Estrelinhas));

    public MemoriaViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    public void Configurar(int numeroFase, string titulo)
    {
        _numeroFase = numeroFase;
        TituloFase = titulo;

        ParesEncontrados = 0;
        Erros = 0;
        JogoConcluido = false;
        EstrelasConquistadas = 0;
        _primeiraSelecionada = null;

        var cartas = GerarCartas();
        foreach (var carta in cartas) carta.Revelada = true;
        Cartas = new ObservableCollection<CartaMemoria>(cartas);

        MostrandoPreview = true;
        AguardandoComparacao = true;
        TempoPreview = TempoPreviewInicial;
        _ = ExecutarPreviewAsync();
    }

    private async Task ExecutarPreviewAsync()
    {
        while (TempoPreview > 0)
        {
            await Task.Delay(1000);
            TempoPreview--;
        }

        foreach (var carta in Cartas) carta.Revelada = false;
        MostrandoPreview = false;
        AguardandoComparacao = false;
    }

    private List<CartaMemoria> GerarCartas()
    {
        var fatos = new List<(int A, int B)>();
        var produtos = new HashSet<int>();

        while (fatos.Count < TotalPares)
        {
            var a = _random.Next(2, 10);
            var b = _random.Next(2, 10);
            var produto = a * b;
            if (!produtos.Add(produto)) continue;
            fatos.Add((a, b));
        }

        var cartas = new List<CartaMemoria>();
        for (var i = 0; i < fatos.Count; i++)
        {
            var (a, b) = fatos[i];
            cartas.Add(new CartaMemoria { ParId = i, Texto = $"{a} × {b}" });
            cartas.Add(new CartaMemoria { ParId = i, Texto = (a * b).ToString() });
        }

        return cartas.OrderBy(_ => _random.Next()).ToList();
    }

    [RelayCommand]
    private async Task VirarCartaAsync(CartaMemoria carta)
    {
        if (AguardandoComparacao || carta.Revelada || carta.Combinada) return;

        carta.Revelada = true;

        if (_primeiraSelecionada is null)
        {
            _primeiraSelecionada = carta;
            return;
        }

        var primeira = _primeiraSelecionada;
        _primeiraSelecionada = null;

        if (primeira.ParId == carta.ParId)
        {
            primeira.Combinada = true;
            carta.Combinada = true;
            ParesEncontrados++;

            if (ParesEncontrados == TotalPares)
            {
                EstrelasConquistadas = Erros switch
                {
                    0 => 3,
                    <= 2 => 2,
                    <= 5 => 1,
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

                await Task.Delay(500);
                JogoConcluido = true;
            }
            return;
        }

        Erros++;
        AguardandoComparacao = true;
        await Task.Delay(900);
        primeira.Revelada = false;
        carta.Revelada = false;
        AguardandoComparacao = false;
    }

    [RelayCommand]
    private void VoltarTrilha() => _navigation.NavigateTo(_services.GetRequiredService<ShellViewModel>());
}
