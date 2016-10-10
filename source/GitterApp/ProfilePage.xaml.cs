using System;
using System.Diagnostics;
using Xamarin.Forms;

using GitterApi;
using GitterApi.Models;
using System.Threading.Tasks;

namespace GitterApp
{
	public partial class ProfilePage : ContentPage
	{
		private UserProfile user;

		public ProfilePage(string username, UserProfile user = null)
		{
			InitializeComponent();

			if (string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException(nameof(username));
			}

			User = user;
			Username = username;

			BindingContext = this;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await LoadUserAsync();
		}

		public string Username { get; private set; }

		public string AvatarUrl => "https://avatars.githubusercontent.com/u/594566";

		public UserProfile User
		{
			get { return user; }
			set
			{
				user = value;
				if (user != null)
				{
					Username = user.Username;
				}

				OnPropertyChanged();
			}
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			Navigation.PopModalAsync(true);
		}

		private async Task LoadUserAsync()
		{
			IsBusy = true;

			var client = new GitterClient(App.CurrentApp.CurrentToken);

			User = await client.GetUserAsync(Username);

			IsBusy = false;
		}
	}
}
