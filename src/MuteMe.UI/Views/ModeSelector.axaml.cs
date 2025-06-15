using Avalonia;
using Avalonia.Controls;

namespace MuteMe.UI.Views;

public partial class ModeSelector : UserControl
{
    public static readonly StyledProperty<string> SelectedModeProperty =
        AvaloniaProperty.Register<ModeSelector, string>(nameof(ModeSelector));

    public ModeSelector()
    {
        InitializeComponent();
    }

    public string SelectedMode
    {
        get => GetValue(SelectedModeProperty);
        set => SetValue(SelectedModeProperty, value);
    }
}