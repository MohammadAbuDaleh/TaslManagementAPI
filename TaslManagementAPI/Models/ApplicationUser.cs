using Microsoft.AspNetCore.Identity;

namespace TaslManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserTask> Tasks { get; set; }
    }
}
