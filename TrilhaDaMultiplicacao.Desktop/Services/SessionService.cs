namespace TrilhaDaMultiplicacao.Desktop.Services;

public class SessionService
{
    private readonly Dictionary<int, int> _estrelasPorFase = new();

    public string? AlunoNome { get; private set; }

    public void EntrarComo(string nome)
    {
        AlunoNome = nome;

        if (_estrelasPorFase.Count == 0)
        {
            _estrelasPorFase[1] = 3;
            _estrelasPorFase[3] = 3;
        }
    }

    public void Sair()
    {
        AlunoNome = null;
        _estrelasPorFase.Clear();
    }

    public void RegistrarConclusaoFase(int numeroFase, int estrelas) =>
        _estrelasPorFase[numeroFase] = Math.Max(estrelas, _estrelasPorFase.GetValueOrDefault(numeroFase));

    public int? EstrelasDaFase(int numeroFase) =>
        _estrelasPorFase.TryGetValue(numeroFase, out var estrelas) ? estrelas : null;
}
