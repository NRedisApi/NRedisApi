using StructureMap;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NRedisApi.Samples.Mvc
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(ObjectFactory.Container)); // for MVC
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            ObjectFactory.Container.Configure(x =>
            {
                x.For<IControllerActivator>().Use<StructureMapControllerActivator>();
                x.Scan(scan =>
                {
                    scan.LookForRegistries();
                    scan.Assembly("NRedisApi.Samples.Mvc");
                });

            });
        }
    }
}
