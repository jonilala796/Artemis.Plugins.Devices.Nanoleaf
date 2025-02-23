using System.Collections.Generic;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.API;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Enum;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Helper;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Generic;

public sealed class NanoleafRGBDevice : AbstractRGBDevice<NanoleafRGBDeviceInfo>
{
    internal NanoleafRGBDevice(NanoleafRGBDeviceInfo deviceInfo, string address, ushort port,
        IDeviceUpdateTrigger updateTrigger)
        : base(deviceInfo,
            new NanoleafDeviceUpdateQueue(updateTrigger, address, port, deviceInfo.Info.PanelLayout.Layout.NumPanels,
                deviceInfo.Info.PanelLayout.Layout.PositionData[0].ShapeType.GetExtControlVersion(),
                deviceInfo.LedIdToIndex))
    {
        InitializeLayout();
    }

    private void InitializeLayout()
    {
        var positionData = DeviceInfo.Info.PanelLayout.Layout.PositionData;
        var i = 0;
        foreach (var position in positionData)
        {
            if (position.ShapeType.GetSideLength() != null && position.ShapeType.GetSideLength() > 0)
            {
                var ledId = position.ShapeType == NanoleafShapeType.Lightstrip4D
                    ? LedId.LedStripe1 + i++
                    : LedId.Custom1 + i++;
                DeviceInfo.LedIdToIndex.Add(ledId, position.PanelId);
                var sideLength = position.ShapeType.GetSideLength() ?? 0;
                var led = AddLed(ledId, new Point(position.X, position.Y), new Size(sideLength, sideLength));
                if (led != null)
                {
                    led.Shape = position.ShapeType.GetShape() ?? Shape.Rectangle;
                    led.Rotation = Rotation.FromDegrees(position.O);
                }
            }
        }

        Rotation = Rotation.FromDegrees(DeviceInfo.Info.PanelLayout.GlobalOrientation.Value);
    }
}