using System.Security.Claims;
using TaslManagementAPI.Utilities;

namespace TaslManagementAPI.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public bool IsUserAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole(RolesConstants.Admin) ?? false;

    }
}
