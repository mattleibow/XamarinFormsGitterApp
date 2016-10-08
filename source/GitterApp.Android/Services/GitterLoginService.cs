using System;
using System.Threading.Tasks;

using GitterApp.Services;

namespace GitterApp.Platform.Services
{
	public class GitterLoginService : GitterLoginServiceBase
	{
		public override Task<GitterUser> GetLastUserAsync()
		{
			throw new NotImplementedException();
		}

		public override Task LogoutAsync()
		{
			throw new NotImplementedException();
		}

		protected override Task<string> GetAuthCodeAsync()
		{
			throw new NotImplementedException();
		}

		protected override Task<string> GetLocalToken()
		{
			throw new NotImplementedException();
		}

		protected override Task SaveLocalToken(string token)
		{
			throw new NotImplementedException();
		}

		protected override Task SaveUser(GitterUser user)
		{
			throw new NotImplementedException();
		}
	}
}
