using System;

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace MuteMe.UI;

public partial class App
{
    private void Exit_OnClick(object? sender, EventArgs e)
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window? mainWindow = desktop.MainWindow;

            if (mainWindow is not null)
            {
                mainWindow.Tag = "CLOSE";
                mainWindow.Close();
            }
        }
    }

    private void ShowWindow_OnClick(object? sender, EventArgs e)
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window? mainWindow = desktop.MainWindow;

            if (mainWindow is not null)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
        }
    }
    
    private DateTime _lastClick = DateTime.MinValue;
    private const int DoubleClickTime = 400; // milliseconds

    private void TrayIcon_OnClicked(object? sender, EventArgs e)
    {
        DateTime now = DateTime.Now;
        if ((now - _lastClick).TotalMilliseconds <= DoubleClickTime)
        {
            ShowWindow_OnClick(sender, e);
        }

        _lastClick = now;
    }
}