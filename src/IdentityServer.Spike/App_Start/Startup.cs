using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer.Spike
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.Map("/identity", idsrvApp =>
			{
				idsrvApp.UseIdentityServer(new IdentityServerOptions
				{
					SiteName = "Embedded IdentityServer",
					SigningCertificate = LoadCertificate(),

					Factory = InMemoryFactory.Create(
						users: Users.Get(),
						clients: Clients.Get(),
						scopes: StandardScopes.All)
				});
			});

			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = "Cookies"
			});

			app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
			{
				Authority = "https://localhost:44300/identity",
				ClientId = "mvc",
				RedirectUri = "https://localhost:44300/",
				ResponseType = "id_token",

				SignInAsAuthenticationType = "Cookies"
			});
		}

		X509Certificate2 LoadCertificate()
		{
			return new X509Certificate2(
				string.Format(@"{0}\bin\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
		}
	}
}