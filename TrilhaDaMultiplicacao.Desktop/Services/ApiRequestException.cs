using System.Net;

namespace TrilhaDaMultiplicacao.Desktop.Services;

public class ApiRequestException(string mensagem, HttpStatusCode? statusCode = null) : Exception(mensagem)
{
    public HttpStatusCode? StatusCode { get; } = statusCode;
}
