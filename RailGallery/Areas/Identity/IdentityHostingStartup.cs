using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(RailGallery.Areas.Identity.IdentityHostingStartup))]
namespace RailGallery.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}