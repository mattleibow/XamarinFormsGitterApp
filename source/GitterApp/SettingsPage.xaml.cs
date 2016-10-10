using System;
using System.Collections.Generic;
using GitterApi.Models;
using Xamarin.Forms;

namespace GitterApp
{
	public partial class SettingsPage : NavigationPage
	{
		public SettingsPage()
		{
			InitializeComponent();
		}

		private void OnProfileClicked(object sender, EventArgs e)
		{
			var up = new UserProfile(App.CurrentApp.CurrentUser);
			var profile = new ProfilePage(App.CurrentApp.CurrentUser.Username, up);
			//var nav = new NavigationPage(profile)
			//{
			//	BarBackgroundColor = (Color)App.CurrentApp.Resources["GitterBackgroundColor"],
			//	BarTextColor = (Color)App.CurrentApp.Resources["GitterForegroundColor"]
			//};
			Navigation.PushModalAsync(profile, true);
		}
	}
}
