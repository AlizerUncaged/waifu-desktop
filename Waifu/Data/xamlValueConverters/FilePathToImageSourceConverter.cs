using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Waifu.Data.xamlValueConverters;

public class FilePathToImageSourceConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var filePath = value as string;

        if (!string.IsNullOrEmpty(filePath))
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // nah we dont need this
        throw new NotImplementedException("You're not supposed to see this...");
    }
}