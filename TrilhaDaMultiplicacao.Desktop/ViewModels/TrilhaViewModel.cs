using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    public string AlunoNome => _session.AlunoNome ?? "explorador";

    public HomeViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    [RelayCommand]
    private void Sair()
    {
        _session.Sair();
        _navigation.NavigateTo(_services.GetRequiredService<LoginViewModel>());
    }
}
