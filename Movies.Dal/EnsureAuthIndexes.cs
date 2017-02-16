using AspNet.Identity.MongoDB;

namespace Movies.Dal
{
    public static class EnsureAuthIndexes
    {
        public static void Exist()
        {
            IContext context = new Context();
            var users = context.GetUserCollection();
            var roles = context.GetRoleCollection();
            IndexChecks.EnsureUniqueIndexOnUserName(users);
            IndexChecks.EnsureUniqueIndexOnRoleName(roles);
        }
    }
}