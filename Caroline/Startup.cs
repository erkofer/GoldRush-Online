using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Caroline.Startup))]
namespace Caroline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
