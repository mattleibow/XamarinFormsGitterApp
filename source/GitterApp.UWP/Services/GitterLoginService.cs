using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;
using Windows.Storage;
using Newtonsoft.Json;

using GitterApp.Services;

namespace GitterApp.Platform.Services
{
	public class GitterLoginService : GitterLoginServiceBase
	{
		public override Task<GitterUser> GetLastUserAsync()
		{
			object userJson = null;
			var settings = ApplicationData.Current.RoamingSettings;
			ApplicationDataContainer container;
			if (settings.Containers.TryGetValue(SettingsResourceKey, out container))
			{
				container.Values.TryGetValue(SettingsUserNameKey, out userJson);
			}

			var json = userJson as string;
			if (json == null)
			{
				return Task.FromResult<GitterUser>(null);
			}

			return Task.FromResult(JsonConvert.DeserializeObject<GitterUser>(json));
		}

		public override async Task<LoginResult> LoginAsync()
		{
			GitterUser user = null;

			// try and load an existing token
			var token = GetLocalToken();
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

			// return the info to the caller
			return new LoginResult
			{
				User = user,
				Token = token
			};
		}

		public override Task LogoutAsync()
		{
			var settings = ApplicationData.Current.RoamingSettings;
			settings.DeleteContainer(SettingsResourceKey);

			try
			{
				var vault = new PasswordVault();
				var credential = vault.Retrieve(SettingsResourceKey, SettingsUserNameKey);
				vault.Remove(credential);
			}
			catch
			{
			}

			return Task.FromResult(true);
		}

		private async Task<string> GetNewTokenAsync()
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
			var settings = new PasswordVault();
			settings.Add(new PasswordCredential(SettingsResourceKey, SettingsUserNameKey, result.AccessToken));

			// return the token
			return result.AccessToken;
		}

		private string GetLocalToken()
		{
			Debug.WriteLine("GitterLoginService: Retrieving saved token.");

			string token;
			try
			{
				// load the token from the roaming store
				var settings = new PasswordVault();
				var credential = settings.Retrieve(SettingsResourceKey, SettingsUserNameKey);
				credential.RetrievePassword();
				token = credential.Password;
			}
			catch
			{
				token = null;
			}
			return token;
		}

		private async Task<string> GetAuthCodeAsync()
		{
			var url =
				$"{AuthorizeUrl}" +
				$"?client_id={Uri.EscapeDataString(ClientId)}" +
				$"&redirect_uri={Uri.EscapeDataString(RedirectUrl)}" +
				$"&response_type=code" +
				$"&scope={Uri.EscapeDataString(Scope)}";

			var operation = WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(url), new Uri(RedirectUrl));
			var result = await operation.AsTask().ConfigureAwait(false);
			if (result.ResponseStatus == WebAuthenticationStatus.Success)
			{
				return result.ResponseData.Substring(result.ResponseData.IndexOf('=') + 1);
			}

			return null;
		}

		private async Task<GitterTokenResult> GetAuthTokenAsync(string code)
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
					{ "grant_type", "authorization_code"},
				});

				var response = await httpClient.PostAsync(new Uri(AccessTokenUrl), content).ConfigureAwait(false);
				var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonConvert.DeserializeObject<GitterTokenResult>(json);
			}
		}

		private async Task<GitterUser> GetUserAsync(string token)
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

				var settings = ApplicationData.Current.RoamingSettings;
				var container = settings.CreateContainer(SettingsResourceKey, ApplicationDataCreateDisposition.Always);
				settings.Values[SettingsUserNameKey] = json;

				return JsonConvert.DeserializeObject<GitterUser>(json);
			}
		}

		private class GitterTokenResult
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
