using Drinctet.Core;
using Prism.Mvvm;

namespace Drinctet.Wpf.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel(DrinctetStatus status)
        {
            Status = status;
        }

        public DrinctetStatus Status { get; }
    }
}