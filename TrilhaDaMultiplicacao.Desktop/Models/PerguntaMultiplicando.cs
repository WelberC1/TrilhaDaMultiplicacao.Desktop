namespace TrilhaDaMultiplicacao.Desktop.Models;

public class PerguntaMultiplicando
{
    public required int FatorA { get; init; }
    public required int FatorB { get; init; }
    public required bool EsconderPrimeiro { get; init; }
    public required int Produto { get; init; }
    public required string[] Opcoes { get; init; }
    public required int RespostaCorreta { get; init; }
}
