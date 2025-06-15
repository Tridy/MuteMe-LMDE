using System;
using System.Threading;
using System.Threading.Tasks;

using AudioControl;

using Microsoft.Extensions.Logging;

using Utilities;

using Button = MuteMeControl.Services.Button;

namespace MuteMe.UI;

public class ButtonHostedService
{
    private readonly ILogger<ButtonHostedService> _logger;
    private readonly IMicrophone _microphone;
    private readonly IOptionsManager _optionsManager;
    private readonly IBackgroundQueue _queue;

    public ButtonHostedService(IMicrophone microphone, IOptionsManager optionsManager, IBackgroundQueue queue, ILogger<ButtonHostedService> logger)
    {
        _microphone = microphone;
        _optionsManager = optionsManager;
        _queue = queue;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting ButtonHostedService");

        Button button = Button.FromMicrophoneAndQueueAndLogger(_microphone, _queue, _logger);

        Task monitorProcess = button.MonitorAsync(_optionsManager, cancellationToken);

        while (cancellationToken.IsCancellationRequested == false && monitorProcess.Status == TaskStatus.Running)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200), CancellationToken.None);
        }

        // _logger.LogInformation("Stopping ButtonHostedService");
    }
}