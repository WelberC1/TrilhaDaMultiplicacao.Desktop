using TrilhaDaMultiplicacao.Desktop.ViewModels;

namespace TrilhaDaMultiplicacao.Desktop.Services;

public class NavigationService
{
    public event Action<ViewModelBase>? NavigateRequested;

    public void NavigateTo(ViewModelBase viewModel) => NavigateRequested?.Invoke(viewModel);
}
