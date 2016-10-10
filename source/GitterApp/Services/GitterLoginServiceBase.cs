using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

using GitterApi;
using GitterApi.Models;

namespace GitterApp.Services
{
	public abstract class GitterLoginServiceBase : IGitterLoginService
	{
		protected const string ClientId = "d68b3b028289803b0ae952605d69da5c78a5ac48";
		protected const string ClientSecret = "834cf9f75b3844ccf44a14e234c9f02e9bfaf17a";
		protected const string Scope = "flow,private";
		protected const string AuthorizeUrl = "https://gitter.im/login/oauth/authorize";
		protected const string RedirectUrl = "gitter-app://gitterapp/login/callback";
		protected const string AccessTokenUrl = "https://gitter.im/login/oauth/token";

		protected const string SettingsResourceKey = "gitter-app";
		protected const string SettingsUserNameKey = "gitter-app";

		protected const string AccessTokenKey = "access_token";
		protected const string TokenTypeKey = "token_type";

		public async Task<LoginResult> LoginAsync()
		{
			// try and load an existing token
			var token = await GetLastTokenAsync();
			if (token == null)
			{
				token = await GetNewTokenAsync();
			}

			var client = new GitterClient(token);
			var user = await client.GetCurrentUserAsync();

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

		public abstract Task<User> GetLastUserAsync();
		
		public abstract Task<string> GetLastTokenAsync();

		protected abstract Task SaveLocalToken(string token);

		protected abstract Task SaveUser(User user);

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
}
