using System.Collections.ObjectModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class TrilhaViewModel : ViewModelBase
{
    private readonly SessionService _session;
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _services;

    public string AlunoNome => _session.AlunoNome ?? "explorador";

    public ObservableCollection<FaseNode> Fases { get; }

    public Points TrilhaPontos { get; }

    public int TotalEstrelas => Fases.Sum(f => f.Estrelas);

    public double CanvasWidth { get; } = 760;
    public double CanvasHeight { get; }

    public double PrimeiraFaseX => Fases.Count > 0 ? Fases[0].X : 0;
    public double UltimaFaseX => Fases.Count > 0 ? Fases[^1].X : 0;
    public double UltimaFaseY => Fases.Count > 0 ? Fases[^1].Y : 0;

    [ObservableProperty]
    public partial string? InfoMessage { get; set; }

    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    partial void OnInfoMessageChanged(string? value) => OnPropertyChanged(nameof(HasInfo));

    public TrilhaViewModel(SessionService session, NavigationService navigation, IServiceProvider services)
    {
        _session = session;
        _navigation = navigation;
        _services = services;

        Fases = new ObservableCollection<FaseNode>(CriarFases(_session));
        TrilhaPontos = new Points(Fases.Select(f => new Point(f.Centro, f.CentroY)));
        CanvasHeight = Fases.Count > 0 ? Fases[^1].Y + 220 : 400;
    }

    private static IEnumerable<FaseNode> CriarFases(SessionService session)
    {
        var definicoes = new (string Titulo, TipoDesafio Tipo)[]
        {
            ("Adivinhe o Multiplicando", TipoDesafio.Calculo),
            ("Memória Numérica", TipoDesafio.Memoria),
            ("Certo ou Errado?", TipoDesafio.RaciocinioLogico),
            ("Ajude o Joãozinho", TipoDesafio.Interpretacao),
            ("Cálculo Rápido", TipoDesafio.Calculo),
            ("Memória Numérica II", TipoDesafio.Memoria),
            ("Jogo dos Algoritmos", TipoDesafio.RaciocinioLogico),
            ("Ajude o Joãozinho II", TipoDesafio.Interpretacao),
            ("Operações com 2 Dígitos", TipoDesafio.Calculo),
            ("Memória Numérica III", TipoDesafio.Memoria),
            ("Certo ou Errado? II", TipoDesafio.RaciocinioLogico),
            ("Operações com 3 Dígitos", TipoDesafio.Calculo),
        };

        const double centerX = 320;
        const double amplitude = 230;
        const double ySpacing = 165;
        const double yStart = 50;

        var desbloqueando = true;

        for (var i = 0; i < definicoes.Length; i++)
        {
            var (titulo, tipo) = definicoes[i];
            var numero = i + 1;
            var estrelas = session.EstrelasDaFase(numero);

            FaseStatus status;
            if (estrelas.HasValue)
            {
                status = FaseStatus.Concluida;
            }
            else if (desbloqueando)
            {
                status = FaseStatus.Disponivel;
                desbloqueando = false;
            }
            else
            {
                status = FaseStatus.Bloqueada;
            }

            var x = centerX + amplitude * Math.Sin((i + 0.5) * 0.85) - 42;

            yield return new FaseNode
            {
                Numero = numero,
                Titulo = titulo,
                Tipo = tipo,
                Status = status,
                Estrelas = estrelas ?? 0,
                X = x,
                Y = yStart + i * ySpacing
            };
        }
    }

    [RelayCommand]
    private void AbrirFase(FaseNode fase)
    {
        if (fase.Status == FaseStatus.Disponivel && fase.Tipo == TipoDesafio.Interpretacao)
        {
            var jogo = _services.GetRequiredService<InterpretacaoViewModel>();
            jogo.Configurar(fase.Numero, fase.Titulo);
            _navigation.NavigateTo(jogo);
            return;
        }

        InfoMessage = fase.Status switch
        {
            FaseStatus.Bloqueada => $"🔒 Complete as fases anteriores para desbloquear \"{fase.Titulo}\"!",
            FaseStatus.Disponivel => $"🚧 \"{fase.Titulo}\" chega em breve! Continue treinando.",
            FaseStatus.Concluida => $"⭐ Você já mandou bem em \"{fase.Titulo}\"! Modo revisão chega em breve.",
            _ => null
        };
    }

    [RelayCommand]
    private void Sair()
    {
        _session.Sair();
        _navigation.NavigateTo(_services.GetRequiredService<LoginViewModel>());
    }
}
