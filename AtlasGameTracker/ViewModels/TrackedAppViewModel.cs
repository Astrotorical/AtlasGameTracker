using AtlasGameTrackerLibrary;
using AtlasGameTrackerLibrary.models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AtlasGameTrackerUI.ViewModels
{
    public partial class TrackedAppViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ObservableCollection<RegisteredApp> _registeredApps = new ObservableCollection<RegisteredApp>();

        private RegisteredApp? _selectedApp;
        [ObservableProperty]
        private bool _isPanelOpen;
        [ObservableProperty]
        private bool _isTrackingEnabled;
        public RegisteredApp? SelectedApp
        {
            get => _selectedApp;
            set => SetProperty(ref _selectedApp, value);
        }

        public TrackedAppViewModel()
        {
            LoadRegisteredApps();
            if (RegisteredApps.Count > 0)
            {
                SelectedApp = RegisteredApps.First();
                if (SelectedApp != null)
                {
                    IsTrackingEnabled = SelectedApp.IsTracked;
                }
            }
            IsPanelOpen = true;
        }

        private void LoadRegisteredApps()
        {
            var apps = DBUtil.GetAllRegisteredApps();
            RegisteredApps.Clear();
            foreach (var app in apps)
            {
                RegisteredApps.Add(app);
            }
        }

        [RelayCommand]
        private void ToggleTracking()
        {
            if (SelectedApp != null)
            {
                if (!SelectedApp.IsTracked)
                {
                    DBUtil.ReregisterApp(SelectedApp.RegisteredAppId);
                }
                else
                {
                    DBUtil.UnregisterApp(SelectedApp.RegisteredAppId);
                }
                SelectedApp.IsTracked = !SelectedApp.IsTracked;
            }
        }

        [RelayCommand]
        private void TogglePanel()
        {
            IsPanelOpen = !IsPanelOpen;
        }
    }
}
