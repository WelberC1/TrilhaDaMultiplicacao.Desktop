using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial ViewModelBase? CurrentView { get; set; }

    public MainViewModel(IServiceProvider services, NavigationService navigation)
    {
        navigation.NavigateRequested += vm => CurrentView = vm;

        CurrentView = services.GetRequiredService<LoginViewModel>();
    }
}
