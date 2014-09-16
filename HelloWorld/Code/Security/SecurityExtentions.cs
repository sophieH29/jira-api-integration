using System.Security.Principal;

namespace HelloWorld.Code.Security
{
    public static class SecurityExtentions
    {
        public static CustomPrincipal ToCustomPrincipal(this IPrincipal principal)
        {
            return (CustomPrincipal) principal;
        }
    }
}