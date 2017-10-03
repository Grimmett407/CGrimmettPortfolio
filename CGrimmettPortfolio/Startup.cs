using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CGrimmettPortfolio.Startup))]
namespace CGrimmettPortfolio
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
