using Foundation;
using UIKit;

using GitterApp.Platform.Services;

namespace GitterApp.Platform
{
	[Register("AppDelegate")]
	public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Localization.UpdateCulture();

			Xamarin.Forms.Forms.Init();
			Xamarin.Forms.DependencyService.Register<GitterLoginService>();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}

		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if (GitterLoginService.CurrentAuthFlow?.ResumeAuthorizationFlow(url) == true)
			{
				return true;
			}

			return false;
		}
	}
}
