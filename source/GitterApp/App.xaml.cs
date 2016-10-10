using System.Diagnostics;
using Xamarin.Forms;

using GitterApi.Models;
using GitterApp.Services;

namespace GitterApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = CurrentUser == null ? (Page)new LoginPage() : (Page)new MainPage();

			MessagingCenter.Subscribe<App, LoginResult>(CurrentApp, Messages.LoggedIn, (sender, result) =>
			{
				CurrentToken = result.Token;
				CurrentUser = result.User;

				Debug.WriteLine($"Logged in: {CurrentUser.DisplayName}.");

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

		public string CurrentToken { get; private set; }

		public User CurrentUser { get; private set; }

		protected override async void OnStart()
		{
			// Handle when your app starts

			// get the last user
			var service = DependencyService.Get<IGitterLoginService>();
			var token = await service.GetLastTokenAsync();
			var user = await service.GetLastUserAsync();
			if (token != null && user != null)
			{
				var result = new LoginResult
				{
					Token = token,
					User = user
				};
				MessagingCenter.Send(CurrentApp, Messages.LoggedIn, result);
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

		public const string ToggleMenu = "ToggleMenu";
	}
}
