using System.Linq;

namespace HelloWorld.Code.Security
{
    public class UserRoleAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public UserRoleAuthorizeAttribute(){}

        public UserRoleAuthorizeAttribute(params UserRole[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }

    public enum UserRole
    {
        Administrator = 1,
        User = 2
    }
}