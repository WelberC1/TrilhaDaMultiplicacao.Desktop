using System.Collections.ObjectModel;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class RankingViewModel : ViewModelBase
{
    public ObservableCollection<RankingEntrada> Entradas { get; }

    public RankingViewModel(IProgressoRepository progresso)
    {
        Entradas = new ObservableCollection<RankingEntrada>(progresso.ObterRanking());
    }
}
