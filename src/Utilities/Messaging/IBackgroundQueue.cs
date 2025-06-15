using System.Threading;
using System.Threading.Tasks;

namespace Utilities;

public interface IBackgroundQueue
{
    ValueTask SendUiToButtonAsync(UiToButtonWorkItem workItem);
    ValueTask<UiToButtonWorkItem> ReceiveUiToButtonAsync(CancellationToken cancellationToken);

    ValueTask SendButtonToUiAsync(ButtonToUiWorkItemType itemType);
    ValueTask<ButtonToUiWorkItemType> ReceiveButtonToUiAsync(CancellationToken cancellationToken);
}