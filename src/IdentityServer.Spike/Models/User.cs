using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityServer.Spike.Models
{
	public class User : IdentityUser
	{
		public int Age { get; set; }
		public bool IsAGeek { get; set; }
	}
}