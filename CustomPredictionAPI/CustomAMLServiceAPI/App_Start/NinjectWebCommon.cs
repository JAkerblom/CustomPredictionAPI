[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CustomerAMLServiceAPI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CustomerAMLServiceAPI.App_Start.NinjectWebCommon), "Stop")]

namespace CustomerAMLServiceAPI.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    using CustomerAMLServiceAPI.Controllers;
    using CustomerAMLServiceAPI.Models;
    using CustomerAMLServiceAPI.Services;
    using System.Web.Http;
    using WebApiContrib.IoC.Ninject;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(); // you'll add modules to the parameter list here
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
                //GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        ///// <summary>
        ///// Load your modules or register your services here!
        ///// </summary>
        ///// <param name="kernel">The kernel.</param>
        //private static void RegisterServices(IKernel kernel)
        //{
        //}
        private static void RegisterServices(IKernel kernel)
        {
            //kernel.Bind<IPredictionDataContext>().To<PredictionDataContext>().InRequestScope();
            //kernel.Bind<PredictionDataContext>().To<PredictionDataContext>().InRequestScope();
            //kernel.Bind<IPredictionDataRepository>().To<PredictionDataRepository>().InRequestScope();
            kernel.Bind<IAzureService>().To<AzureService>().InRequestScope();
            //kernel.Bind<IModel>().To<BaseModel>().InRequestScope();
        }
    }
}