using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(AmdmProject.Startup1))]

namespace AmdmProject
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("SongContext");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            // Дополнительные сведения о настройке приложения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
