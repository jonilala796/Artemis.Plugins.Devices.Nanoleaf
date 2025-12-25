using Artemis.Plugins.Devices.Nanoleaf.ViewModels;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Devices.Nanoleaf.Views;

public partial class NanoleafConfigurationView : ReactiveUserControl<NanoleafConfigurationViewModel>
{
    public NanoleafConfigurationView()
    {
        InitializeComponent();
    }
}