namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Generic;

/// <summary>
/// Represents the data used to create a Nanoleaf-device.
/// </summary>
public interface INanoleafDeviceDefinition
{
    string Address { get; }
    string AuthToken { get; }
    
    
}