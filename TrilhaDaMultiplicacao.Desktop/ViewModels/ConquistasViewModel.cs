using System.Collections.ObjectModel;
using TrilhaDaMultiplicacao.Desktop.Models;
using TrilhaDaMultiplicacao.Desktop.Services;

namespace TrilhaDaMultiplicacao.Desktop.ViewModels;

public partial class ConquistasViewModel : ViewModelBase
{
    public ObservableCollection<Conquista> Conquistas { get; }

    public int TotalDesbloqueadas => Conquistas.Count(c => c.Desbloqueada);
    public int TotalConquistas => Conquistas.Count;

    public ConquistasViewModel(IProgressoRepository progresso)
    {
        Conquistas = new ObservableCollection<Conquista>(AvaliarConquistas(progresso));
    }

    private static IEnumerable<Conquista> AvaliarConquistas(IProgressoRepository progresso)
    {
        var fasesCompletas = progresso.TodasEstrelas.Count;
        var fasesComTresEstrelas = progresso.TodasEstrelas.Values.Count(e => e == 3);
        var pontos = progresso.PontosTotais;

        yield return new Conquista
        {
            Titulo = "Primeiro Passo",
            Descricao = "Complete a sua primeira fase da trilha.",
            Icone = "🥇",
            Desbloqueada = fasesCompletas >= 1
        };

        yield return new Conquista
        {
            Titulo = "Trinca de Ouro",
            Descricao = "Consiga 3 estrelas em pelo menos uma fase.",
            Icone = "🌟",
            Desbloqueada = fasesComTresEstrelas >= 1
        };

        yield return new Conquista
        {
            Titulo = "Sequência de Craque",
            Descricao = "Complete 3 fases da trilha.",
            Icone = "🔥",
            Desbloqueada = fasesCompletas >= 3
        };

        yield return new Conquista
        {
            Titulo = "Meio Caminho",
            Descricao = "Complete 6 fases da trilha.",
            Icone = "🏃",
            Desbloqueada = fasesCompletas >= 6
        };

        yield return new Conquista
        {
            Titulo = "Trilha Completa",
            Descricao = "Complete as 12 fases da trilha da multiplicação.",
            Icone = "🏆",
            Desbloqueada = fasesCompletas >= 12
        };

        yield return new Conquista
        {
            Titulo = "Estrela em Dobro",
            Descricao = "Consiga 3 estrelas em 5 fases diferentes.",
            Icone = "✨",
            Desbloqueada = fasesComTresEstrelas >= 5
        };

        yield return new Conquista
        {
            Titulo = "Colecionador de Pontos",
            Descricao = "Acumule 300 pontos.",
            Icone = "💰",
            Desbloqueada = pontos >= 300
        };

        yield return new Conquista
        {
            Titulo = "Mestre da Trilha",
            Descricao = "Acumule 800 pontos.",
            Icone = "👑",
            Desbloqueada = pontos >= 800
        };
    }
}
