using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Caroline.Api.SignalR
{
    /// <summary>
    /// If the user is not logged in, creates a profile whos username and password are saved in a cookie.
    /// </summary>
    public class GenerateAnonymousProfileAttribute : AuthorizeAttribute
    {
        string[] _usersSplit;
        string[] _rolesSplit;

        string[] UsersSplit
        {
            get { return _usersSplit ?? (_usersSplit = Mvc.GenerateAnonymousProfileAttribute.SplitString(Users)); }
        }

        string[] RolesSplit
        {
            get { return _rolesSplit ?? (_rolesSplit = Mvc.GenerateAnonymousProfileAttribute.SplitString(Roles)); }
        }

        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            return AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(request.GetHttpContext(), UsersSplit, RolesSplit);
        }

        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            throw new NotImplementedException();
        }
    }
}