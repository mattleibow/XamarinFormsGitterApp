using System;
using System.Globalization;
using System.Threading;

namespace GitterApp.Platform
{
	public static class Localization
	{
		public static void UpdateCulture()
		{
			var netLanguage = "en";
#if __IOS__
			if (Foundation.NSLocale.PreferredLanguages.Length > 0)
			{
				var pref = Foundation.NSLocale.PreferredLanguages[0];
				netLanguage = GetNetLanguage(pref);
			}
#elif __ANDROID__
			var androidLocale = Java.Util.Locale.Default;
			netLanguage = GetNetLanguage(androidLocale.ToString());
#endif

			// this gets called a lot - try/catch can be expensive so consider caching or something
			CultureInfo ci = null;
			try
			{
				ci = new CultureInfo(netLanguage);
			}
			catch (CultureNotFoundException)
			{
				var dashIndex = netLanguage.IndexOf("-", StringComparison.Ordinal);
				if (dashIndex > 0)
				{
					netLanguage = netLanguage.Split('-')[0];
				}

				// iOS locale not valid .NET culture (eg. "en-ES" : English in Spain)
				// fallback to first characters, in this case "en"
				try
				{
					var fallback = GetFallbackLanguage(netLanguage);
					ci = new CultureInfo(fallback);
				}
				catch (CultureNotFoundException)
				{
					// iOS language not valid .NET culture, falling back to English
					ci = new CultureInfo("en");
				}
			}

			Thread.CurrentThread.CurrentCulture = ci;
			Thread.CurrentThread.CurrentUICulture = ci;
		}

		private static string GetNetLanguage(string nativeLanguage)
		{
			var netLanguage = nativeLanguage.Replace("_", "-");

			// certain languages need to be converted to CultureInfo equivalent
			switch (nativeLanguage)
			{
				case "ms-BN": // "Malaysian (Brunei)" not supported .NET culture
				case "ms-MY": // "Malaysian (Malaysia)" not supported .NET culture
				case "ms-SG": // "Malaysian (Singapore)" not supported .NET culture
					netLanguage = "ms"; // closest supported
					break;
				case "in-ID": // "Indonesian (Indonesia)" has different code in  .NET
					netLanguage = "id-ID"; // correct code for .NET
					break;
				case "gsw-CH": // "Schwiizertüütsch (Swiss German)" not supported .NET culture
					netLanguage = "de-CH"; // closest supported
					break;
					// add more application-specific cases here (if required)
					// ONLY use cultures that have been tested and known to work
			}

			return netLanguage;
		}

		private static string GetFallbackLanguage(string languageCode)
		{
			var netLanguage = languageCode; // use the first part of the identifier (two chars, usually);

			switch (languageCode)
			{
				case "pt":
					netLanguage = "pt-PT"; // fallback to Portuguese (Portugal)
					break;
				case "gsw":
					netLanguage = "de-CH"; // equivalent to German (Switzerland) for this app
					break;
					// add more application-specific cases here (if required)
					// ONLY use cultures that have been tested and known to work
			}

			return netLanguage;
		}
	}
}
