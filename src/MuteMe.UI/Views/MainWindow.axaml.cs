using System.Diagnostics;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

using MuteMe.UI.ViewModels;

namespace MuteMe.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        if (this.DataContext is MainWindowViewModel vm)
        {
            if (vm.StartMinimized)
            {
                this.WindowState = WindowState.Minimized;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property.Name == "WindowState")
        {
            if (change.NewValue is WindowState ws)
            {
                if (ws == WindowState.Minimized)
                {
                    this.ShowInTaskbar = false;
                    return;
                }

                this.ShowInTaskbar = true;
            }
        }
    }

    private void Window_Closing(object? sender, WindowClosingEventArgs e)
    {
        // During system logout/shutdown or when explicitly asked to close, do not cancel
        bool allowClose = App.IsSystemShutdownRequested || Tag is not null;
        if (!allowClose)
        {
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            return;
        }
        
        if (DataContext is MainWindowViewModel vm)
        {
            vm.OnShuttingDown();
        }
    }
}