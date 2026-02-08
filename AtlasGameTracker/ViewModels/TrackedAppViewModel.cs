using AtlasGameTrackerLibrary;
using AtlasGameTrackerLibrary.models;
using CommunityToolkit.Mvvm.ComponentModel;
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
            }
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
    }
}
