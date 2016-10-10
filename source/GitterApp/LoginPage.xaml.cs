using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using GitterApp.Services;
using Xamarin.Forms;

namespace GitterApp
{
	public partial class LoginPage : ContentPage
	{		private ICommand loginCommand;

		public LoginPage()
		{
			InitializeComponent();

			BindingContext = this;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			AnimateLogoIn();
		}

		public ICommand LogInCommand => (loginCommand ?? (loginCommand = new Command(OnLogIn)));

		private async Task AnimateLogoIn()
		{
			// 0%
			logo.TranslationY = -Height;
			leftArm.TranslationY = 0;
			rightArm.TranslationY = 0;

			// delay for 1s
			await Task.Delay(1000);

			// 40%
			await logo.TranslateTo(0, 0, 400);
			// 50%
			await Task.WhenAll(
				logo.TranslateTo(0, 4, 100),
				leftBody.LayoutTo(new Rectangle(leftBody.X, leftBody.Y, leftBody.Width, 16), 100),
				rightBody.LayoutTo(new Rectangle(rightBody.X, rightBody.Y, rightBody.Width, 16), 100),
				leftArm.LayoutTo(new Rectangle(leftArm.X, 4, leftArm.Width, 11), 100),
				rightArm.LayoutTo(new Rectangle(rightArm.X, 4, rightArm.Width, 11), 100));
			// 60%
			await Task.WhenAll(
				logo.TranslateTo(0, 0, 100),
				leftBody.LayoutTo(new Rectangle(leftBody.X, leftBody.Y, leftBody.Width, 20), 100),
				rightBody.LayoutTo(new Rectangle(rightBody.X, rightBody.Y, rightBody.Width, 20), 100),
				leftArm.LayoutTo(new Rectangle(leftArm.X, 4, leftArm.Width, 11), 100),
				rightArm.LayoutTo(new Rectangle(rightArm.X, 4, rightArm.Width, 11), 100));
			// 90%
			await Task.Delay(300);
			// 100%
			await leftArm.LayoutTo(new Rectangle(leftArm.X, 0, leftArm.Width, 11), 100);

			// fade label in
			await logoText.FadeTo(1, 1000);
		}

		private async void OnLogIn()
		{
			Debug.WriteLine("Logging in...");

			var service = DependencyService.Get<IGitterLoginService>();

			var user = await service.GetLastUserAsync();
			if (user != null)
			{
				Debug.WriteLine($"Last user: {user.DisplayName}.");
			}

			var result = await service.LoginAsync();

			if (result.User == null)
			{
				Debug.WriteLine($"Log in cancelled.");
			}
			else
			{
				MessagingCenter.Send(App.CurrentApp, Messages.LoggedIn, result);
			}
		}

		private async void OnLogoutCLicked(object sender, EventArgs e)
		{
			Debug.WriteLine("Logging out...");

			var service = DependencyService.Get<IGitterLoginService>();

			var user = await service.GetLastUserAsync();
			if (user != null)
			{
				Debug.WriteLine($"Last user: {user.DisplayName}.");
			}

			await service.LogoutAsync();

			Debug.WriteLine($"Logged out.");
		}
	}
}
