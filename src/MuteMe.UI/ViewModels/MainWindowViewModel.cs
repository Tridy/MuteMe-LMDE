using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;

using Avalonia.Controls;
using Avalonia.Platform;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.Extensions.Logging;

using Utilities;

// ReSharper disable InconsistentNaming

namespace MuteMe.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly CancellationToken _cancellationToken;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly bool _isInitializing;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly Options _options;

    private readonly IOptionsManager _optionsManager;
    private readonly IBackgroundQueue _queue;

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private string? selectedMode;

    [ObservableProperty]
    private string? selectedMutedColor;

    [ObservableProperty]
    private string? selectedUnmutedColor;

    [ObservableProperty]
    private string state;

    public MainWindowViewModel(IOptionsManager optionsManager, ButtonHostedService buttonHostedService, IBackgroundQueue queue, ILogger<MainWindowViewModel> logger)
    {
        _isInitializing = true;
        _logger = logger;
        _optionsManager = optionsManager;
        isConnected = false;
        _options = ApplyFromOptions();
        _queue = queue;
        _isInitializing = false;
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        
        State = "UNMUTED";
        SystemTrayIcon = new WindowIcon(AssetLoader.Open(new Uri("avares://MuteMe.UI/Assets/green-mic.png")));

        StartButtonHostService(buttonHostedService);
        StartButtonToUiMonitor();
    }

    public WindowIcon SystemTrayIcon
    {
        get;
    }

    public bool StartMinimized
    {
        get;
        set;
    }

    private void StartButtonHostService(ButtonHostedService buttonHostedService)
    {
        Task.Run(async () =>
            {
                try
                {
                    await buttonHostedService.ExecuteAsync(_cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in ButtonHostedService");
                    throw;
                }
            }, _cancellationToken)
            .SafeFireAndForget(onException: ex =>
            {
                if (ex is OperationCanceledException)
                {
                    _logger.LogDebug("ButtonHostedService was cancelled");
                }
                else
                {
                    _logger.LogError(ex, $"Error in {nameof(MainWindowViewModel)} when calling {nameof(StartButtonHostService)}");
                }
            });
    }

    private void StartButtonToUiMonitor()
    {
        Task.Run(async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    ButtonToUiWorkItemType changeType = await _queue.ReceiveButtonToUiAsync(_cancellationToken);

                    switch (changeType)
                    {
                        case ButtonToUiWorkItemType.Muted:
                            State = "MUTED";
                            break;
                        case ButtonToUiWorkItemType.Unmuted:
                            State = "UNMUTED";
                            break;
                        case ButtonToUiWorkItemType.Connected:
                            IsConnected = true;
                            break;
                        case ButtonToUiWorkItemType.Disconnected:
                            IsConnected = false;
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }, _cancellationToken)
            .SafeFireAndForget(onException: ex =>
            {
                if (ex is OperationCanceledException)
                {
                    _logger.LogDebug("ButtonToUiMonitor was cancelled");
                }
                else
                {
                    _logger.LogError(ex, $"Error in {nameof(MainWindowViewModel)} when calling {nameof(StartButtonToUiMonitor)}");
                }
            });
    }

    private Options ApplyFromOptions()
    {
        Options options = _optionsManager.GetOptions();
        SelectedMutedColor = options.Main.MutedColor.Name;
        SelectedUnmutedColor = options.Main.UnmutedColor.Name;
        SelectedMode = options.Main.Mode;
        return options;
    }

    partial void OnSelectedMutedColorChanged(string? value)
    {
        if (_isInitializing || value is null)
        {
            return;
        }

        _logger.LogDebug("Muted Color Changed: {Color}", value);
        Color color = Color.FromName(value);
        _options.Main.MutedColor = color;
        _optionsManager.SaveOptions(_options);

        _queue.SendUiToButtonAsync(new UiToButtonWorkItem(UiToButtonWorkItemType.MuteColor, value))
            .SafeFireAndForget(onException: ex =>
            {
                _logger.LogError(ex, $"Error in {nameof(OnSelectedMutedColorChanged)} when calling {nameof(MainWindowViewModel)}.{nameof(_queue.SendButtonToUiAsync)}");
            });
    }

    partial void OnSelectedUnmutedColorChanged(string? value)
    {
        if (_isInitializing || value is null)
        {
            return;
        }

        _logger.LogDebug("Unmuted Color Changed: {Color}", value);
        Color color = Color.FromName(value);
        _options.Main.UnmutedColor = color;
        _optionsManager.SaveOptions(_options);

        _queue.SendUiToButtonAsync(new UiToButtonWorkItem(UiToButtonWorkItemType.UnmuteColor, value))
            .SafeFireAndForget(onException: ex => _logger.LogError(ex, $"Error in {nameof(OnSelectedUnmutedColorChanged)} when calling {nameof(MainWindowViewModel)}.{nameof(_queue.SendButtonToUiAsync)}"));
    }

    partial void OnSelectedModeChanged(string? value)
    {
        if (_isInitializing || value is null)
        {
            return;
        }

        _logger.LogDebug("Mode Changed: {Mode}", value);
        _options.Main.Mode = value;
        _optionsManager.SaveOptions(_options);

        _queue.SendUiToButtonAsync(new UiToButtonWorkItem(UiToButtonWorkItemType.Mode, value))
            .SafeFireAndForget(onException: ex => _logger.LogError(ex, $"Error in {nameof(OnSelectedModeChanged)} when calling {nameof(MainWindowViewModel)}.{nameof(_queue.SendButtonToUiAsync)}"));
    }

    public void OnShuttingDown()
    {
        if (!_cancellationTokenSource.IsCancellationRequested)
        {
            _queue.SendUiToButtonAsync(new UiToButtonWorkItem(UiToButtonWorkItemType.ShuttingDown, "OFF"))
                .SafeFireAndForget(onException: ex => _logger.LogError(ex, $"Error in {nameof(OnShuttingDown)} when calling {nameof(MainWindowViewModel)}.{nameof(_queue.SendButtonToUiAsync)}"));

            _logger.LogDebug("Shutting down");
            _cancellationTokenSource.Cancel();
        }
    }
}