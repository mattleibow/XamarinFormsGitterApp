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

		public override Task<LoginResult> LoginAsync()
		{
			throw new NotImplementedException();
		}

		public override Task LogoutAsync()
		{
			throw new NotImplementedException();
		}
	}
}
