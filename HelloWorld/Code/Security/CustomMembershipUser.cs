using System;
using System.Web.Security;
using HelloWorld.App_Data;

namespace HelloWorld.Code.Security
{
    public class CustomMembershipUser : MembershipUser
    {
        #region Properties

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int UserRoleId { get; set; }

        public string UserRoleName { get; set; }

        #endregion

        public CustomMembershipUser(User user)
            : base("CustomMembershipProvider", user.Username, user.UserId, string.Empty, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserRoleId = user.UserRoleId;
            UserRoleName = user.UserRole.UserRoleName;
        }
    }
}