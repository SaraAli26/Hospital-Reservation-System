using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LeHospital.Startup))]
namespace LeHospital
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
