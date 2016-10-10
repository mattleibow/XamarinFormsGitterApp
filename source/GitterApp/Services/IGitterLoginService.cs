using System.Threading.Tasks;

using GitterApi.Models;

namespace GitterApp.Services
{
	public interface IGitterLoginService
	{
		Task<User> GetLastUserAsync();

		Task<string> GetLastTokenAsync();

		Task<LoginResult> LoginAsync();

		Task LogoutAsync();
	}

	public class LoginResult
	{
		public string Token { get; set; }

		public User User { get; set; }
	}
}
