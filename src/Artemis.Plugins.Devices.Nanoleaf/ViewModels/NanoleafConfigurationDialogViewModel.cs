using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Nanoleaf.Helper;
using Artemis.Plugins.Devices.Nanoleaf.RGB.NET.Helper;
using Artemis.Plugins.Devices.Nanoleaf.Settings;
using Artemis.Plugins.Devices.Nanoleaf.ViewModels.Dialogs;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using DynamicData;
using ReactiveUI;

namespace Artemis.Plugins.Devices.Nanoleaf.ViewModels;

public class NanoleafConfigurationDialogViewModel : PluginConfigurationViewModel
{
    private readonly PluginSetting<List<DeviceDefinition>> _definitions;
    private readonly IPluginManagementService _pluginManagementService;
    private readonly PluginSettings _settings;
    private readonly IWindowService _windowService;

    public ObservableCollection<DeviceDefinition> DeviceDefinitions { get; }


    public ReactiveCommand<Unit, Unit> AddDevice { get; }
    public ReactiveCommand<DeviceDefinition, Unit> EditDevice { get; }
    public ReactiveCommand<DeviceDefinition, Unit> RemoveDevice { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public ReactiveCommand<Unit, Unit> DiscoverDevices { get; }


    public NanoleafConfigurationDialogViewModel(Plugin plugin, PluginSettings settings, IWindowService windowService,
        IPluginManagementService pluginManagementService) : base(plugin)
    {
        _settings = settings;
        _windowService = windowService;
        _pluginManagementService = pluginManagementService;

        _definitions = settings.GetSetting(nameof(DeviceDefinitions), new List<DeviceDefinition>());

        DeviceDefinitions = new ObservableCollection<DeviceDefinition>(_definitions.Value);

        AddDevice = ReactiveCommand.CreateFromTask(ExecuteAddDevice);
        EditDevice = ReactiveCommand.CreateFromTask<DeviceDefinition>(ExecuteEditDevice);
        RemoveDevice = ReactiveCommand.Create<DeviceDefinition>(ExecuteRemoveDevice);
        Save = ReactiveCommand.Create(ExecuteSave);
        Cancel = ReactiveCommand.CreateFromTask(ExecuteCancel);
        DiscoverDevices = ReactiveCommand.CreateFromTask(ExecuteDiscoverDevices);
    }

    private async Task ExecuteDiscoverDevices()
    {
        List<(string address, string model)> discoverDevices = NanoleafDiscoveryHelper.DiscoverDevices();

        if (await _windowService
                .ShowDialogAsync<DeviceDiscoverDialogViewModel, DiscoverDialogResult>(discoverDevices) !=
            DiscoverDialogResult.Ok)
            return;


        //check if devices with one of the ip adresses already exist
        foreach ((var ipAddress, string? model) in discoverDevices)
        {
            if (_definitions.Value.Any(d => ipAddress.Equals(d.Hostname)))
                continue;


            if (ipAddress == null)
                continue;

            _definitions.Value.Add(new DeviceDefinition
            {
                Hostname = ipAddress.ToString(),
                Model = model
            });

            DeviceDefinitions.Add(new DeviceDefinition
            {
                Hostname = ipAddress.ToString(),
                Model = model
            });
        }
    }


    private async Task ExecuteAddDevice()
    {
        DeviceDefinition device = new();
        if (await _windowService.ShowDialogAsync<DeviceConfigurationDialogViewModel, DeviceDialogResult>(device) !=
            DeviceDialogResult.Save)
            return;

        _definitions.Value.Add(device);
        DeviceDefinitions.Add(device);
    }

    private async Task ExecuteEditDevice(DeviceDefinition device)
    {
        if (await _windowService.ShowDialogAsync<DeviceConfigurationDialogViewModel, DeviceDialogResult>(device) ==
            DeviceDialogResult.Remove)
            DeviceDefinitions.Remove(device);
    }

    private async Task ExecuteCancel()
    {
        if (_definitions.HasChanged)
        {
            if (!await _windowService.ShowConfirmContentDialog("Discard changes",
                    "Do you want to discard any changes you made?"))
                return;
        }

        _definitions.RejectChanges();

        Close();
    }

    private void ExecuteRemoveDevice(DeviceDefinition device)
    {
        _definitions.Value.Remove(device);
        DeviceDefinitions.Remove(device);
    }

    private void ExecuteSave()
    {
        _definitions.Save();

        Task.Run(() =>
        {
            NanoleafDeviceProvider deviceProvider = Plugin.GetFeature<NanoleafDeviceProvider>();
            if ((deviceProvider == null || !deviceProvider.IsEnabled)) return;
            _pluginManagementService.DisablePluginFeature(deviceProvider, false);
            _pluginManagementService.EnablePluginFeature(deviceProvider, false);
        });
    }
}