using IdentityServer.Spike.Models;
using Thinktecture.IdentityServer.AspNetIdentity;

namespace IdentityServer.Spike
{
	public class UserService : AspNetIdentityUserService<User, string>
	{
		public UserService(ApplicationUserManager userManager) : base(userManager)
		{
		}
	}
}