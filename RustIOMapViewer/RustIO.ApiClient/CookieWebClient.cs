using System;
using System.Net;

namespace RustIO.ApiClient
{
    public class CookieWebClient : WebClient
    {
        private readonly CookieContainer cookieContainer = new CookieContainer();
        
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            HttpWebRequest webRequest = request as HttpWebRequest;

            if (webRequest != null)
            {
                webRequest.CookieContainer = cookieContainer;
                webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            return request;
        }
    }
}