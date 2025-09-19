using System.Linq;

using AudioControl;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MuteMe.UI.ViewModels;
using MuteMe.UI.Views;

using Utilities;

namespace MuteMe.UI;

public partial class App : Application
{
    private static bool _isSystemShutdownRequested;
    public static bool IsSystemShutdownRequested => _isSystemShutdownRequested;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ServiceProvider serviceProvider = CreateServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            bool startMinimized = desktop.Args is not null && desktop.Args.Any(a => a.ToLower().Contains("minimized"));
            
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            MainWindowViewModel mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindowViewModel.StartMinimized = startMinimized;
            MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();


            mainWindow.DataContext = mainWindowViewModel;
            DataContext = mainWindowViewModel;
            desktop.MainWindow = mainWindow;

            desktop.ShutdownRequested += (_, _) =>
            {
                _isSystemShutdownRequested = true;
                try
                {
                    if (desktop.MainWindow is not null)
                    {
                        desktop.MainWindow.Tag = "CLOSE";
                    }

                    if (mainWindowViewModel is not null)
                    {
                        mainWindowViewModel.OnShuttingDown();
                    }
                }
                catch
                {
                    // ignore exceptions during shutdown
                }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }

    private void ConfigureServices(IServiceCollection services)
    {
        AddLogging(services);
        services.AddSingleton<ButtonHostedService>();
        services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
        services.AddSingleton<IMicrophone, Microphone>();
        services.AddSingleton<IOptionsManager, OptionsManager>();
        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }

    private static void AddLogging(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        DataAnnotationsValidationPlugin[] dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (DataAnnotationsValidationPlugin plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}