using AtlasGameTrackerUI.Persistence;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AtlasGameTrackerUI.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ObservableCollection<string> ThemeOptions { get; } = new(new[] {"Light", "Dark" });

        [ObservableProperty]
        private string _selectedTheme;

        public SettingsViewModel()
        {
            AppSettings? saved = Settings.LoadSettings();
            if (saved != null)
            {
                SelectedTheme = ThemeOptions.Select(x => x.ToLower()).Contains(saved.Theme.ToLower()) ? saved.Theme : "Dark";
            }
            else
            {
                SelectedTheme = "Dark";
                ApplyTheme();
            }
        }

        public void ApplyTheme()
        {
            if (Application.Current == null)
                return;

            switch (SelectedTheme)
            {
                case "Light":
                    Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                    break;
                case "Dark":
                    Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                    break;
                default:
                    Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                    break;
            }

            AppSettings settings = new AppSettings
            {
                Theme = SelectedTheme
            };
            Settings.SaveSettings(settings);
        }
    }
}
