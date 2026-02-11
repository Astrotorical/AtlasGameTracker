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

    private void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AddRegisteredAppViewModel viewModel)
        {
            if (viewModel.SelectedProcess != null)
            {
                Close(viewModel.SelectedProcess);
            }
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}