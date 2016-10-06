using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GitterApp
{
	[ContentProperty("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		public string Text { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Text == null)
			{
				return string.Empty;
			}

			var translation = Resources.ResourceManager.GetString(Text);

			if (translation == null)
			{
#if DEBUG
				throw new ArgumentException($"Key '{Text}' was not found in the resource.", nameof(Text));
#else
				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
			}

			return translation;
		}
	}
}
