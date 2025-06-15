using System;
using System.Globalization;

using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Platform;

namespace MuteMe.UI.Converters;

public class StateToTrayIconConverter : IValueConverter
{
    private static readonly WindowIcon RedIcon = new(AssetLoader.Open(new Uri("avares://MuteMe.UI/Assets/red-mic.png")));
    private static readonly WindowIcon GreenIcon = new(AssetLoader.Open(new Uri("avares://MuteMe.UI/Assets/green-mic.png")));

    public static readonly StateToTrayIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? converterParameter, CultureInfo culture)
    {
        if (value is string mutedState)
        {
            return mutedState == "MUTED" ? RedIcon : GreenIcon;
        }

        return RedIcon;
    }

    public object? ConvertBack(object? value, Type targetType, object? converterParameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}