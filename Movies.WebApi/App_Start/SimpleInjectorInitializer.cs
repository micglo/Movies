using System.Web.Http;
using Movies.Dal;
using Movies.Mapper.Factory.Category;
using Movies.Mapper.Factory.Client;
using Movies.Mapper.Factory.Movie;
using Movies.Mapper.Factory.Person;
using Movies.Mapper.Factory.Role;
using Movies.Mapper.Factory.User;
using Movies.Service.Client;
using Movies.Service.Logger;
using Movies.Service.Movie;
using Movies.Service.Role;
using Movies.Service.User;
using Movies.WebApi.IdentityConfig;
using Movies.WebApi.Utility.Exception.CustomException;
using Movies.WebApi.Utility.RequestMessageProvider;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Movies.WebApi
{
    public static class SimpleInjectorInitializer
    {
        public static Container Initialize(HttpConfiguration config)
        {
            var container = new Container();

            ConfigureContainer(container);

            // This is an extension method from the integration package.
            container.RegisterWebApiControllers(config);
            container.EnableHttpRequestMessageTracking(config);

            container.Verify();

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            return container;
        }

        private static void ConfigureContainer(Container container)
        {
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            //configure
            container.Register<IContext, Context>(Lifestyle.Scoped);

            ConfigureIdentityService(container);

            container.RegisterSingleton<IRequestMessageProvider>(new RequestMessageProvider(container));

            ConfigureServices(container);
            ConfigureMappers(container);
        }

        private static void ConfigureIdentityService(Container container)
        {
            container.Register(
                () => new UserStore(container.GetInstance<IContext>().GetUserCollection()), Lifestyle.Scoped);
            container.Register<UserManager>(Lifestyle.Scoped);
        }

        private static void ConfigureServices(Container container)
        {
            container.Register<ICustomException, CustomException>(Lifestyle.Scoped);
            container.Register<ILogService, LogService>(Lifestyle.Scoped);

            container.Register<IClientService, ClientService>(Lifestyle.Scoped);
            container.Register<IRoleService, RoleService>(Lifestyle.Scoped);
            container.Register<IUserService, UserService>(Lifestyle.Scoped);
            container.Register<IMovieService, MovieService>(Lifestyle.Scoped);
        }

        private static void ConfigureMappers(Container container)
        {
            container.Register<IClientFactory, ClientFactory>(Lifestyle.Scoped);
            container.Register<IRoleFactory, RoleFactory>(Lifestyle.Scoped);
            container.Register<IUserFactory, UserFactory>(Lifestyle.Scoped);
            container.Register<IMovieFactory, MovieFactory>(Lifestyle.Scoped);
            container.Register<ICategoryFactory, CategoryFactory>(Lifestyle.Scoped);
            container.Register<IPersonFactory, PersonFactory>(Lifestyle.Scoped);
        }
    }
}