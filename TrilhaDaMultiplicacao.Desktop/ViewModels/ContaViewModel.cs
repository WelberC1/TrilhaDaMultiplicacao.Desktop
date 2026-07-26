using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class ContaViewModel : ViewModelBase
{
    private static readonly string[] EmojisDisponiveis =
    [
        "🦉", "🐱", "🐶", "🦊", "🐻", "🐼", "🦁", "🐯",
        "🐰", "🐵", "🐨", "🦄", "🐸", "🐷", "🐔", "🦋"
    ];

    private readonly IProgressoRepository _progresso;

    public ObservableCollection<OpcaoAvatar> Avatares { get; }

    [ObservableProperty]
    public partial string Nome { get; set; }

    [ObservableProperty]
    public partial string Email { get; set; }

    [ObservableProperty]
    public partial string AvatarSelecionado { get; set; }

    [ObservableProperty]
    public partial string? MensagemPerfil { get; set; }

    [ObservableProperty]
    public partial string SenhaAtual { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NovaSenha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmarNovaSenha { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? MensagemSenha { get; set; }

    [ObservableProperty]
    public partial string? ErroSenha { get; set; }

    public bool TemMensagemPerfil => !string.IsNullOrWhiteSpace(MensagemPerfil);
    public bool TemMensagemSenha => !string.IsNullOrWhiteSpace(MensagemSenha);
    public bool TemErroSenha => !string.IsNullOrWhiteSpace(ErroSenha);

    partial void OnMensagemPerfilChanged(string? value) => OnPropertyChanged(nameof(TemMensagemPerfil));
    partial void OnMensagemSenhaChanged(string? value) => OnPropertyChanged(nameof(TemMensagemSenha));
    partial void OnErroSenhaChanged(string? value) => OnPropertyChanged(nameof(TemErroSenha));

    public ContaViewModel(IProgressoRepository progresso)
    {
        _progresso = progresso;
        Nome = progresso.AlunoNome ?? string.Empty;
        Email = progresso.Email;
        AvatarSelecionado = progresso.AvatarEmoji;

        Avatares = new ObservableCollection<OpcaoAvatar>(EmojisDisponiveis.Select(emoji => new OpcaoAvatar
        {
            Emoji = emoji,
            Selecionado = emoji == AvatarSelecionado
        }));
    }

    [RelayCommand]
    private void SelecionarAvatar(string emoji)
    {
        AvatarSelecionado = emoji;
        foreach (var opcao in Avatares) opcao.Selecionado = opcao.Emoji == emoji;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SalvarPerfilAsync()
    {
        MensagemPerfil = null;
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Nome))
        {
            ErrorMessage = "Digite um nome para continuar. 🙂";
            return;
        }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            ErrorMessage = "Digite um e-mail válido para continuar. 📧";
            return;
        }

        IsBusy = true;
        try
        {
            await _progresso.AtualizarPerfilAsync(Nome.Trim(), Email.Trim(), AvatarSelecionado);
            MensagemPerfil = "✅ Perfil atualizado com sucesso!";
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

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task AlterarSenhaAsync()
    {
        MensagemSenha = null;
        ErroSenha = null;

        if (string.IsNullOrWhiteSpace(SenhaAtual))
        {
            ErroSenha = "Digite sua senha atual. 🔒";
            return;
        }

        if (string.IsNullOrWhiteSpace(NovaSenha))
        {
            ErroSenha = "Digite a nova senha. 🔒";
            return;
        }

        if (NovaSenha != ConfirmarNovaSenha)
        {
            ErroSenha = "As senhas não são iguais. Vamos tentar de novo? 🔁";
            return;
        }

        if (NovaSenha.Length < 6)
        {
            ErroSenha = "A nova senha precisa ter pelo menos 6 caracteres. 🔒";
            return;
        }

        IsBusy = true;
        try
        {
            await _progresso.AlterarSenhaAsync(SenhaAtual, NovaSenha);
            SenhaAtual = string.Empty;
            NovaSenha = string.Empty;
            ConfirmarNovaSenha = string.Empty;
            MensagemSenha = "✅ Senha alterada com sucesso!";
        }
        catch (ApiRequestException ex)
        {
            ErroSenha = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
