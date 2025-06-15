using Avalonia;
using Avalonia.Controls;

namespace MuteMe.UI.Views;

public partial class ColorSelector : UserControl
{
    public static readonly StyledProperty<string> SelectedColorProperty =
        AvaloniaProperty.Register<ColorSelector, string>(nameof(SelectedColor));

    public ColorSelector()
    {
        InitializeComponent();
    }

    public string SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
}