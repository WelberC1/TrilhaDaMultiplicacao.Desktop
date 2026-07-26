using CommunityToolkit.Mvvm.ComponentModel;

namespace TrilhaDaMultiplicacao.Desktop.Models;

public partial class OpcaoAvatar : ObservableObject
{
    public required string Emoji { get; init; }

    [ObservableProperty]
    public partial bool Selecionado { get; set; }
}
