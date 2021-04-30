using System;
using System.Net.Http;

namespace VentasGpo.Portable.Interfaces
{
    public interface IHttpClientHandlerService
    {
        HttpClientHandler GetInsecureHandler();
    }
}
