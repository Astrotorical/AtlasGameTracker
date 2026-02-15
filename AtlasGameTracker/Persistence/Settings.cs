using Avalonia;
using Avalonia.Controls;
using System;
using System.IO;
using System.Text.Json;

namespace AtlasGameTrackerUI.Persistence
{
    public static class Settings
    {
        private static string GetSettingsPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = System.IO.Path.Combine(appData, "AtlasGameTracker");
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            return System.IO.Path.Combine(folderPath, "settings.json");
        }

        public static AppSettings? LoadSettings()
        {
            try
            {
                var path = GetSettingsPath();
                if (!File.Exists(path))
                    return null;

                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AppSettings>(json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return null;
            }
        }

        public static void SaveSettings(AppSettings settings)
        {
            try
            {
                var path = GetSettingsPath();
                var dir = Path.GetDirectoryName(path)!;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(settings,
                          new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                {
                    var dialog = new Window
                    {
                        Title = "Error: Unable to save settings",
                        Content = new TextBlock { Text = ex.Message, TextWrapping = Avalonia.Media.TextWrapping.Wrap, Margin = new Thickness(20) },
                        Width = 300,
                        Height = 150,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    dialog.Show();
                }
            }
        }
    }
}
