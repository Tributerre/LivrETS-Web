using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using LivrETS.Models;
using LivrETS.Migrations;

[assembly: OwinStartupAttribute(typeof(LivrETS.Startup))]
namespace LivrETS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
