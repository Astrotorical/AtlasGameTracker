using AtlasGameTrackerUI.ViewModels;
using Avalonia.Controls;

namespace AtlasGameTrackerUI;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = new ViewModels.SettingsViewModel();
    }

    private void SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.ApplyTheme();
        }
    }
}