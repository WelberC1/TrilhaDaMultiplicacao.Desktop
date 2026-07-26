using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public enum AbaPrincipal
{
    Trilha,
    Ranking,
    Conquistas,
    Conta
}

public partial class ShellViewModel : ViewModelBase
{
    private readonly SessionService _progresso;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    public string AlunoNome => _progresso.AlunoNome ?? "explorador";
    public string AvatarEmoji => _progresso.AvatarEmoji;
    public int PontosTotais => _progresso.PontosTotais;

    [ObservableProperty]
    public partial AbaPrincipal AbaAtual { get; set; }

    [ObservableProperty]
    public partial ViewModelBase AbaConteudo { get; set; }

    public bool NaTrilha => AbaAtual == AbaPrincipal.Trilha;
    public bool NoRanking => AbaAtual == AbaPrincipal.Ranking;
    public bool NasConquistas => AbaAtual == AbaPrincipal.Conquistas;
    public bool NaConta => AbaAtual == AbaPrincipal.Conta;

    partial void OnAbaAtualChanged(AbaPrincipal value)
    {
        OnPropertyChanged(nameof(NaTrilha));
        OnPropertyChanged(nameof(NoRanking));
        OnPropertyChanged(nameof(NasConquistas));
        OnPropertyChanged(nameof(NaConta));
    }

    public ShellViewModel(SessionService progresso, NavigationService navigation, IServiceProvider services)
    {
        _progresso = progresso;
        _navigation = navigation;
        _services = services;

        AbaAtual = AbaPrincipal.Trilha;
        AbaConteudo = _services.GetRequiredService<TrilhaViewModel>();
    }

    [RelayCommand]
    private void IrParaTrilha()
    {
        AtualizarTopoPerfil();
        AbaAtual = AbaPrincipal.Trilha;
        AbaConteudo = _services.GetRequiredService<TrilhaViewModel>();
    }

    [RelayCommand]
    private void IrParaRanking()
    {
        AtualizarTopoPerfil();
        AbaAtual = AbaPrincipal.Ranking;
        AbaConteudo = _services.GetRequiredService<RankingViewModel>();
    }

    [RelayCommand]
    private void IrParaConquistas()
    {
        AtualizarTopoPerfil();
        AbaAtual = AbaPrincipal.Conquistas;
        AbaConteudo = _services.GetRequiredService<ConquistasViewModel>();
    }

    [RelayCommand]
    private void IrParaConta()
    {
        AtualizarTopoPerfil();
        AbaAtual = AbaPrincipal.Conta;
        AbaConteudo = _services.GetRequiredService<ContaViewModel>();
    }

    /// <summary>
    /// AlunoNome/AvatarEmoji/PontosTotais são leituras diretas do SessionService,
    /// que não implementa INotifyPropertyChanged — então a barra fixa do topo só
    /// reflete edições feitas na aba Conta (ou pontos ganhos numa fase) ao trocar
    /// de aba. Isso cobre o fluxo real (salvar e navegar), sem exigir inscrição em
    /// eventos do singleton, que vazaria memória entre ShellViewModels transientes.
    /// </summary>
    private void AtualizarTopoPerfil()
    {
        OnPropertyChanged(nameof(AlunoNome));
        OnPropertyChanged(nameof(AvatarEmoji));
        OnPropertyChanged(nameof(PontosTotais));
    }

    [RelayCommand]
    private void Sair()
    {
        _progresso.Sair();
        _navigation.NavigateTo(_services.GetRequiredService<LoginViewModel>());
    }
}
