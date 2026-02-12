using AtlasGameTrackerUI.ViewModels;
using Avalonia.Controls;

namespace AtlasGameTrackerUI;

public partial class TrackedAppView : UserControl
{
    public TrackedAppView()
    {
        InitializeComponent();
    }

    private void OnSelectedAppChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is TrackedAppViewModel viewModel)
        {
            viewModel.OnSelectedAppChanged();
        }
    }

    //private void DisplayNameLostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    //{
    //    if (DataContext is TrackedAppViewModel viewModel)
    //    {
    //        viewModel.OnDisplayNameLostFocus();
    //    }
    //}
}