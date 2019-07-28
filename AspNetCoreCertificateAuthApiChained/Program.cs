using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreCertificateAuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
            => WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureKestrel(options =>
            {
                var cert = new X509Certificate2(Path.Combine("root_ca_dev_damienbod.pfx"), "1234");
                options.ConfigureHttpsDefaults(o =>
                {
                    o.ServerCertificate = cert;
                    o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    o.ClientCertificateValidation += (msg, cert, chain) => {
                        return true;
                    };
                });
            })
            .Build();
    }
}
