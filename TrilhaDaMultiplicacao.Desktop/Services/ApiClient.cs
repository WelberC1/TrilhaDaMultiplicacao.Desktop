using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TrilhaDaMultiplicacao.Desktop.Services;

public class ApiClient(HttpClient httpClient)
{
    public const string BaseUrl = "http://localhost:5271";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<TResponse> PostAsync<TRequest, TResponse>(string caminho, TRequest corpo, string? token = null) =>
        EnviarAsync<TResponse>(CriarMensagem(HttpMethod.Post, caminho, corpo, token));

    public Task PostAsync<TRequest>(string caminho, TRequest corpo) =>
        EnviarSemRespostaAsync(CriarMensagem(HttpMethod.Post, caminho, corpo, token: null));

    public Task<TResponse> PutAsync<TRequest, TResponse>(string caminho, TRequest corpo, string token) =>
        EnviarAsync<TResponse>(CriarMensagem(HttpMethod.Put, caminho, corpo, token));

    public Task<TResponse> GetAsync<TResponse>(string caminho, string token) =>
        EnviarAsync<TResponse>(CriarMensagem<object?>(HttpMethod.Get, caminho, corpo: null, token));

    private static HttpRequestMessage CriarMensagem<TRequest>(HttpMethod metodo, string caminho, TRequest? corpo, string? token)
    {
        var mensagem = new HttpRequestMessage(metodo, caminho);

        if (corpo is not null)
            mensagem.Content = JsonContent.Create(corpo, options: JsonOptions);

        if (token is not null)
            mensagem.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return mensagem;
    }

    private async Task<TResponse> EnviarAsync<TResponse>(HttpRequestMessage mensagem)
    {
        var resposta = await EnviarERegistrarErroAsync(mensagem);

        var resultado = await resposta.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
        return resultado ?? throw new ApiRequestException("Resposta inválida do servidor.");
    }

    private async Task EnviarSemRespostaAsync(HttpRequestMessage mensagem) =>
        await EnviarERegistrarErroAsync(mensagem);

    private async Task<HttpResponseMessage> EnviarERegistrarErroAsync(HttpRequestMessage mensagem)
    {
        HttpResponseMessage resposta;
        try
        {
            resposta = await httpClient.SendAsync(mensagem);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new ApiRequestException("Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente.");
        }

        if (!resposta.IsSuccessStatusCode)
        {
            var erro = await resposta.Content.ReadFromJsonAsync<ErroResponse>(JsonOptions);
            throw new ApiRequestException(erro?.Mensagem ?? "Ocorreu um erro inesperado. Tente novamente.");
        }

        return resposta;
    }

    private record ErroResponse(string Mensagem);
}
