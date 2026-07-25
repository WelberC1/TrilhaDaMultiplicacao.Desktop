using Avalonia;
using Avalonia.Media;

namespace TrilhaDaMultiplicacao.Desktop.Models;

public enum TipoDesafio
{
    Calculo,
    Memoria,
    RaciocinioLogico,
    Interpretacao
}

public enum FaseStatus
{
    Bloqueada,
    Disponivel,
    Concluida
}

public class FaseNode
{
    public const double LinhaLargura = 660;
    public const double LinhaAltura = 190;

    private static readonly IBrush CorCalculo = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(Color.Parse("#FFA24D"), 0), new GradientStop(Color.Parse("#F5590C"), 1) }
    };

    private static readonly IBrush CorMemoria = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(Color.Parse("#5B9BFF"), 0), new GradientStop(Color.Parse("#1E4FD1"), 1) }
    };

    private static readonly IBrush CorRaciocinio = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(Color.Parse("#FF7A7E"), 0), new GradientStop(Color.Parse("#E63238"), 1) }
    };

    private static readonly IBrush CorInterpretacao = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = { new GradientStop(Color.Parse("#FFDA6B"), 0), new GradientStop(Color.Parse("#F5AC1B"), 1) }
    };

    private static readonly IBrush CorBloqueada = new SolidColorBrush(Color.Parse("#8C97B4"));
    private static readonly IBrush CorConectorFeito = new SolidColorBrush(Color.Parse("#2FC48D"));
    private static readonly IBrush CorConectorPadrao = new SolidColorBrush(Color.Parse("#A9B4D6"));

    public required int Numero { get; init; }
    public required string Titulo { get; init; }
    public required TipoDesafio Tipo { get; init; }
    public required FaseStatus Status { get; init; }
    public int Estrelas { get; init; }

    /// <summary>Deslocamento horizontal (borda esquerda) do nó dentro da linha, gerando o efeito de zigue-zague.</summary>
    public required double X { get; init; }

    /// <summary>Centro horizontal do nó anterior, usado para desenhar o conector diagonal entre as fases.</summary>
    public double CentroAnterior { get; init; }

    public bool TemConector { get; init; }
    public bool ConectorFeito { get; init; }

    public bool EhBloqueada => Status == FaseStatus.Bloqueada;
    public bool EhDisponivel => Status == FaseStatus.Disponivel;
    public bool EhConcluida => Status == FaseStatus.Concluida;

    public double Tamanho => EhDisponivel ? 108 : 88;
    public double Centro => X + Tamanho / 2;
    public double TopoNode => 96 - Tamanho / 2;

    public Point PontoConectorInicio => new(CentroAnterior, 96 - LinhaAltura);
    public Point PontoConectorFim => new(Centro, 96);

    public Thickness MargemRotulo => new(Math.Max(0, Centro - 78), 6, 0, 0);

    public string Glyph => Status == FaseStatus.Bloqueada
        ? "🔒"
        : Tipo switch
        {
            TipoDesafio.Calculo => "✖️",
            TipoDesafio.Memoria => "🧠",
            TipoDesafio.RaciocinioLogico => "🧩",
            TipoDesafio.Interpretacao => "📖",
            _ => "❓"
        };

    public string TipoNome => Tipo switch
    {
        TipoDesafio.Calculo => "Cálculo",
        TipoDesafio.Memoria => "Memória",
        TipoDesafio.RaciocinioLogico => "Raciocínio lógico",
        TipoDesafio.Interpretacao => "Interpretação de problemas",
        _ => ""
    };

    public string Estrelinhas => new string('★', Math.Clamp(Estrelas, 0, 3)) + new string('☆', 3 - Math.Clamp(Estrelas, 0, 3));

    public IBrush Cor => Status == FaseStatus.Bloqueada
        ? CorBloqueada
        : Tipo switch
        {
            TipoDesafio.Calculo => CorCalculo,
            TipoDesafio.Memoria => CorMemoria,
            TipoDesafio.RaciocinioLogico => CorRaciocinio,
            TipoDesafio.Interpretacao => CorInterpretacao,
            _ => CorBloqueada
        };

    public IBrush CorConector => ConectorFeito ? CorConectorFeito : CorConectorPadrao;
}
