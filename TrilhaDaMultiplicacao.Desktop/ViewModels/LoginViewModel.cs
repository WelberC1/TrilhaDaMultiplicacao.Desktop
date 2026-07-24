using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    public partial string Usuario { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Senha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? InfoMessage { get; set; }

    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    partial void OnInfoMessageChanged(string? value) => OnPropertyChanged(nameof(HasInfo));

    public LoginViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    [RelayCommand]
    private async Task EntrarAsync()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Senha))
        {
            ErrorMessage = "Ops! Preencha seu usuário e sua senha para continuar. 🙂";
            return;
        }

        IsBusy = true;
        try
        {
            await Task.Delay(500);

            _session.EntrarComo(Usuario.Trim());
            _navigation.NavigateTo(_services.GetRequiredService<HomeViewModel>());
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void CriarConta() => InfoMessage = "Essa novidade está a caminho! 🚧✨";

    [RelayCommand]
    private void EsqueceuSenha() => InfoMessage = "Essa novidade está a caminho! 🚧✨";
}
