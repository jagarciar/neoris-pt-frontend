using System;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using Unity.Lifetime;
using NeorisFrontend.Services.Interfaces;
using NeorisFrontend.Services;
using NeorisFrontend.Infrastructure;

namespace NeorisFrontend
{
    /// <summary>
    /// Configuración del contenedor de inyección de dependencias Unity
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Obtiene el contenedor Unity configurado
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registra las interfaces y sus implementaciones en el contenedor Unity
        /// </summary>
        public static void RegisterComponents()
        {
            var container = Container;

            // Configurar el DependencyResolver de MVC para usar Unity
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        /// <summary>
        /// Registra todos los tipos (interfaces e implementaciones) en el contenedor
        /// </summary>
        /// <param name="container">Contenedor Unity</param>
        private static void RegisterTypes(IUnityContainer container)
        {
            // Registrar ApiClientService como Singleton (una única instancia)
            // Ya es singleton por diseño, pero registrarlo aquí para DI
            container.RegisterType<ApiClientService>(new ContainerControlledLifetimeManager());

            // Registrar los servicios con sus interfaces
            // TransientLifetimeManager = nueva instancia en cada resolución (por defecto)
            // ContainerControlledLifetimeManager = singleton
            // HierarchicalLifetimeManager = una instancia por contenedor hijo

            // Servicios transient (nueva instancia en cada request)
            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IAutorService, AutorService>();
            container.RegisterType<ILibroService, LibroService>();

            // Ejemplos de otros registros que podrías necesitar en el futuro:
            
            // Registrar un servicio como singleton
            // container.RegisterType<ICacheService, CacheService>(new ContainerControlledLifetimeManager());

            // Registrar con nombre específico
            // container.RegisterType<ILogger, FileLogger>("FileLogger");
            // container.RegisterType<ILogger, DatabaseLogger>("DbLogger");

            // Registrar una instancia específica
            // var configService = new ConfigurationService();
            // container.RegisterInstance<IConfigurationService>(configService);

            // Registrar con factory method
            // container.RegisterFactory<IConnection>(c => 
            // {
            //     return new SqlConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
            // });
        }
    }
}
