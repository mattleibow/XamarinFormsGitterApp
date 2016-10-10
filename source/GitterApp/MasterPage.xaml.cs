using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

using GitterApi;
using GitterApi.Models;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GitterApp
{
	public partial class MasterPage : ContentPage
	{
		private GitterClient client;
		private GitterChatRoom currentChatRoom;
		private string searchText;
		private bool searchGitter;
		private GitterChatRoom[] gitterRooms;
		private GitterChatRoom[] userSuggestions;
		private GitterChatRoom[] searchResults;

		public MasterPage()
		{
			InitializeComponent();

			client = new GitterClient(App.CurrentApp.CurrentToken);
			MyRoomsCommand = new Command(OnMyRooms);
			SearchGitterCommand = new Command(OnSearchGitter);
			ChatRooms = new ObservableCollection<GroupedMenuItem>();

			BindingContext = this;
		}

		public Command MyRoomsCommand { get; }
		public Command SearchGitterCommand { get; }

		public ObservableCollection<GroupedMenuItem> ChatRooms { get; }

		public GitterChatRoom CurrentChatRoom
		{
			get { return currentChatRoom; }
			set
			{
				currentChatRoom = value;

				OnPropertyChanged();
			}
		}

		public string SearchText
		{
			get { return searchText; }
			set
			{
				// TODO: this just starts firing off requests
				if (searchText != value && SearchGitter)
				{
					StartSearch(searchText);
				}

				searchText = value;

				OnPropertyChanged();

				if (SearchGitter)
				{
					if (string.IsNullOrEmpty(SearchText))
					{
						SetChatRooms(userSuggestions);
					}
					else
					{
						var options = new[]
						{
							userSuggestions?.Where(r => r.Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) != -1),
							searchResults
						};
						var filtered = options.Where(o => o != null).SelectMany(s => s);
						SetChatRooms(filtered.ToArray());
					}
				}
				else
				{
					if (string.IsNullOrEmpty(SearchText))
					{
						SetChatRooms(gitterRooms);
					}
					else
					{
						var filtered = gitterRooms?.Where(r => r.Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) != -1);
						SetChatRooms(filtered?.ToArray());
					}
				}
			}
		}

		public bool SearchGitter
		{
			get { return searchGitter; }
			set
			{
				searchGitter = value;

				OnPropertyChanged();
			}
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			// the the current rooms
			var rooms = await client.GetCurrentRoomsAsync();
			gitterRooms = rooms.Select(r => new GitterChatRoom(r)).ToArray();
			SetChatRooms(gitterRooms);

			// refresh UI
			if (!SearchGitter)
			{
				OnMyRooms();
			}

			// get any suggestions
			var suggestions = await client.GetUserRoomSuggestionsAsync(App.CurrentApp.CurrentUser.Id);
			userSuggestions = suggestions.Select(r => new GitterChatRoom(r) { IsSuggestion = true }).ToArray();

			// refresh UI
			if (SearchGitter)
			{
				OnSearchGitter();
			}
		}

		private async Task StartSearch(string search)
		{
			if (string.IsNullOrWhiteSpace(search))
			{
				// clear the results
				searchResults = null;
			}
			else
			{
				try
				{
					// get the results
					var results = await client.GetRoomsAsync(search, 5);
					searchResults = results.Select(r => new GitterChatRoom(r)).ToArray();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}

			// refresh UI
			if (SearchGitter)
			{
				OnSearchGitter();
			}
		}

		private void OnMyRooms()
		{
			// update the menu
			SearchGitter = false;
			SearchText = SearchText;
		}

		private void OnSearchGitter()
		{
			// update the menu
			SearchGitter = true;
			SearchText = SearchText;
		}

		private void SetChatRooms(GitterChatRoom[] rooms)
		{
			IEnumerable<GroupedMenuItem> intermediate;

			if (rooms == null || rooms.Length == 0)
			{
				intermediate = new GroupedMenuItem[0];
			}
			else
			{
				if (SearchGitter)
				{
					intermediate = new[]
					{
						new GroupedMenuItem(AppResources.MenuGroupSearchResults, rooms.Where(r => !r.IsSuggestion)),
						new GroupedMenuItem(AppResources.MenuGroupSuggestions, rooms.Where(r => r.IsSuggestion)),
					};
				}
				else
				{
					intermediate = new[]
					{
						new GroupedMenuItem(AppResources.MenuGroupStarredUnreads, rooms.Where(r => r.IsStarred && r.HasUnread).OrderBy(r => r.Room.FavouriteIndex)),
						new GroupedMenuItem(AppResources.MenuGroupUnreads, rooms.Where(r => !r.IsStarred && r.HasUnread).OrderBy(r => r.Name.ToUpperInvariant())),
						new GroupedMenuItem(AppResources.MenuGroupStarred, rooms.Where(r => r.IsStarred && !r.HasUnread).OrderBy(r => r.Room.FavouriteIndex)),
						new GroupedMenuItem(AppResources.MenuGroupRooms, rooms.Where(r => !r.IsStarred && !r.HasUnread && !r.IsPerson).OrderBy(r => r.Name.ToUpperInvariant())),
						new GroupedMenuItem(AppResources.MenuGroupDirects, rooms.Where(r => !r.IsStarred && !r.HasUnread && r.IsPerson).OrderBy(r => r.Name.ToUpperInvariant()))
					};
				}
			}

			// remove empty items
			var results = intermediate.Where(g => g.Any()).ToArray();

			// handle no items
			if (results.Length == 0)
			{
				results = new[] { new GroupedMenuItem(AppResources.MenuGroupNoResults) };
			}

			// update the UI
			ChatRooms.Clear();
			var groups = new ObservableCollection<GroupedMenuItem>(results);
			foreach (var group in groups)
			{
				ChatRooms.Add(group);
			}
		}
	}

	public class GroupedMenuItem : ObservableCollection<GitterChatRoom>
	{
		public GroupedMenuItem(string name)
		{
			Name = name;
		}

		public GroupedMenuItem(string name, IEnumerable<GitterChatRoom> rooms)
			: base(rooms)
		{
			Name = name;
		}

		public string Name { get; set; }
	}

	public class GitterChatRoom
	{
		public GitterChatRoom(Room room)
		{
			Room = room;
			Type = Room?.Type ?? RoomType.Unknown;
		}

		public Room Room { get; set; }

		public bool IsSuggestion { get; set; }

		public RoomType Type { get; set; }

		public string Name => Room?.Name;

		public bool IsStarred => Room?.Favourite ?? false;
		public bool HasUnread => UnreadItems > 0;
		public int UnreadItems => (Room?.UnreadItems ?? 0) + (Room?.UnreadMentions ?? 0);
		public bool IsPerson => Type == RoomType.OneToOne || Type == RoomType.UserChannel;
	}
}
