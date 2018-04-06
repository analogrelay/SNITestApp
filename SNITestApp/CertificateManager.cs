using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                var certificates =
                    store.Certificates.Find(X509FindType.FindBySubjectName, serverName, validOnly: false);
                if (certificates.Count > 0)
                {
                    return certificates[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var certsPath = Environment.GetEnvironmentVariable("ASPNETCORE_CERTIFICATES");
                var certPath = Path.Combine(certsPath, $"{serverName}.pfx");
                if (File.Exists(certPath))
                {
                    Console.WriteLine("Loading certificate: " + certPath);
                    return new X509Certificate2(certPath, password: "test");
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
