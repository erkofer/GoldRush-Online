using Caroline.Connections;
using Caroline.Middleware;
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
            app.Use<AnonymousProfileComponent>();
            SimpleInjectorInitializer.Initialize();
            app.MapSignalR<GameConnection>("/neon/komodo/dragons");
           // app.MapConnection<GameConnection>("/neon/komodo/dragons");
        }
    }
}
