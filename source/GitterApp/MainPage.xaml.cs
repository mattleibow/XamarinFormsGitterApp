using System;
using System.Diagnostics;
using Xamarin.Forms;

using GitterApp.Services;

namespace GitterApp
{
	public partial class MainPage : MasterDetailPage
	{
		public MainPage()
		{
			InitializeComponent();

			BindingContext = this;

			Master = new MasterPage();
			Detail = new DetailsPage(new ChatPage());
		}
	}
}
