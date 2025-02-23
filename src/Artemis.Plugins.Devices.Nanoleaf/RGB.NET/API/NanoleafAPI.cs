using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.API;

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
            return client.Send(new HttpRequestMessage(HttpMethod.Get, $"http://{address}:16021/api/v1/{authToken}/"))
                .Content
                .ReadFromJsonAsync<NanoleafInfo>()
                .Result;
        }
        catch
        {
            return null;
        }
    }

    public static (string address, ushort port) StartExternalControl(string address, string authToken,
        ExtControlVersion? version)
    {
        if (string.IsNullOrEmpty(address)) return (string.Empty, 0);

        using HttpClient client = new();
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"http://{address}:16021/api/v1/{authToken}/effects")
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

            var responseContent = client.Send(request).Content.ReadAsStringAsync().Result;
            var response = string.IsNullOrWhiteSpace(responseContent) ? null : JsonSerializer.Deserialize<NanoleafExternalControlResponse>(responseContent);
            return (response?.Address ?? address, response?.Port ?? 60222);
        }
        catch
        {
            return (string.Empty, 0);
        }
    }

    private class NanoleafExternalControlResponse
    {
        [JsonPropertyName("streamControlIpAddr")]
        public string? Address { get; set; }

        [JsonPropertyName("streamControlPort")]
        public ushort? Port { get; set; }

        [JsonPropertyName("streamControlProtocol")]
        public string? Protocol { get; set; }
    }
}