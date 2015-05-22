using System.Collections.Generic;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Spike
{
	public static class Clients
	{
		public static IEnumerable<Client> Get()
		{
			return new[]
        {
            new Client 
            {
                Enabled = true,
                ClientName = "MVC Client",
                ClientId = "mvc",
                Flow = Flows.Implicit,

                RedirectUris = new List<string>
                {
                    "https://GBBLL3658.drl.local:44300/"
                },
				PostLogoutRedirectUris = new List<string>
				{
					"https://GBBLL3658.drl.local:44300/"
				}
            }
        };
		}
	}
}