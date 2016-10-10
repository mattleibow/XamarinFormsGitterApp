using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using Security;
using UIKit;
using Newtonsoft.Json;
using OpenId.AppAuth;

using GitterApi.Models;
using GitterApp.Services;

namespace GitterApp.Platform.Services
{
	public class GitterLoginService : GitterLoginServiceBase
	{
		public const string LastUserKey = "LastUser";

		public static IOIDAuthorizationFlowSession CurrentAuthFlow;

		public override Task<User> GetLastUserAsync()
		{
			User user = null;

			var json = GetLocalValue(LastUserKey);
			if (json != null)
			{
				user = JsonConvert.DeserializeObject<User>(json);
			}

			return Task.FromResult(user);
		}

		public override Task LogoutAsync()
		{
			throw new NotImplementedException();
		}

		protected override Task SaveUser(User user)
		{
			var json = JsonConvert.SerializeObject(user);
			SetLocalValue(LastUserKey, json);

			json = GetLocalValue(LastUserKey);

			return Task.FromResult(true);
		}

		public override Task<string> GetLastTokenAsync()
		{
			return Task.FromResult(GetLocalValue(SettingsUserNameKey));
		}

		protected override Task SaveLocalToken(string token)
		{
			SetLocalValue(SettingsUserNameKey, token);

			return Task.FromResult(true);
		}

		protected override Task<string> GetAuthCodeAsync()
		{
			var tcs = new TaskCompletionSource<string>();

			var config = new OIDServiceConfiguration(new NSUrl(AuthorizeUrl), new NSUrl(AccessTokenUrl));
			var request = new OIDAuthorizationRequest(config, ClientId, Scope.Split(','), new NSUrl(RedirectUrl), OIDResponseType.Code, null);

			var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
			CurrentAuthFlow = OIDAuthorizationService.PresentAuthorizationRequest(request, vc, (authResponse, error) =>
			{
				if (error != null)
				{
					Debug.WriteLine("OIDAuthorizationService.PresentAuthorizationRequest error: " + error);
				}

				tcs.SetResult(authResponse?.AuthorizationCode);
			});

			return tcs.Task;
		}

		private string GetLocalValue(string key)
		{
			var query = new SecRecord(SecKind.GenericPassword);
			query.Service = SettingsResourceKey;
			query.Account = key;

			SecStatusCode result;
			var record = SecKeyChain.QueryAsRecord(query, out result);

			if (result != SecStatusCode.Success || record == null)
			{
				return null;
			}

			return record.ValueData.ToString(NSStringEncoding.UTF8);
		}

		private void SetLocalValue(string key, string value)
		{
			var query = new SecRecord(SecKind.GenericPassword);
			query.Service = SettingsResourceKey;
			query.Account = key;

			SecStatusCode result;
			var record = SecKeyChain.QueryAsRecord(query, out result);
			if (result == SecStatusCode.Success && record != null)
			{
				SecKeyChain.Remove(record);
			}

			record = new SecRecord(SecKind.GenericPassword);
			record.Accessible = SecAccessible.AfterFirstUnlock;
			record.Service = SettingsResourceKey;
			record.Account = key;
			record.ValueData = NSData.FromString(value, NSStringEncoding.UTF8);

			result = SecKeyChain.Add(record);
		}
	}
}
