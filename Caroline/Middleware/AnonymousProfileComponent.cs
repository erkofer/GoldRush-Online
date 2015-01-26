using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Threading.Tasks;
using Microsoft.Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string,object>, System.Threading.Tasks.Task>;

namespace Caroline.Middleware
{
    public class AnonymousProfileComponent
    {
        readonly AppFunc _next;
        public AnonymousProfileComponent(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            
            await _next(environment);
        }
    }
}