using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                var cert = new X509Certificate2(Path.Combine("sts_dev_cert.pfx"), "1234");
                options.ConfigureHttpsDefaults(o =>
                {
                    o.ServerCertificate = cert;
                    o.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                    //o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                });
            })
            //.ConfigureKestrel(options =>
            //{
            //    options.ConfigureHttpsDefaults(opt =>
            //    {
            //        opt.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
            //        var cert = new X509Certificate2(Path.Combine("sts_dev_cert.pfx"), "1234");

            //        opt.ServerCertificate = cert;
            //    });
            //})
            .Build();

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
