using System.Collections.ObjectModel;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class RankingViewModel : ViewModelBase
{
    private readonly IProgressoRepository _progresso;

    public ObservableCollection<RankingEntrada> Entradas { get; } = [];

    public RankingViewModel(IProgressoRepository progresso)
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
            var entradas = await _progresso.ObterRankingAsync();
            Entradas.Clear();
            foreach (var entrada in entradas) Entradas.Add(entrada);
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
