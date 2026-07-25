namespace TrilhaDaMultiplicacao.Desktop.Models;

public class PerguntaCalculo
{
    public required int FatorA { get; init; }
    public required int FatorB { get; init; }
    public required string[] Opcoes { get; init; }
    public required int RespostaCorreta { get; init; }
}
