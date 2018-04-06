using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SNITestApp
{
    public class Program
    {
        private static readonly CertificateManager _certificateManager = new CertificateManager();

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        httpsOptions.ServerCertificateSelector = SelectCertificate;
                    });
                })
                .UseStartup<Startup>();

        private static X509Certificate2 SelectCertificate(IFeatureCollection connectionFeatures, string serverName)
        {
            Console.WriteLine("Requested server: " + serverName);
            
            // Look up in the cert store
            return _certificateManager.GetCertificate(serverName);
        }
    }
}
