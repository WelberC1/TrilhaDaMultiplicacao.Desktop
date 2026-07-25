using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    public ObservableCollection<string> PerguntasSeguranca { get; } =
    [
        "Qual o nome do seu primeiro bichinho de estimação?",
        "Qual é a sua comida favorita?",
        "Qual é o nome da sua escola?",
        "Qual é o seu brinquedo ou desenho favorito?"
    ];

    [ObservableProperty]
    public partial string Nome { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Usuario { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Senha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmarSenha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? PerguntaSelecionada { get; set; }

    [ObservableProperty]
    public partial string RespostaSeguranca { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool CadastroConcluido { get; set; }

    public RegisterViewModel(NavigationService navigation, IServiceProvider services)
    {
        _navigation = navigation;
        _services = services;
        PerguntaSelecionada = PerguntasSeguranca[0];
    }

    [RelayCommand]
    private async Task CriarContaAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Usuario) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            ErrorMessage = "Ops! Preencha todos os campos para criar sua conta. 🙂";
            return;
        }

        if (Senha != ConfirmarSenha)
        {
            ErrorMessage = "As senhas não são iguais. Vamos tentar de novo? 🔁";
            return;
        }

        if (string.IsNullOrWhiteSpace(RespostaSeguranca))
        {
            ErrorMessage = "Não esqueça de responder a pergunta de segurança! 🔐";
            return;
        }

        IsBusy = true;
        try
        {
            await Task.Delay(600);
            CadastroConcluido = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void IrParaLogin() => _navigation.NavigateTo(_services.GetRequiredService<LoginViewModel>());
}
