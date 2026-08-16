using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TrilhaDaMultiplicacao.Desktop.Models;

public partial class CartaMemoria : ObservableObject
{
    /// <summary>Mesmo azul do verso padrão das cartas (Styles/Controls.axaml, Button.memory-card) — usado aqui pra pintar o Background do card inteiro via binding, cobrindo o card virado pra baixo também.</summary>
    private static readonly IBrush CorVirada = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(Color.Parse("#5B9BFF"), 0), new GradientStop(Color.Parse("#1E4FD1"), 1) }
    };

    public required int ParId { get; init; }
    public required string Texto { get; init; }

    /// <summary>Cor do par — igual na conta e no resultado, pra ficar visualmente óbvio que combinam assim que viradas.</summary>
    public required IBrush Cor { get; init; }

    [ObservableProperty]
    public partial bool Revelada { get; set; }

    [ObservableProperty]
    public partial bool Combinada { get; set; }

    public string NomeAcessivel => Revelada || Combinada ? Texto : "Carta virada";

    /// <summary>Background do card inteiro: cor do par quando virada/combinada, azul padrão quando de costas.</summary>
    public IBrush CorFundo => Revelada || Combinada ? Cor : CorVirada;

    partial void OnReveladaChanged(bool value)
    {
        OnPropertyChanged(nameof(NomeAcessivel));
        OnPropertyChanged(nameof(CorFundo));
    }

    partial void OnCombinadaChanged(bool value)
    {
        OnPropertyChanged(nameof(NomeAcessivel));
        OnPropertyChanged(nameof(CorFundo));
    }
}
