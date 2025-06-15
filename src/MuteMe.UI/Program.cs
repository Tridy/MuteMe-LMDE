using System;
using System.Diagnostics;
using System.Linq;

using Avalonia;

namespace MuteMe.UI;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (StartupHelper.IsAlreadyRunning())
        {
            StartupHelper.ShowAlreadyRunningMessageBox();
            return;
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}

public static class StartupHelper
{
    public static bool IsAlreadyRunning()
    {
        Process[] processes = Process.GetProcesses();
        string? currentProcessMainModuleName = Process.GetCurrentProcess().MainModule?.ModuleName;

        if (currentProcessMainModuleName is null)
        {
            return false;
        }
        
        int count = processes.Count(p => p.MainModule is not null && p.MainModule.ModuleName == currentProcessMainModuleName);
        return count > 1;
    }
    
    public static void ShowAlreadyRunningMessageBox()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "x-terminal-emulator", // or specific terminal like "gnome-terminal", "xterm"
            Arguments = $"-e \"bash -c 'echo MuteMe UI instance is already running!; sleep 5'\"",
            UseShellExecute = true
        };
        
        Process.Start(startInfo);
    }
}