﻿using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace GitterApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private async void OnLoginCLicked(object sender, EventArgs e)
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
				Debug.WriteLine($"Logged in: {result.User.DisplayName}.");
			}
		}
	}
}
