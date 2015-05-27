using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using IdentityManager;
using IdentityManager.AspNetIdentity;
using IdentityManager.Configuration;
using IdentityServer.Spike.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Constants = Thinktecture.IdentityServer.Core.Constants;

namespace IdentityServer.Spike
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = ConfigurationManager.AppSettings["AuthType"],
				LoginPath = new PathString("/Home/Login")
			});

			app.Map("/identity", idsrvApp =>
			{
				var factory = Factory.Configure();
				factory.ConfigureUserService("identityDatabase");

				idsrvApp.UseIdentityServer(new IdentityServerOptions
				{
					SiteName = "Embedded IdentityServer",
					SigningCertificate = LoadCertificate(),

					Factory = factory
				});
			});

			app.Map("/identitymanager", idm =>
			{
				var factory = new IdentityManagerServiceFactory
				{
					IdentityManagerService =
						new IdentityManager.Configuration.Registration<IIdentityManagerService, ApplicationIdentityManagerService>()
				};
				factory.Register(new IdentityManager.Configuration.Registration<ApplicationUserManager>());
				factory.Register(new IdentityManager.Configuration.Registration<UserStore>());
				factory.Register(new IdentityManager.Configuration.Registration<ApplicationRoleManager>());
				factory.Register(new IdentityManager.Configuration.Registration<ApplicationRoleStore>());
				factory.Register(new IdentityManager.Configuration.Registration<Context>(resolver=>new Context("identityDatabase")));
				idm.UseIdentityManager(new IdentityManagerOptions
				{
					Factory = factory,
					SecurityConfiguration = (Convert.ToBoolean(ConfigurationManager.AppSettings["localAuth"])) ?
					new LocalhostSecurityConfiguration() : new HostSecurityConfiguration
					{
						HostAuthenticationType = ConfigurationManager.AppSettings["AuthType"],
						NameClaimType = "name",
						RoleClaimType = "role",
						AdminRoleName = "admin"
					}
				});
			});

			app.UseResourceAuthorization(new AuthorizationManager());

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
						var givenName = id.FindFirst(Constants.ClaimTypes.Name);
						var roles = id.FindAll(Constants.ClaimTypes.Role);

						// create new identity and set name and role claim type
						var nid = new ClaimsIdentity(
							id.AuthenticationType,
							Constants.ClaimTypes.Name,
							Constants.ClaimTypes.Role);

						if(givenName != null)
							nid.AddClaim(givenName);

						var claims = roles as Claim[] ?? roles.ToArray();
						if(claims.Any())
							nid.AddClaims(claims);

						// add some other app specific claim
						nid.AddClaim(new Claim("app_specific", "some data"));

						n.AuthenticationTicket = new AuthenticationTicket(
							nid,
							n.AuthenticationTicket.Properties);

						nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
					},
					RedirectToIdentityProvider = async n =>
					{
						if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
						{
							var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token").Value;
							n.ProtocolMessage.IdTokenHint = idTokenHint;
						}
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

	public class ApplicationRoleStore : RoleStore<Role>
	{
		public ApplicationRoleStore(Context context) : base(context)
		{
			
		}
	}

	public class ApplicationRoleManager : RoleManager<Role>
	{
		public ApplicationRoleManager(ApplicationRoleStore roleStore) : base(roleStore)
		{
			
		}
	}

	public class ApplicationIdentityManagerService : AspNetIdentityManagerService<User, string, Role, string>
	{
		public ApplicationIdentityManagerService(ApplicationUserManager userManager, ApplicationRoleManager roleManager) : base(userManager, roleManager)
		{
			
		}
	}

	public class ApplicationUserManager : UserManager<User, string>
	{
		public ApplicationUserManager(UserStore store):base(store)
		{
			
		}
	}

	public class UserStore : UserStore<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
	{
		public UserStore(Context ctx)
			: base(ctx)
		{
		}
	}

	public class Context : IdentityDbContext<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
	{
		public Context(string connString)
			: base(connString)
		{
		}
	}

	public class Role : IdentityRole { }
}