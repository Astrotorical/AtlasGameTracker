using AtlasGameTrackerUI.Persistence;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
                ApplyTheme(false);
            }
        }

        public void ApplyTheme(bool hadExistingTheme)
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
            AppSettings? settings = null;
            // Preventing two back-to-back calls of LoadSettings
            if (hadExistingTheme)
            {
                settings = Settings.LoadSettings();
            }

            if (settings == null) { 
                settings = new AppSettings();
            }

            settings.Theme = SelectedTheme;

            Settings.SaveSettings(settings);
        }
    }
}
