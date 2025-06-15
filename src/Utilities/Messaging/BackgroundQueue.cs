using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Utilities;

public class BackgroundQueue : IBackgroundQueue
{
    private readonly Channel<ButtonToUiWorkItemType> _buttontoUiChannel;
    private readonly ILogger<BackgroundQueue> _logger;
    private readonly Channel<UiToButtonWorkItem> _uiToButtonChannel;

    public BackgroundQueue(ILogger<BackgroundQueue> logger)
    {
        _logger = logger;

        BoundedChannelOptions options = new BoundedChannelOptions(4)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _uiToButtonChannel = Channel.CreateBounded<UiToButtonWorkItem>(options);
        _buttontoUiChannel = Channel.CreateBounded<ButtonToUiWorkItemType>(options);
    }

    public async ValueTask SendUiToButtonAsync(UiToButtonWorkItem workItem)
    {
        await _uiToButtonChannel.Writer.WriteAsync(workItem);
    }

    public async ValueTask<UiToButtonWorkItem> ReceiveUiToButtonAsync(CancellationToken cancellationToken)
    {
        return await _uiToButtonChannel.Reader.ReadAsync(cancellationToken);
    }

    public async ValueTask SendButtonToUiAsync(ButtonToUiWorkItemType itemType)
    {
        await _buttontoUiChannel.Writer.WriteAsync(itemType);
    }

    public async ValueTask<ButtonToUiWorkItemType> ReceiveButtonToUiAsync(CancellationToken cancellationToken)
    {
        return await _buttontoUiChannel.Reader.ReadAsync(cancellationToken);
    }
}