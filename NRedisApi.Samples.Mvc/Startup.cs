using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NRedisApi.Samples.Mvc.Startup))]
namespace NRedisApi.Samples.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
