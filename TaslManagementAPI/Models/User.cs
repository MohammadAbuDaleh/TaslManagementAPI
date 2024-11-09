using Microsoft.AspNetCore.Identity;

namespace TaslManagementAPI.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<UserTask> Tasks { get; set; }
    }
}
