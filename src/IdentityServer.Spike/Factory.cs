using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core.Services.InMemory;

namespace IdentityServer.Spike
{
	public class Factory
	{
		public static IdentityServerServiceFactory Configure()
		{
			var factory = new IdentityServerServiceFactory
			{
				ClientStore =
					new Registration<IClientStore>(new InMemoryClientStore(Clients.Get())),
				ScopeStore =
					new Registration<IScopeStore>(new InMemoryScopeStore(Scopes.Get()))
			};

			return factory;
		}
	}
}