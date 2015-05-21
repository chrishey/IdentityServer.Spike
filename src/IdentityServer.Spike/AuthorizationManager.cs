using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace IdentityServer.Spike
{
	public class AuthorizationManager : ResourceAuthorizationManager
	{
		public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
		{
			switch (context.Resource.FirstOrDefault().Value)
			{
				case "ContactDetails":
					return AuthorizeContactDetails(context);
				default:
					return Nok();
			}
		}

		private Task<bool> AuthorizeContactDetails(ResourceAuthorizationContext context)
		{
			switch (context.Action.FirstOrDefault().Value)
			{
				case "Read":
					return Eval(context.Principal.HasClaim(x=>x.Value == "Geezer" && x.Type == "role"));
				case "Write":
					return Eval(context.Principal.HasClaim(x=>x.Type == "role" && x.Value == "Operator"));
				default:
					return Nok();
			}
		}
	}
}