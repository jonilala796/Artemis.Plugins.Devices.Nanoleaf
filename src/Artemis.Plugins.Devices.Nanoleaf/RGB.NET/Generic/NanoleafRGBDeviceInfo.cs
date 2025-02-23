using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.API;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Generic;

// ReSharper disable once InconsistentNaming
public class NanoleafRGBDeviceInfo : IRGBDeviceInfo
{
    /// <summary>
    /// 
    /// </summary>
    public RGBDeviceType DeviceType { get; }

    public string DeviceName { get; }
    public string Manufacturer { get; }
    public string Model { get; }
    public object? LayoutMetadata { get; set; }
    public NanoleafInfo Info { get; }

    public NanoleafRGBDeviceInfo(NanoleafInfo info)
    {
        Info = info;
        DeviceName = info.Name;
        Manufacturer = "Nanoleaf";
        Model = info.Model;
        DeviceType = info.PanelLayout.Layout.PositionData[0].ShapeType == NanoleafShapeType.Lightstrip4D ? RGBDeviceType.LedStripe : RGBDeviceType.Unknown;
    }
}