using System.Diagnostics;
using Xamarin.Forms;

using GitterApp.Services;

namespace GitterApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = CurrentUser == null ? (Page)new LoginPage() : (Page)new MainPage();

			MessagingCenter.Subscribe<App, GitterUser>(CurrentApp, Messages.LoggedIn, (sender, user) =>
			{
				CurrentUser = user;

				Debug.WriteLine($"Logged in: {user.DisplayName}.");

				if (!(MainPage is MainPage))
				{
					MainPage = new MainPage();
				}
			});

			MessagingCenter.Subscribe<App>(CurrentApp, Messages.LoggedOut, sender =>
			{
				CurrentUser = null;

				if (!(MainPage is LoginPage))
				{
					MainPage = new LoginPage();
				}
			});
		}

		public static App CurrentApp => (App)Current;

		public GitterUser CurrentUser { get; private set; }

		protected override async void OnStart()
		{
			// Handle when your app starts

			// get the last user
			var service = DependencyService.Get<IGitterLoginService>();
			var user = await service.GetLastUserAsync();
			if (user != null)
			{
				MessagingCenter.Send(CurrentApp, Messages.LoggedIn, user);
			}
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}

	public static class Messages
	{
		public const string LoggedIn = "LoggedIn";
		public const string LoggedOut = "LoggedOut";
	}
}
