using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Animals.Startup))]
namespace Animals
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
 
        }
    }
}
