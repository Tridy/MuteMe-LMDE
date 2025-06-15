using System;
using System.Globalization;

using Avalonia.Data.Converters;

namespace MuteMe.UI.Converters;

public class StateToIsVisibleConverter : IValueConverter
{
    public static readonly StateToIsVisibleConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? converterParameter, CultureInfo culture)
    {
        if (converterParameter is string parameter)
        {
            return value?.ToString() == parameter;
        }

        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? converterParameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}