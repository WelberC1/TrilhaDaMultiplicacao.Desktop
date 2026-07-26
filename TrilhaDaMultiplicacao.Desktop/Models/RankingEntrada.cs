namespace TrilhaDaMultiplicacao.Desktop.Models;

public class RankingEntrada
{
    public required int Posicao { get; init; }
    public required string Nome { get; init; }
    public required string AvatarEmoji { get; init; }
    public required int Pontos { get; init; }
    public bool EhVoce { get; init; }

    public string Medalha => Posicao switch
    {
        1 => "🥇",
        2 => "🥈",
        3 => "🥉",
        _ => ""
    };

    public bool TemMedalha => Posicao <= 3;
}
