using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace AtlasGameTrackerUI.ViewModels
{
    public partial class ConfirmationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private string _message;
        [ObservableProperty] 
        private string _windowText = "Confirmation";
        [ObservableProperty]
        private string _confirmText = "OK";
        [ObservableProperty]
        private string _cancelText = "Cancel";

        public ConfirmationViewModel(string message, string? windowText = null, string? confirmText = null, string? cancelText = null)
        {
            Message = message;
            if (!string.IsNullOrEmpty(windowText))
            {
                WindowText = windowText;
            }
            if (!string.IsNullOrEmpty(confirmText))
            {
                ConfirmText = confirmText;
            }
            if (!string.IsNullOrEmpty(cancelText))
            {
                CancelText = cancelText;
            }
        }
    }
}
