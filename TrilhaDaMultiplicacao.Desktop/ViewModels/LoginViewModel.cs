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
    public partial string Email { get; set; } = string.Empty;

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

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task EntrarAsync()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            ErrorMessage = "Ops! Preencha seu e-mail e sua senha para continuar. 🙂";
            return;
        }

        IsBusy = true;
        try
        {
            await _session.LoginAsync(Email.Trim(), Senha);
            _navigation.NavigateTo(_services.GetRequiredService<ShellViewModel>());
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

    [RelayCommand]
    private void CriarConta() => _navigation.NavigateTo(_services.GetRequiredService<RegisterViewModel>());

    [RelayCommand]
    private void EsqueceuSenha() => _navigation.NavigateTo(_services.GetRequiredService<ForgotPasswordViewModel>());
}
