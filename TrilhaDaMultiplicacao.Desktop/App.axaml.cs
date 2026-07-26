using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TrilhaDaMultiplicacao.Desktop.Services;
using TrilhaDaMultiplicacao.Desktop.ViewModels;
using TrilhaDaMultiplicacao.Desktop.Views;

namespace TrilhaDaMultiplicacao.Desktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SessionService>();
        services.AddSingleton<NavigationService>();

        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<TrilhaViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<ForgotPasswordViewModel>();
        services.AddTransient<InterpretacaoViewModel>();
        services.AddTransient<CalculoViewModel>();
        services.AddTransient<RetanguloViewModel>();
        services.AddTransient<MemoriaViewModel>();
        services.AddTransient<RaciocinioViewModel>();
        services.AddTransient<MultiplicandoViewModel>();
    }
}