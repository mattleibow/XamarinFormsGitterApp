using System;
using System.Globalization;
using Xamarin.Forms;

namespace GitterApp
{
	public class BoolBoldConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value == true)
			{
				return FontAttributes.Bold;
			}
			return FontAttributes.None;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
