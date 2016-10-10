using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace GitterApp
{
	public partial class MainPage : MasterDetailPage
	{
		public MainPage()
		{
			InitializeComponent();

			BindingContext = this;

			var master = new MasterPage();
			var details = new DetailsPage();

			Master = master;
			Detail = details;

			Master.PropertyChanged += OnPropertyChanged;
			IsPresentedChanged += OnToggleMenu;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(MasterPage.CurrentChatRoom))
			{
				var master = Master as MasterPage;
				var details = Detail as DetailsPage;

				IsPresented = false;
				if (master != null && details != null)
				{
					details.LoadRoom(master.CurrentChatRoom);
				}
			}
		}

		private void OnToggleMenu(object sender, EventArgs e)
		{
			MessagingCenter.Send(App.CurrentApp, Messages.ToggleMenu, IsPresented);
		}
	}
}
