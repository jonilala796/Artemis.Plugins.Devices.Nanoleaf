using Artemis.Core;
using Artemis.Plugins.Devices.Nanoleaf.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Nanoleaf;

public class NanoleafBootstrapper : PluginBootstrapper
{
    public override void OnPluginLoaded(Plugin plugin)
    {
        plugin.ConfigurationDialog = new PluginConfigurationDialog<NanoleafConfigurationDialogViewModel>();
    }
}