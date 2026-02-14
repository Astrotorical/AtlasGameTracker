using AtlasGameTrackerUI.ViewModels;
using Avalonia.Controls;

namespace AtlasGameTrackerUI;

public partial class ConfirmationView : Window
{
    public ConfirmationView(string message, string? windowText = null, string? confirmText = null, string? cancelText = null)
    {
        InitializeComponent();
        DataContext = new ConfirmationViewModel(message, windowText, confirmText, cancelText);
    }

    private void OnConfirm(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(true);
    }

    private void OnCancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(false);
    }
}