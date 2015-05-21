﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Thinktecture.IdentityServer.Core;
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
						scopes: Scopes.Get())
				});
			});

			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = "Cookies"
			});

			app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
			{
				Authority = "https://GBBLL3658.drl.local:44300/identity",
				ClientId = "mvc",
				Scope = "openid profile roles",
				RedirectUri = "https://GBBLL3658.drl.local:44300/",
				ResponseType = "id_token",

				SignInAsAuthenticationType = "Cookies",

				Notifications = new OpenIdConnectAuthenticationNotifications
				{
					SecurityTokenValidated = async n =>
					{
						var id = n.AuthenticationTicket.Identity;

						// we want to keep first name, last name, subject and roles
						var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
						var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
						var sub = id.FindFirst(Constants.ClaimTypes.Subject);
						var roles = id.FindAll(Constants.ClaimTypes.Role);

						// create new identity and set name and role claim type
						var nid = new ClaimsIdentity(
							id.AuthenticationType,
							Constants.ClaimTypes.GivenName,
							Constants.ClaimTypes.Role);

						nid.AddClaim(givenName);
						nid.AddClaim(familyName);
						nid.AddClaim(sub);
						nid.AddClaims(roles);

						// add some other app specific claim
						nid.AddClaim(new Claim("app_specific", "some data"));

						n.AuthenticationTicket = new AuthenticationTicket(
							nid,
							n.AuthenticationTicket.Properties);
					}
				}
			});

			AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
			JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
		}

		X509Certificate2 LoadCertificate()
		{
			return new X509Certificate2(
				string.Format(@"{0}\bin\localhost.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
		}
	}
}