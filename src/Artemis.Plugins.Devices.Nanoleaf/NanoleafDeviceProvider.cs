using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Generic;
using Artemis.Plugins.Devices.Nanoleaf.Settings;
using Artemis.Plugins.Devices.Nanoleaf.ViewModels;
using RGB.NET.Core;
using Serilog;


namespace Artemis.Plugins.Devices.Nanoleaf;

[PluginFeature(Name = "Nanoleaf Device Provider")]
public class NanoleafDeviceProvider(ILogger logger, IDeviceService deviceService, PluginSettings settings)
    : DeviceProvider
{
    public override void Enable()
    {
        RgbDeviceProvider.Exception += Provider_OnException;
        RgbDeviceProvider.DeviceDefinitions.Clear();

        PluginSetting<List<DeviceDefinition>> definitions =
            settings.GetSetting(nameof(NanoleafConfigurationDialogViewModel.DeviceDefinitions),
                new List<DeviceDefinition>());

        List<(string hostname, string model, string authToken)> devices = definitions.Value.Select(deviceDefinition =>
            (deviceDefinition.Hostname, deviceDefinition.Model, deviceDefinition.AuthToken)).ToList();

        foreach ((string hostname, string model, string authToken) in devices)
            RgbDeviceProvider.DeviceDefinitions.Add(new NanoleafDeviceDefinition(hostname, authToken));
        
        deviceService.AddDeviceProvider(this);
    }

    public override void Disable()
    {
        deviceService.RemoveDeviceProvider(this);

        RgbDeviceProvider.Exception -= Provider_OnException;
        RgbDeviceProvider.Dispose();
    }

    public override NanoleafRGBDeviceProvider RgbDeviceProvider => NanoleafRGBDeviceProvider.Instance;

    private void Provider_OnException(object sender, ExceptionEventArgs args)
    {
        logger.Debug(args.Exception, "Nanoleaf Exception: {message}", args.Exception.Message);
    }
}