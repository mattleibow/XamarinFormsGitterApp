using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitterApp.Platform
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
	}
}
