using System;
using System.Globalization;
using Xamarin.Forms;

using GitterApi.Models;

namespace GitterApp
{
	public class RoomTypeConverter : IValueConverter
	{
		private const string RoomChar = "#";
		private const string PersonChar = "●";
		private const string NoneChar = " ";

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is RoomType)
			{
				var roomType = (RoomType)value;

				switch (roomType)
				{
					case RoomType.Organisation:
					case RoomType.Repository:
					case RoomType.OrganisationChannel:
					case RoomType.RepositoryChannel:
						return RoomChar;
					case RoomType.OneToOne:
					case RoomType.UserChannel:
						return PersonChar;
				}
			}
			return NoneChar;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
