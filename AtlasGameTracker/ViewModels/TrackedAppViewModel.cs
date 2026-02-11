using AtlasGameTrackerLibrary;
using AtlasGameTrackerLibrary.models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AtlasGameTrackerUI.ViewModels
{
    public partial class TrackedAppViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ObservableCollection<RegisteredApp> _registeredApps = new ObservableCollection<RegisteredApp>();
        [ObservableProperty]
        private RegisteredApp? _selectedApp;
        [ObservableProperty]
        private bool _hasSelectedApp;
        [ObservableProperty]
        private bool _isPanelOpen;
        [ObservableProperty]
        private bool _isTrackingEnabled;

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

        // We kinda need to update the bool like this since the toggle button is bound to it,
        // and we want it to reflect the state of the selected app
        public void OnSelectedAppChanged()
        {
            if (SelectedApp != null)
            {
                IsTrackingEnabled = SelectedApp.IsTracked;
                HasSelectedApp = true;
            }
            else
            {
                IsTrackingEnabled = false;
                HasSelectedApp = false;
            }
        }

        private void LoadRegisteredApps()
        {
            List<RegisteredApp> apps = DBUtil.GetAllRegisteredApps()
                .OrderBy(a => string.IsNullOrWhiteSpace(a.DisplayName) ? a.ProcessName : a.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
            RegisteredApps.Clear();
            foreach (RegisteredApp app in apps)
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
