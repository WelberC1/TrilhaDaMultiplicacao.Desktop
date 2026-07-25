namespace TrilhaDaMultiplicacao.Desktop.Models;

public class PerguntaInterpretacao
{
    public required string Enunciado { get; init; }
    public required string[] Opcoes { get; init; }
    public required int RespostaCorreta { get; init; }
}
