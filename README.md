# 🦉 Trilha da Multiplicação

Aplicação desktop **gamificada** para ajudar crianças do Ensino Fundamental I a aprender a operação de multiplicação, brincando.

Este projeto nasceu do meu Trabalho de Graduação (TG), *"O uso das TICs no ensino da operação matemática da multiplicação para alunos do Ensino Fundamental I"*, que investigou como a gamificação pode tornar o aprendizado da matemática mais divertido e engajante — e agora está ganhando uma versão desktop nova, construída do zero.

![Tela de login do Trilha da Multiplicação](docs/tela-login.png)

## 📖 Sobre o projeto

A pesquisa que originou este projeto mostrou que a gamificação — pontuação, ranqueamento, progressão em fases e a liberdade de errar sem punição — ajuda a quebrar o paradigma de que matemática é uma disciplina difícil e chata, aumentando o engajamento e o desempenho dos alunos.

A paleta de cores da interface também não foi escolhida por acaso: segue a psicologia das cores estudada no TG — **azul** para transmitir calma, **laranja** e **vermelho** para estimular energia e ação, e **amarelo** para alegria e estímulo intelectual.

## ✨ O que já existe

- **Login, cadastro e recuperação de senha** ilustrados e animados, pensados para o público infantil.
- **Trilha de fases navegável de verdade**, com progressão sequencial, estrelas por fase e barra de progresso.
- **6 mini-jogos**, cada um trabalhando um ângulo diferente da multiplicação:
  - 🔍 **Adivinhe o Multiplicando** — descobre o fator que falta (`2 × ? = 6`).
  - 🧠 **Memória Numérica** — jogo da memória combinando conta e resultado, com prévia cronometrada antes de virar as cartas.
  - 🧩 **Certo ou Errado?** — julga afirmações verdadeiro/falso: contas, comparações entre produtos e propriedades da multiplicação.
  - 📖 **Ajude o Joãozinho** — problemas de interpretação de texto.
  - ✖️ **Cálculo Rápido** — contas contra o relógio.
  - 🧱 **Monte o Retângulo** — modelo visual de array (linhas × colunas), reforçando *por que* a multiplicação funciona, não só o resultado.
- **Casca de navegação por abas** depois do login: 🗺️ Trilha, 🏆 Ranking, 🎖️ Conquistas e 👤 Conta, com barra fixa mostrando avatar, nome e pontos.
- **Sistema de pontos** que soma a cada fase concluída e alimenta o ranking.
- **Ranking** com o aluno destacado entre outros exploradores, vindo de verdade da API.
- **Conquistas** desbloqueadas automaticamente conforme o progresso.
- **Conta do aluno**: editar nome e e-mail, escolher avatar entre ícones de bichinhos fofos, e trocar senha.

## 📸 Capturas de tela

### Conta e autenticação

| Cadastro | Recuperar senha (e-mail) | Recuperar senha (código) | Recuperar senha (nova senha) |
|---|---|---|---|
| ![Tela de criar conta](docs/tela-criar-conta.png) | ![Recuperar senha — passo 1, informar e-mail](docs/tela-recuperar-senha-1.png) | ![Recuperar senha — passo 2, código de 6 dígitos](docs/tela-recuperar-senha-2.png) | ![Recuperar senha — passo 3, nova senha](docs/tela-recuperar-senha-3.png) |

### Trilha, ranking, conquistas e conta

| Trilha de fases | Ranking | Conquistas | Minha conta |
|---|---|---|---|
| ![Trilha de fases navegável](docs/tela-trilha.png) | ![Ranking geral dos exploradores](docs/tela-ranking.png) | ![Conquistas desbloqueadas](docs/tela-conquistas.png) | ![Edição de conta e avatar](docs/tela-conta.png) |

### Os 6 mini-jogos

| 🧱 Monte o Retângulo | ✖️ Cálculo Rápido | 🧠 Memória Numérica |
|---|---|---|
| ![Monte o Retângulo](docs/jogo-retangulo.png) | ![Cálculo Rápido](docs/jogo-calculo.png) | ![Memória Numérica](docs/jogo-memoria.png) |

| 📖 Ajude o Joãozinho | 🔍 Adivinhe o Multiplicando | 🧩 Certo ou Errado? |
|---|---|---|
| ![Ajude o Joãozinho](docs/jogo-joaozinho.png) | ![Adivinhe o Multiplicando](docs/jogo-multiplicando.png) | ![Certo ou Errado?](docs/jogo-certo-errado.png) |

## 🗺️ Roadmap

- [x] Integração com uma API/backend para persistir progresso, perfil e autenticação de verdade
- [x] Ranking com alunos reais
- [ ] Mais fases e variações para os mini-jogos existentes
- [ ] Testes automatizados

## 🛠️ Tecnologias

- [.NET 10](https://dotnet.microsoft.com/)
- [Avalonia UI](https://avaloniaui.net/) 12 (multiplataforma: Windows, Linux, macOS)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) — padrão MVVM
- Injeção de dependência via `Microsoft.Extensions.DependencyInjection`

## 🚀 Como rodar

Pré-requisitos: [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/WelberC1/TrilhaDaMultiplicacao.Desktop.git
cd TrilhaDaMultiplicacao.Desktop
dotnet run --project TrilhaDaMultiplicacao.Desktop
```

## 📁 Estrutura do projeto

```
TrilhaDaMultiplicacao.Desktop/
├── Views/          # Telas (XAML) — trilha, mini-jogos, abas e casca de navegação
├── ViewModels/      # Lógica de apresentação (MVVM)
├── Models/           # Modelos de fase, perguntas dos jogos, ranking, conquistas etc.
├── Services/          # Navegação, sessão do aluno e progresso (IProgressoRepository)
├── Styles/             # Paleta de cores e estilos dos componentes
└── Assets/              # Imagens e ícones
```

`SessionService` implementa `IProgressoRepository` e fala de verdade com o [backend](https://github.com/WelberC1/TrilhaDaMultiplicacaoAPI) — login, cadastro, progresso, ranking e conquistas são todos persistidos na API. Dentro do processo, a sessão (token, refresh token e perfil) fica só em memória: fechar o app exige logar de novo.

## 🎓 Contexto acadêmico

Projeto pessoal de continuidade do TG apresentado por **Welber Caetano Santos**, sobre o uso de tecnologias da informação e gamificação no ensino da multiplicação para alunos do Ensino Fundamental I.
