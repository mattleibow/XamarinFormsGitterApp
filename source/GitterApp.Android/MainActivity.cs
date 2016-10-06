using Android.App;
using Android.Content.PM;
using Android.OS;

using GitterApp.Platform.Services;

namespace GitterApp.Platform
{
	[Activity(Label = "GitterApp", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			Localization.UpdateCulture();

			Xamarin.Forms.Forms.Init(this, bundle);
			Xamarin.Forms.DependencyService.Register<GitterLoginService>();

			LoadApplication(new App());
		}
	}
}

