namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Generic;

/// <summary>
/// Represents a definition of a Nanoleaf device.
/// </summary>
public sealed class NanoleafDeviceDefinition(string address, string authToken) : INanoleafDeviceDefinition
{
    public string Address { get; } = address;
    public string AuthToken { get; } = authToken;
}