using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GitterApp.Services
{
	public interface IGitterLoginService
	{
		Task<GitterUser> GetLastUserAsync();

		Task<LoginResult> LoginAsync();

		Task LogoutAsync();
	}

	public abstract class GitterLoginServiceBase : IGitterLoginService
	{
		protected const string ClientId = "d68b3b028289803b0ae952605d69da5c78a5ac48";
		protected const string ClientSecret = "834cf9f75b3844ccf44a14e234c9f02e9bfaf17a";
		protected const string Scope = "flow,private";
		protected const string AuthorizeUrl = "https://gitter.im/login/oauth/authorize";
		protected const string RedirectUrl = "gitter-app://gitterapp/login/callback";
		protected const string AccessTokenUrl = "https://gitter.im/login/oauth/token";

		protected const string GitterCurrentUserUrl = "https://api.gitter.im/v1/user/me";

		protected const string SettingsResourceKey = "gitter-app";
		protected const string SettingsUserNameKey = "gitter-app";

		protected const string AccessTokenKey = "access_token";
		protected const string TokenTypeKey = "token_type";

		public abstract Task<GitterUser> GetLastUserAsync();

		public async Task<LoginResult> LoginAsync()
		{
			GitterUser user = null;

			// try and load an existing token
			var token = await GetLocalToken();
			if (token != null)
			{
				user = await GetUserAsync(token);
			}

			// either expired, or not logged in
			if (user == null)
			{
				token = await GetNewTokenAsync();
				user = await GetUserAsync(token);
			}

			// persist last user
			await SaveUser(user);

			// return the info to the caller
			return new LoginResult
			{
				User = user,
				Token = token
			};
		}

		public abstract Task LogoutAsync();

		public async Task<GitterUser> GetUserAsync(string token)
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				return null;
			}

			Debug.WriteLine("GitterLoginService: Retrieving user profile.");

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");

				var response = await httpClient.GetAsync(GitterCurrentUserUrl).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.Unauthorized)
					{
						// may have expired
					}
					else
					{
						// some other error
					}

					return null;
				}

				// everything is fine
				var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonConvert.DeserializeObject<GitterUser>(json);
			}
		}

		protected abstract Task<string> GetLocalToken();

		protected abstract Task SaveLocalToken(string token);

		protected abstract Task SaveUser(GitterUser user);

		protected async Task<string> GetNewTokenAsync()
		{
			Debug.WriteLine("GitterLoginService: Fetching new token.");

			// get the token
			var code = await GetAuthCodeAsync().ConfigureAwait(false);
			var result = await GetAuthTokenAsync(code).ConfigureAwait(false);
			if (result == null)
			{
				// the user has cancelled
				return null;
			}

			// save the token
			await SaveLocalToken(result.AccessToken);

			// return the token
			return result.AccessToken;
		}

		protected abstract Task<string> GetAuthCodeAsync();

		protected async Task<GitterTokenResult> GetAuthTokenAsync(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				return null;
			}

			using (var httpClient = new HttpClient())
			{
				var content = new FormUrlEncodedContent(new Dictionary<string, string> {
					{ "code", code},
					{ "client_id", ClientId},
					{ "client_secret", ClientSecret},
					{ "redirect_uri", RedirectUrl},
					{ "grant_type", "authorization_code"}
				});

				var response = await httpClient.PostAsync(new Uri(AccessTokenUrl), content).ConfigureAwait(false);
				var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonConvert.DeserializeObject<GitterTokenResult>(json);
			}
		}

		protected class GitterTokenResult
		{
			[JsonProperty("access_token")]
			public string AccessToken { get; set; }

			[JsonProperty("expires_in")]
			public string ExpiresIn { get; set; }

			[JsonProperty("token_type")]
			public string TokenType { get; set; }
		}
	}

	public class LoginResult
	{
		public string Token { get; set; }

		public GitterUser User { get; set; }
	}

	public class GitterUser
	{
		public string Id { get; set; }

		public string Username { get; set; }

		public string DisplayName { get; set; }

		public string Url { get; set; }

		public string AvatarUrlSmall { get; set; }

		public string AvatarUrlMedium { get; set; }
	}
}
