using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace CreateChainedCertificates
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            var root = createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName { CommonName = "root_localhost", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
            root.FriendlyName = "root_localhost certificate";

            // Intermediate
            var intermediate = createClientServerAuthCerts.NewIntermediateChainedCertificate(
                new DistinguishedName { CommonName = "intermediate_localhost", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                2, "localhost", root);
            intermediate.FriendlyName = "intermediate_localhost certificate";

            var server = createClientServerAuthCerts.NewServerChainedCertificate(
                new DistinguishedName { CommonName = "server", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediate);

            var client = createClientServerAuthCerts.NewClientChainedCertificate(
                new DistinguishedName { CommonName = "client", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediate);
            server.FriendlyName = "server certificate";
            client.FriendlyName = "client certificate";

            var incorrectdns = createClientServerAuthCerts.NewServerChainedCertificate(
                new DistinguishedName { CommonName = "incorrectdns", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "incorrectdns", intermediate);
            client.FriendlyName = "incorrectdns certificate";

            string password = "1234";
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, root);
            File.WriteAllBytes("root_localhost.pfx", rootCertInPfxBtyes);

            var intermediateCertInPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, intermediate, root);
            File.WriteAllBytes("intermediate_localhost.pfx", intermediateCertInPfxBtyes);

            var serverCertInPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, server, intermediate);
            File.WriteAllBytes("server.pfx", serverCertInPfxBtyes);

            var clientCertInPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, client, intermediate);
            File.WriteAllBytes("client.pfx", clientCertInPfxBtyes);

            var incorrectdnsPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, incorrectdns, intermediate);
            File.WriteAllBytes("incorrectdns.pfx", incorrectdnsPfxBtyes);

            Console.WriteLine("Certificates exported to pfx and cer files");
        }
    }
}
