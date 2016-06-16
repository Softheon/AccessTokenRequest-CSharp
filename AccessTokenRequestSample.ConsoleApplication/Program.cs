using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace AccessTokenRequestSample.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string tokenEndpointUri = "https://developer.softheon.com/IdentityServer3.WebHost/core/connect/token";

            // Example Client Credentials
            string clientId = "3177AF10D71D4287BC0A8C8946F0BB75";
            string clientSecret = "17CA89F45D5E4E21926192ACD3D2B151";
            string scope = "exampleapi";

            string response = RequestAccessToken(tokenEndpointUri, clientId, clientSecret, scope);

            Console.WriteLine("Token Response:");
            Console.WriteLine(response);
        }

        /// <summary>
        /// Gets an Access Token from the given token endpoint URI using the given Client ID and Client Secret
        /// </summary>
        /// <param name="tokenEndpointUri">Token Endpoint URI</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <param name="scope">Scope</param>
        /// <returns>String</returns>
        private static string RequestAccessToken(string tokenEndpointUri, string clientId, string clientSecret, string scope)
        {
            // Required for .NET 4.5.1
            // SetSecurityProtocol();

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(tokenEndpointUri);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            string encodedClientCredentials = EncodeClientCredentials(clientId, clientSecret);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedClientCredentials);

            FormUrlEncodedContent encodedContent = EncodeFormContent(scope);

            CancellationToken cancellationToken = default(CancellationToken);

            HttpResponseMessage response = client.PostAsync(string.Empty, encodedContent, cancellationToken).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Sets TLS to version 1.2
        /// </summary>
        private static void SetSecurityProtocol()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                return true;
            };

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Encodes the Client ID and Client Secret as a Base 64 String.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <returns>String</returns>
        private static string EncodeClientCredentials(string clientId, string clientSecret)
        {
            Encoding encoding = Encoding.UTF8;
            string clientCredentials = String.Format("{0}:{1}", clientId, clientSecret);
            return Convert.ToBase64String(encoding.GetBytes(clientCredentials));
        }

        /// <summary>
        /// Form URL Encodes the grant_type and scope.
        /// </summary>
        /// <param name="scope">Scope</param>
        /// <returns>FormUrlEncodedContent</returns>
        private static FormUrlEncodedContent EncodeFormContent(string scope)
        {
            Dictionary<string, string> content = new Dictionary<string, string> {
                { "grant_type", "client_credentials" },
                { "scope", scope }
            };

            return new FormUrlEncodedContent(content);
        }
    }
}
