using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;
using Windows.Storage;
using Newtonsoft.Json;

using GitterApp.Services;

namespace GitterApp.Platform.Services
{
	public class GitterLoginService1 : GitterLoginServiceBase
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

		protected override Task SaveUser(GitterUser user)
		{
			var settings = ApplicationData.Current.RoamingSettings;
			var container = settings.CreateContainer(SettingsResourceKey, ApplicationDataCreateDisposition.Always);
			settings.Values[SettingsUserNameKey] = JsonConvert.SerializeObject(user);

			return Task.FromResult(true);
		}

		protected override Task SaveLocalToken(string token)
		{
			var settings = new PasswordVault();
			settings.Add(new PasswordCredential(SettingsResourceKey, SettingsUserNameKey, token));

			return Task.FromResult(true);
		}

		protected override Task<string> GetLocalToken()
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
			return Task.FromResult(token);
		}

		protected override Task<string> GetAuthCodeAsync()
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
	}
}
