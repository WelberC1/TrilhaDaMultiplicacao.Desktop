using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    public partial string Nome { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Senha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmarSenha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool CadastroConcluido { get; set; }

    public RegisterViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task CriarContaAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            ErrorMessage = "Ops! Preencha todos os campos para criar sua conta. 🙂";
            return;
        }

        if (Senha != ConfirmarSenha)
        {
            ErrorMessage = "As senhas não são iguais. Vamos tentar de novo? 🔁";
            return;
        }

        if (Senha.Length < 6)
        {
            ErrorMessage = "A senha precisa ter pelo menos 6 caracteres. 🔒";
            return;
        }

        IsBusy = true;
        try
        {
            await _session.RegistrarAsync(Nome.Trim(), Email.Trim(), Senha);
            CadastroConcluido = true;
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
    private void IrParaLogin() => _navigation.NavigateTo(_services.GetRequiredService<LoginViewModel>());
}
