using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Spike
{
	public static class UserServiceExtensions
	{
		public static void ConfigureUserService(this IdentityServerServiceFactory factory, string connectionString)
		{
			factory.UserService = new Registration<IUserService, UserService>();
			factory.Register(new Registration<ApplicationUserManager>());
			factory.Register(new Registration<UserStore>());
			factory.Register(new Registration<Context>(resolver => new Context("identityDatabase")));
		}
	}
}