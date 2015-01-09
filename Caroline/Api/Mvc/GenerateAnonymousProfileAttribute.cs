using System.Linq;
using System.Web;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace Caroline.Api.Mvc
{
    /// <summary>
    /// If the user is not logged in, creates a profile whos username and password are saved in a cookie.
    /// </summary>
    public class GenerateAnonymousProfileAttribute : AuthorizeAttribute
    {
        private string[] _usersSplit;
        private string[] _rolesSplit;

        string[] UsersSplit
        {
            get { return _usersSplit ?? (_usersSplit = SplitString(Users)); }
        }

        string[] RolesSplit
        {
            get { return _rolesSplit ?? (_rolesSplit = SplitString(Roles)); }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(httpContext, UsersSplit, RolesSplit);
        }

        [CanBeNull]
        internal static string[] SplitString(string original)
        {
            // decompiled from AuthorizeAttribute.SplitString()
            if (string.IsNullOrEmpty(original))
                return null;
            return original.Split(new[]
            {
                ','
            }).Select(piece => new
            {
                piece,
                trimmed = piece.Trim()
            }).Where(param0 => !string.IsNullOrEmpty(param0.trimmed)).Select(param0 => param0.trimmed).ToArray();
        }
    }
}