using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace SNITestApp
{
    public class CertificateManager
    {
        private ConcurrentDictionary<string, X509Certificate2> _certificates = new ConcurrentDictionary<string, X509Certificate2>();

        public X509Certificate2 GetCertificate(string serverName)
        {
            return _certificates.GetOrAdd(serverName, LoadCertificate);
        }

        private static X509Certificate2 LoadCertificate(string serverName)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, serverName, validOnly: false);
            if(certificates.Count > 0)
            {
                return certificates[0];
            }
            else
            {
                return null;
            }
        }
    }
}
