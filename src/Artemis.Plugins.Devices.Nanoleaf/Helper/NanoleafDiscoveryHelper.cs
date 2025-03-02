using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Artemis.Plugins.Devices.Nanoleaf.Helper
{
    /// <summary>
    /// Helper class for discovering Nanoleaf devices on the network using SSDP.
    /// </summary>
    public class NanoleafDiscoveryHelper
    {
        /// <summary>
        /// Discovers Nanoleaf devices on the network.
        /// </summary>
        /// <param name="waitFor">The time to wait for responses in milliseconds.</param>
        /// <returns>A list of tuples containing the address and model of the discovered devices.</returns>
        public static List<(string address, string model)> DiscoverDevices(int waitFor = 5000)
        {
            var multicastEndpoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
            var localEndpoint = new IPEndPoint(IPAddress.Any, 0);

            List<(string address, string model)> devices = [];

            var udpClient = new UdpClient();

            string buffer =
                "M-SEARCH * HTTP/1.1\r\nHost: 239.255.255.250:1900\r\nST: nanoleaf_aurora:light\r\nMan: \"ssdp:all\"\r\nMX: 3\r\n\r\n";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(buffer);

            udpClient.Send(data, data.Length, multicastEndpoint);
            udpClient.Client.ReceiveTimeout = waitFor;

            while (true)
            {
                try
                {
                    byte[] result = udpClient.Receive(ref localEndpoint);
                    string response = System.Text.Encoding.UTF8.GetString(result);
                    var ssdpResponse = ParseSsdpResponse(response);

                    if (!ssdpResponse.Headers.TryGetValue("ST", out string? st) || !st.Contains("nanoleaf") ||
                        !ssdpResponse.Headers.TryGetValue("Location", out string? location)) continue;
                    string address = new Uri(location).Host;
                    int port = new Uri(location).Port;
                    devices.Add((address, st.Split(':')[1].ToUpper()));
                }
                catch (SocketException)
                {
                    break;
                }
            }

            return devices;
        }

        /// <summary>
        /// Represents an SSDP response with headers.
        /// </summary>
        private class SsdpResponse
        {
            /// <summary>
            /// Gets or sets the headers of the SSDP response.
            /// </summary>
            public Dictionary<string, string> Headers { get; set; } = new();
        }

        /// <summary>
        /// Parses a raw SSDP response string into an <see cref="SsdpResponse"/> object.
        /// </summary>
        /// <param name="rawResponse">The raw SSDP response string.</param>
        /// <returns>An <see cref="SsdpResponse"/> object containing the parsed headers.</returns>
        private static SsdpResponse ParseSsdpResponse(string rawResponse)
        {
            var result = new SsdpResponse();
            string[] lines = rawResponse.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++)
            {
                int sepIndex = lines[i].IndexOf(':');
                if (sepIndex > -1)
                {
                    string headerName = lines[i].Substring(0, sepIndex).Trim();
                    string headerValue = lines[i].Substring(sepIndex + 1).Trim();
                    result.Headers[headerName] = headerValue;
                }
            }

            return result;
        }
    }
}