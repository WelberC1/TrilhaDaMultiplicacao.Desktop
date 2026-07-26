using CommunityToolkit.Mvvm.ComponentModel;

namespace TrilhaDaMultiplicacao.Desktop.Models;

public partial class CartaMemoria : ObservableObject
{
    public required int ParId { get; init; }
    public required string Texto { get; init; }

    [ObservableProperty]
    public partial bool Revelada { get; set; }

    [ObservableProperty]
    public partial bool Combinada { get; set; }

    public string NomeAcessivel => Revelada || Combinada ? Texto : "Carta virada";

    partial void OnReveladaChanged(bool value) => OnPropertyChanged(nameof(NomeAcessivel));
    partial void OnCombinadaChanged(bool value) => OnPropertyChanged(nameof(NomeAcessivel));
}
