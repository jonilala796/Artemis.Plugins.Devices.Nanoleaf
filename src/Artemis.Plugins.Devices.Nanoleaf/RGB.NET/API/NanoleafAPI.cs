using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.API
{
    /// <summary>
    /// Partial implementation of the Nanoleaf-JSON-API
    /// </summary>
    public static class NanoleafAPI
    {
        /// <summary>
        /// Gets the data returned by the 'info' endpoint of the Nanoleaf-device.
        /// </summary>
        /// <param name="address">The address of the device to request data from.</param>
        /// <param name="authToken">The authentication token of the device to request data from.</param>
        /// <returns>The data returned by the Nanoleaf-device.</returns>
        public static NanoleafInfo? Info(string address, string authToken)
        {
            if (string.IsNullOrEmpty(address)) return null;

            using HttpClient client = new();
            try
            {
                var uri = new UriBuilder("http", address, 16021, $"/api/v1/{authToken}/").Uri;
                return client.Send(new HttpRequestMessage(HttpMethod.Get, uri))
                    .Content
                    .ReadFromJsonAsync<NanoleafInfo>()
                    .Result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Authenticates with the Nanoleaf device and retrieves an authentication token.
        /// </summary>
        /// <param name="address">The address of the device to authenticate with.</param>
        /// <returns>The authentication token, or null if authentication fails.</returns>
        public static string? Authenticate(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;

            using HttpClient client = new();
            try
            {
                var uri = new UriBuilder("http", address, 16021, "/api/v1/new").Uri;
                return client.Send(new HttpRequestMessage(HttpMethod.Post, uri))
                           .Content
                           .ReadFromJsonAsync<NanoleafAuthenticationResponse>()
                           .Result?
                           .AuthToken
                       ?? string.Empty;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Starts external control on the Nanoleaf device.
        /// </summary>
        /// <param name="address">The address of the device to control.</param>
        /// <param name="authToken">The authentication token of the device.</param>
        /// <param name="version">The version of the external control protocol.</param>
        /// <returns>A tuple containing the address and port for external control.</returns>
        public static (string address, ushort port) StartExternalControl(string address, string authToken,
            ExtControlVersion? version)
        {
            if (string.IsNullOrEmpty(address)) return (string.Empty, 0);

            using HttpClient client = new();
            try
            {
                var uri = new UriBuilder("http", address, 16021, $"/api/v1/{authToken}/effects").Uri;
                var request = new HttpRequestMessage(HttpMethod.Put, uri)
                {
                    Content = JsonContent.Create(new
                    {
                        write = new
                        {
                            command = "display", animType = "extControl",
                            extControlVersion = System.Enum.GetName(version ?? ExtControlVersion.v1)
                        }
                    })
                };

                string responseContent = client.Send(request).Content.ReadAsStringAsync().Result;
                var response = string.IsNullOrWhiteSpace(responseContent)
                    ? null
                    : JsonSerializer.Deserialize<NanoleafExternalControlResponse>(responseContent);
                return (response?.Address ?? address, response?.Port ?? 60222);
            }
            catch
            {
                return (string.Empty, 0);
            }
        }

        /// <summary>
        /// Represents the response from the Nanoleaf device for external control.
        /// </summary>
        private class NanoleafExternalControlResponse
        {
            /// <summary>
            /// Gets or sets the address for external control.
            /// </summary>
            [JsonPropertyName("streamControlIpAddr")]
            public string? Address { get; init; }

            /// <summary>
            /// Gets or sets the port for external control.
            /// </summary>
            [JsonPropertyName("streamControlPort")]
            public ushort? Port { get; init; }

            /// <summary>
            /// Gets or sets the protocol for external control.
            /// </summary>
            [JsonPropertyName("streamControlProtocol")]
            public string? Protocol { get; init; }
        }

        /// <summary>
        /// Represents the response from the Nanoleaf device for authentication.
        /// </summary>
        private class NanoleafAuthenticationResponse
        {
            /// <summary>
            /// Gets or sets the authentication token.
            /// </summary>
            [JsonPropertyName("auth_token")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string? AuthToken { get; init; }
        }
    }
}