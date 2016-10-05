using System.Threading.Tasks;

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

		public abstract Task<LoginResult> LoginAsync();

		public abstract Task LogoutAsync();
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
