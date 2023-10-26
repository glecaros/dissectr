using CommunityToolkit.Maui.Core.Primitives;
using System.Globalization;

namespace Dissectr.Converters;

public class MediaElementStateToPlaying : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (MediaElementState)value == MediaElementState.Playing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? MediaElementState.Playing : MediaElementState.Paused;
    }
}
