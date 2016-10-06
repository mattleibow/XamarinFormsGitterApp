using Xamarin.Forms;

namespace GitterApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			var loginPage = new LoginPage();
			loginPage.LoggedIn += delegate
			{
			MainPage = new MainPage();
			};

			MainPage = loginPage;
		}

		protected override void OnStart()
		{
			// Handle when your app starts
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
}
