namespace TrilhaDaMultiplicacao.Desktop.Models.Api;

public record RegistrarRequest(string Nome, string Email, string Senha);

public record LoginRequest(string Email, string Senha);

public record AuthResponse(string Token, AlunoResponseDto Aluno);

public record AlunoResponseDto(int Id, string Nome, string Email, string AvatarEmoji, int PontosTotais);

public record AtualizarPerfilRequest(string Nome, string Email, string AvatarEmoji);

public record EsqueciSenhaRequest(string Email);

public record RedefinirSenhaRequest(string Email, string Codigo, string NovaSenha);
