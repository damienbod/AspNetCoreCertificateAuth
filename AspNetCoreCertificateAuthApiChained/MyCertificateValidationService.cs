using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreCertificateAuthApi
{
    public class MyCertificateValidationService 
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            return CheckIfThumbprintIsValid(clientCertificate);
        }

        private bool CheckIfThumbprintIsValid(X509Certificate2 clientCertificate)
        {
            var listOfValidThumbprints = new List<string>
            {
                "CBF52D037D4CF0401F8EC8260C6382520D60EDD3",
                "BEE026E73A64D58943A66451D94389FA466169A4",
                "70D38240A71DD2882B4103E703F94D0B22285B0D",
                // valid but incorret DNS
                "ABF302B616CDEED10C53EA2C0E07CA1616814C68"
            };

            if (listOfValidThumbprints.Contains(clientCertificate.Thumbprint))
            {
                return true;
            }

            return false;
        }
    }
}
