using Caroline.Connections;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Caroline.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Caroline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR<KomodoConnection>("/neon/komodo/dragons");
           // app.MapConnection<KomodoConnection>("/neon/komodo/dragons");
        }
    }
}
