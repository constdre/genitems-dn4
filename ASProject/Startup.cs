using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASProject.Startup))]
namespace ASProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            ConfigureAuth(app);
        }
    }
}
