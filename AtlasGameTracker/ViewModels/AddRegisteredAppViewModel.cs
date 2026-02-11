using AtlasGameTrackerLibrary;
using AtlasGameTrackerLibrary.models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AtlasGameTrackerUI.ViewModels
{
    public partial class AddRegisteredAppViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ObservableCollection<ProcessInfo> _options = new ObservableCollection<ProcessInfo>();
        [ObservableProperty]
        private ProcessInfo? _selectedProcess;

        public AddRegisteredAppViewModel()
        {
            var processes = TrackerUtil.getAppsFromProcesses();
            foreach (ProcessInfo process in processes)
            {
                if (!DBUtil.IsAppRegistered(process.ProcessName))
                {
                    Options.Add(process);
                }
            }
        }
    }
}
