namespace TrilhaDaMultiplicacao.Desktop.Models.Api;

public record RegistrarConclusaoRequest(int Estrelas);

public record FaseProgressoResponseDto(int NumeroFase, int Estrelas, int Pontos, DateTime ConcluidaEm);
