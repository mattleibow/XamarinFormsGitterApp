using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GitterApp
{
	public partial class MasterPage : ContentPage
	{
		private GitterChatRoom currentChatRoom;

		public MasterPage()
		{
			InitializeComponent();

			ChatRooms = new ObservableCollection<GroupedMenuItem>();

			BindingContext = this;
		}

		public ObservableCollection<GroupedMenuItem> ChatRooms { get; private set; }

		public GitterChatRoom CurrentChatRoom
		{
			get { return currentChatRoom; }
			set
			{
				currentChatRoom = value;

				OnPropertyChanged();
			}
		}

		public class GroupedMenuItem : ObservableCollection<GitterChatRoom>
		{
			public string Name { get; set; }
		}
	}

	public class GitterChatRoom
	{
		public string Name { get; set; }
	}
}
