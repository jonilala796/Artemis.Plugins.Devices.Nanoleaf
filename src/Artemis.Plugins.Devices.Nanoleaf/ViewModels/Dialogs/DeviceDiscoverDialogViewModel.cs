using System.Collections.Generic;
using System.Net;
using System.Reactive;
using Artemis.UI.Shared;
using ReactiveUI;

namespace Artemis.Plugins.Devices.Nanoleaf.ViewModels.Dialogs;

public class DeviceDiscoverDialogViewModel : DialogViewModelBase<DiscoverDialogResult>
{
    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public ReactiveCommand<Unit, Unit> Ok { get; }

    public int DeviceCount { get; }
    
    public string DeviceCountText => DeviceCount == 1 ? "1 device found" : $"{DeviceCount} devices found";

    public DeviceDiscoverDialogViewModel(List<(string address, string model)>? devices)
    {
        DeviceCount = devices?.Count ?? 0;
        
        Cancel = ReactiveCommand.Create(ExecuteCancel);
        Ok = ReactiveCommand.Create(ExecuteOk);
    }

    private void ExecuteOk()
    {
        Close(DiscoverDialogResult.Ok);
    }

    private void ExecuteCancel()
    {
        Close(DiscoverDialogResult.Cancel);
    }
}

public enum DiscoverDialogResult
{
    Cancel,
    Ok
}