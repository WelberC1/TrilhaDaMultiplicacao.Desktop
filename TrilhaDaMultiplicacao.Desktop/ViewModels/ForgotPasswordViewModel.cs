using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public enum EtapaRecuperacao
{
    Email,
    Codigo,
    NovaSenha,
    Concluido
}

public partial class ForgotPasswordViewModel : ViewModelBase
{
    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    public partial EtapaRecuperacao Etapa { get; set; } = EtapaRecuperacao.Email;

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Codigo { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NovaSenha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmarNovaSenha { get; set; } = string.Empty;

    public bool NoEmail => Etapa == EtapaRecuperacao.Email;
    public bool NoCodigo => Etapa == EtapaRecuperacao.Codigo;
    public bool NaNovaSenha => Etapa == EtapaRecuperacao.NovaSenha;
    public bool NoConcluido => Etapa == EtapaRecuperacao.Concluido;

    public bool Passo1Feito => Etapa > EtapaRecuperacao.Email;
    public bool Passo2Feito => Etapa > EtapaRecuperacao.Codigo;
    public bool Passo3Feito => Etapa > EtapaRecuperacao.NovaSenha;

    partial void OnEtapaChanged(EtapaRecuperacao value)
    {
        OnPropertyChanged(nameof(NoEmail));
        OnPropertyChanged(nameof(NoCodigo));
        OnPropertyChanged(nameof(NaNovaSenha));
        OnPropertyChanged(nameof(NoConcluido));
        OnPropertyChanged(nameof(Passo1Feito));
        OnPropertyChanged(nameof(Passo2Feito));
        OnPropertyChanged(nameof(Passo3Feito));
    }

    public ForgotPasswordViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task AvancarEmailAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Digite um e-mail válido para continuar. 📧";
            return;
        }

        IsBusy = true;
        try
        {
            await _session.EsqueciSenhaAsync(Email.Trim());
            Etapa = EtapaRecuperacao.Codigo;
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
    private void AvancarCodigo()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Codigo))
        {
            ErrorMessage = "Digite o código que enviamos para o seu e-mail. 📩";
            return;
        }

        Etapa = EtapaRecuperacao.NovaSenha;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ConcluirAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(NovaSenha))
        {
            ErrorMessage = "Digite sua nova senha. 🔒";
            return;
        }

        if (NovaSenha != ConfirmarNovaSenha)
        {
            ErrorMessage = "As senhas não são iguais. Vamos tentar de novo? 🔁";
            return;
        }

        if (NovaSenha.Length < 6)
        {
            ErrorMessage = "A senha precisa ter pelo menos 6 caracteres. 🔒";
            return;
        }

        IsBusy = true;
        try
        {
            await _session.RedefinirSenhaAsync(Email.Trim(), Codigo.Trim(), NovaSenha);
            Etapa = EtapaRecuperacao.Concluido;
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
    private void Voltar()
    {
        ErrorMessage = null;

        if (Etapa == EtapaRecuperacao.Email)
        {
            _navigation.NavigateTo(_services.GetRequiredService<LoginViewModel>());
        }
        else
        {
            Etapa -= 1;
        }
    }

    [RelayCommand]
    private void IrParaLogin() => _navigation.NavigateTo(_services.GetRequiredService<LoginViewModel>());
}
