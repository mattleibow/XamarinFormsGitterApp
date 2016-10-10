using System;
using System.Globalization;
using Xamarin.Forms;

namespace GitterApp
{
	public class UnreadCountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int && (int)value > 99)
			{
				return "∞";
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
