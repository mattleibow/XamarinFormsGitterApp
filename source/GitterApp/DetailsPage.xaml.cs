using System;
using System.Threading.Tasks;
using Xamarin.Forms;

using GitterApi.Models;

namespace GitterApp
{
	public partial class DetailsPage : NavigationPage
	{
		private ChatPage chatPage;

		public DetailsPage()
			: base(new ChatPage())
		{
			InitializeComponent();

			chatPage = (ChatPage)CurrentPage;
		}

		public void LoadRoom(GitterChatRoom chatRoom)
		{
			CurrentChatRoom = chatRoom;

			chatPage.Title = CurrentChatRoom?.Name;

			OnPropertyChanged(nameof(CurrentChatRoom));
		}

		public GitterChatRoom CurrentChatRoom { get; private set; }

		private async void OnRoomSettingsClicked(object sender, EventArgs e)
		{
			var type = CurrentChatRoom.IsPerson ? "Chat" : "Room";
			var fave = CurrentChatRoom.Room.Favourite ? $"Un-Star {type}" : $"Star {type}";
			var join = CurrentChatRoom.Room.RoomMember ? $"Leave {type}" : $"Join {type}";
			var profile = CurrentChatRoom.IsPerson ? "View User Profile" : "View Room Profile";

			var option = await DisplayActionSheet(CurrentChatRoom.Name, AppResources.DoneButtonText, null, fave, join, profile);
		}
	}
}
