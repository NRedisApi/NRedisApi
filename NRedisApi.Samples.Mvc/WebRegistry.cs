using System.Web;
using System.Web.Mvc;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace NRedisApi.Samples.Mvc
{
    public sealed class WebRegistry : Registry
    {
        public WebRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.LookForRegistries();
            });

            //Web             
            For<HttpContextBase>()
                .Use(() => HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current));
            For<ModelBinderMappingDictionary>().Use(GetModelBinders());
            For<IModelBinderProvider>().Use<StructureMapModelBinderProvider>();
            For<IFilterProvider>().Use<StructureMapFilterProvider>();

            IncludeRegistry<RedisRegistry>();
        }

        private static ModelBinderMappingDictionary GetModelBinders()
        {
            var binders = new ModelBinderMappingDictionary();
            return binders;
        }
    }
}