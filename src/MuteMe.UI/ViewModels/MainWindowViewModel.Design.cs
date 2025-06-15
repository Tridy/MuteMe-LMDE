using CommunityToolkit.Mvvm.ComponentModel;

namespace MuteMe.UI.ViewModels;

// ReSharper disable InconsistentNaming
public partial class DesignTimeMainWindowViewModel : ViewModelBase
{
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

    public DesignTimeMainWindowViewModel()
    {
        selectedMutedColor = "Red";
        selectedUnmutedColor = "Green";
        selectedMode = "Toggle";
        state = "UNMUTED";
        isConnected = true;
    }

    public void OnShuttingDown()
    {
    }
}