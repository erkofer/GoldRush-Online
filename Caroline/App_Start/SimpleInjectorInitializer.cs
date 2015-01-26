using System.Reflection;
using System.Web.Mvc;
using Caroline;
using Caroline.App;
using Caroline.Persistence;
using GoldRush;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(SimpleInjectorInitializer), "Initialize")]

namespace Caroline
{
    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            // Did you know the container can diagnose your configuration? Go to: https://bit.ly/YE8OJj.
            var container = new Container();

            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            // container.RegisterPersistentConnections()...
            container.Verify();
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void InitializeContainer(Container container)
        {
            // GOLDRUSH
            container.Register<GameFactory, GameFactory>();

            // PERSISTENCE
            container.RegisterPerWebRequest<IUnitOfWork, UnitOfWork>();
            container.RegisterPerWebRequest<IUserRepository, UserRepository>();
            container.RegisterPerWebRequest<IGameRepository, GameRepository>();
            container.RegisterPerWebRequest<IGoldRushDbContext, GoldRushDbContext>();

            // APP
            container.RegisterPerWebRequest<IGameManager, GameManager>();
            container.RegisterPerWebRequest<IGoldRushCache,GoldRushCache>();
        }
    }
}