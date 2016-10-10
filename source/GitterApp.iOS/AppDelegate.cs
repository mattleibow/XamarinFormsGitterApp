using Foundation;
using UIKit;

using GitterApp.Platform.Services;
using System;

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

			Xamarin.Forms.MessagingCenter.Subscribe<App, bool>(this, Messages.ToggleMenu, OnToggleMenu);

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}

		private void OnToggleMenu(App app, bool presented)
		{
			UIApplication.SharedApplication.SetStatusBarHidden(presented, UIStatusBarAnimation.Slide);
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
