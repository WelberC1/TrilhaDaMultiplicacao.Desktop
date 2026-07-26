using System.Collections.ObjectModel;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class ConquistasViewModel : ViewModelBase
{
    private readonly IProgressoRepository _progresso;

    public ObservableCollection<Conquista> Conquistas { get; } = [];

    public int TotalDesbloqueadas => Conquistas.Count(c => c.Desbloqueada);
    public int TotalConquistas => Conquistas.Count;

    public ConquistasViewModel(IProgressoRepository progresso)
    {
        _progresso = progresso;
        _ = CarregarAsync();
    }

    private async Task CarregarAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var conquistas = await _progresso.ObterConquistasAsync();
            Conquistas.Clear();
            foreach (var conquista in conquistas) Conquistas.Add(conquista);
            OnPropertyChanged(nameof(TotalDesbloqueadas));
            OnPropertyChanged(nameof(TotalConquistas));
        }
        catch (ApiRequestException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
