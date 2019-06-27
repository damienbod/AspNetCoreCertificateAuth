using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreCertificateAuthApi
{
    public class MyCertificateValidationService
    {
        private readonly X509Certificate2 rootCertificate = new X509Certificate2(Path.Combine("root_ca_dev_damienbod.pfx"), "1234");
        private readonly X509Certificate2 intermediateCertificate = new X509Certificate2(Path.Combine("child_a_dev_damienbod.pfx"), "1234");

        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            if (clientCertificate.Issuer == rootCertificate.Issuer || 
                clientCertificate.Issuer == intermediateCertificate.Subject)
            {
                return true;
            }

            return false;
        }
    }
}
