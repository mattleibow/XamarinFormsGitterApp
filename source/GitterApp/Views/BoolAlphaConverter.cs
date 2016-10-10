using System;
using System.Globalization;
using Xamarin.Forms;

namespace GitterApp
{
	public class BoolAlphaConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value == true)
			{
				return 1;
			}
			return parameter ?? 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
