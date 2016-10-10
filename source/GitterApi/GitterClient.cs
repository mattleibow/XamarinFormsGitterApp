using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

using GitterApi.Models;

namespace GitterApi
{
	public class GitterClient
	{
		private const string GitterRootUrl = "https://api.gitter.im/v1/";

		private const string GitterCurrentUserEndpoint = "user";
		private const string GitterUserEndpoint = "users/{0}";
		private const string GitterRoomsEndpoint = "rooms";
		private const string GitterGroupSuggestedRoomsEndpoint = "groups/{0}/suggestedRooms";
		private const string GitterUserSuggestedRoomsEndpoint = "user/{0}/suggestedRooms";

		public GitterClient()
			: this(null)
		{
		}

		public GitterClient(string token)
		{
			Token = token;
		}

		public string Token { get; set; }

		public Task<User> GetCurrentUserAsync()
		{
			return GetAsync<User>(GitterCurrentUserEndpoint);
		}

		public Task<UserProfile> GetUserAsync(string username)
		{
			if (string.IsNullOrEmpty(username))
			{
				throw new ArgumentNullException(nameof(username));
			}

			var name = Uri.EscapeUriString(username);
			return GetAsync<UserProfile>(string.Format(GitterUserEndpoint, name));
		}

		public Task<Room[]> GetCurrentRoomsAsync()
		{
			return GetAsync<Room[]>(GitterRoomsEndpoint);
		}

		public async Task<Room[]> GetRoomsAsync(string searchQuery, int limit)
		{
			var query = Uri.EscapeUriString(searchQuery);
			var results = await GetAsync<SearchResults<Room[]>>($"{GitterRoomsEndpoint}?q={query}&limit={limit}");
			return results.Results;
		}

		public Task<Room[]> GetUserRoomSuggestionsAsync(string userId)
		{
			AssertIsValidId(userId, nameof(userId));

			return GetAsync<Room[]>(string.Format(GitterUserSuggestedRoomsEndpoint, userId));
		}

		public Task<Room[]> GetGroupRoomSuggestionsAsync(string groupId)
		{
			AssertIsValidId(groupId, nameof(groupId));

			return GetAsync<Room[]>(string.Format(GitterGroupSuggestedRoomsEndpoint, groupId));
		}

		private void AssertIsValidId(string guidId, string name)
		{
			if (string.IsNullOrWhiteSpace(guidId))
			{
				throw new ArgumentNullException($"Invalid {name}.", name);
			}

			// adjust for .NET GUID
			guidId = guidId.PadLeft(32, '0');

			Guid id;
			if (!Guid.TryParse(guidId, out id))
			{
				throw new ArgumentException($"Invalid {name}.", name);
			}
		}

		private async Task<T> GetAsync<T>(string endpoint)
		{
			using (var client = GetHttpClient())
			{
				var response = await client.GetAsync(endpoint).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					throw new GitterApiException(response);
				}

				var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonConvert.DeserializeObject<T>(result);
			}
		}

		private HttpClient GetHttpClient()
		{
			if (string.IsNullOrWhiteSpace(Token))
			{
				throw new InvalidOperationException("OAuth token not set.");
			}

			var client = new HttpClient();

			client.BaseAddress = new Uri(GitterRootUrl);

			client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

			return client;
		}
	}

	public class SearchResults<T>
	{
		[JsonProperty("results")]
		public T Results { get; set; }
	}
}
