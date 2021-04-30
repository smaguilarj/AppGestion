using System;
using System.IO;
using System.Net.Http;
using Java.Security;
using Java.Security.Cert;
using Javax.Net.Ssl;
using Xamarin.Android.Net;
using Xamarin.Forms;
using VentasGpo.Portable.Interfaces;
using VentasGpo.Droid.Config;

[assembly: Dependency(typeof(HttpClientHandlerService))]
namespace VentasGpo.Droid.Config
{
    public class HttpClientHandlerService : IHttpClientHandlerService
    {
        public HttpClientHandler GetInsecureHandler()
        {
            return new DroidTlsClientHandler();
        }
    }

    public class DroidTlsClientHandler : AndroidClientHandler
    {
        private TrustManagerFactory _trustManagerFactory;
        private KeyManagerFactory _keyManagerFactory;
        private KeyStore _keyStore;

        protected override TrustManagerFactory ConfigureTrustManagerFactory(KeyStore keyStore)
        {
            if (_trustManagerFactory != null)
            {
                return _trustManagerFactory;
            }

            _trustManagerFactory = TrustManagerFactory
                .GetInstance(TrustManagerFactory.DefaultAlgorithm);

            _trustManagerFactory.Init(keyStore);

            return _trustManagerFactory;
        }

        protected override KeyManagerFactory ConfigureKeyManagerFactory(KeyStore keyStore)
        {
            if (_keyManagerFactory != null)
            {
                return _keyManagerFactory;
            }

            _keyManagerFactory = KeyManagerFactory
                .GetInstance(KeyManagerFactory.DefaultAlgorithm);

            _keyManagerFactory.Init(keyStore, null);

            return _keyManagerFactory;
        }

        protected override KeyStore ConfigureKeyStore(KeyStore keyStore)
        {
            if (_keyStore != null)
            {
                return _keyStore;
            }

            _keyStore = KeyStore.GetInstance(KeyStore.DefaultType);
            _keyStore.Load(null, null);

            CertificateFactory cff = CertificateFactory.GetInstance("X.509");
            Certificate cert;

            // Add your Certificate to the Assets folder and address it here by its name
            //Certificado actual vigente Nov-21
            using (Stream certStream = Android.App.Application.Context.Assets.Open("herdez-com.pem"))
            {
                cert = cff.GenerateCertificate(certStream);
            }

            _keyStore.SetCertificateEntry("TrustedCert", cert);

            return _keyStore;
        }
    }
}
