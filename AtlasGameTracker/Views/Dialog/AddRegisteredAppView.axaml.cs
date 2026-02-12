using AtlasGameTrackerUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtlasGameTrackerUI;

public partial class AddRegisteredAppView : Window
{
    public AddRegisteredAppView()
    {
        InitializeComponent();
        DataContext = new AddRegisteredAppViewModel();
    }

    private void AddButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AddRegisteredAppViewModel viewModel)
        {
            if (viewModel.SelectedProcess != null)
            {
                viewModel.SelectedProcess.DisplayName = viewModel.DisplayName;
                Close(viewModel.SelectedProcess);
            }
        }
    }

    private void CancelButtonClick(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is AddRegisteredAppViewModel viewModel)
        {
            if (viewModel.SelectedProcess != null)
            {
                viewModel.DisplayName = viewModel.SelectedProcess.ProcessName;
            }
        }
    }
}