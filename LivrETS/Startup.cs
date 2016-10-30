using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LivrETS.Startup))]
namespace LivrETS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureHangFire(app);
        }
    }
}
