using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public enum EtapaRecuperacao
{
    Email,
    Pergunta,
    NovaSenha,
    Concluido
}

public partial class ForgotPasswordViewModel : ViewModelBase
{
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    [ObservableProperty]
    public partial EtapaRecuperacao Etapa { get; set; } = EtapaRecuperacao.Email;

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PerguntaExibida { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string RespostaSeguranca { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NovaSenha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmarNovaSenha { get; set; } = string.Empty;

    public bool NoEmail => Etapa == EtapaRecuperacao.Email;
    public bool NaPergunta => Etapa == EtapaRecuperacao.Pergunta;
    public bool NaNovaSenha => Etapa == EtapaRecuperacao.NovaSenha;
    public bool NoConcluido => Etapa == EtapaRecuperacao.Concluido;

    public bool Passo1Feito => Etapa > EtapaRecuperacao.Email;
    public bool Passo2Feito => Etapa > EtapaRecuperacao.Pergunta;
    public bool Passo3Feito => Etapa > EtapaRecuperacao.NovaSenha;

    partial void OnEtapaChanged(EtapaRecuperacao value)
    {
        OnPropertyChanged(nameof(NoEmail));
        OnPropertyChanged(nameof(NaPergunta));
        OnPropertyChanged(nameof(NaNovaSenha));
        OnPropertyChanged(nameof(NoConcluido));
        OnPropertyChanged(nameof(Passo1Feito));
        OnPropertyChanged(nameof(Passo2Feito));
        OnPropertyChanged(nameof(Passo3Feito));
    }

    public ForgotPasswordViewModel(NavigationService navigation, IServiceProvider services)
    {
        _navigation = navigation;
        _services = services;
    }

    [RelayCommand]
    private void AvancarEmail()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Digite um e-mail válido para continuar. 📧";
            return;
        }

        PerguntaExibida = "Qual o nome do seu primeiro bichinho de estimação?";
        Etapa = EtapaRecuperacao.Pergunta;
    }

    [RelayCommand]
    private void AvancarPergunta()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(RespostaSeguranca))
        {
            ErrorMessage = "Responda a pergunta de segurança para continuar. 🤔";
            return;
        }

        Etapa = EtapaRecuperacao.NovaSenha;
    }

    [RelayCommand]
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

        IsBusy = true;
        try
        {
            await Task.Delay(600);
            Etapa = EtapaRecuperacao.Concluido;
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
