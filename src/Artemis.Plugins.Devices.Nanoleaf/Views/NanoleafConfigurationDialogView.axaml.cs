using Artemis.Plugins.Devices.Nanoleaf.ViewModels;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Devices.Nanoleaf.Views;

public partial class NanoleafConfigurationDialogView : ReactiveUserControl<NanoleafConfigurationDialogViewModel>
{
    public NanoleafConfigurationDialogView()
    {
        InitializeComponent();
    }
}