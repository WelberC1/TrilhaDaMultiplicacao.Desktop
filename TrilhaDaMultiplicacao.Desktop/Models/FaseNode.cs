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

    public required int Numero { get; init; }
    public required string Titulo { get; init; }
    public required TipoDesafio Tipo { get; init; }
    public required FaseStatus Status { get; init; }
    public int Estrelas { get; init; }
    public double X { get; init; }
    public double Y { get; init; }

    public bool EhBloqueada => Status == FaseStatus.Bloqueada;
    public bool EhDisponivel => Status == FaseStatus.Disponivel;
    public bool EhConcluida => Status == FaseStatus.Concluida;

    public double Tamanho => EhDisponivel ? 104 : 84;
    public double Centro => X + Tamanho / 2;
    public double CentroY => Y + Tamanho / 2;

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
}
