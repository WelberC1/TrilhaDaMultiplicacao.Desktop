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

    public Task PostAsync(string caminho, string token) =>
        EnviarSemRespostaAsync(CriarMensagem<object?>(HttpMethod.Post, caminho, corpo: null, token));

    public Task<TResponse> PutAsync<TRequest, TResponse>(string caminho, TRequest corpo, string token) =>
        EnviarAsync<TResponse>(CriarMensagem(HttpMethod.Put, caminho, corpo, token));

    public Task PutAsync<TRequest>(string caminho, TRequest corpo, string token) =>
        EnviarSemRespostaAsync(CriarMensagem(HttpMethod.Put, caminho, corpo, token));

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
            throw new ApiRequestException(await ExtrairMensagemDeErroAsync(resposta));
        }

        return resposta;
    }

    private static async Task<string> ExtrairMensagemDeErroAsync(HttpResponseMessage resposta)
    {
        // Nem todo erro passa pelo tratamento central da API — uma rejeição de autenticação
        // (ex.: token com sessão revogada) vem direto do middleware, com corpo vazio, então
        // ReadFromJsonAsync lançaria JsonException em vez de simplesmente não achar "mensagem".
        try
        {
            var erro = await resposta.Content.ReadFromJsonAsync<ErroResponse>(JsonOptions);
            if (erro?.Mensagem is not null) return erro.Mensagem;
        }
        catch (JsonException)
        {
            // corpo vazio ou não-JSON — cai no padrão por status abaixo.
        }

        return resposta.StatusCode == System.Net.HttpStatusCode.Unauthorized
            ? "Sessão expirada. Faça login novamente."
            : "Ocorreu um erro inesperado. Tente novamente.";
    }

    private record ErroResponse(string Mensagem);
}
