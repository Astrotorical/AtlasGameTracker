using AtlasGameTrackerLibrary;
using AtlasGameTrackerUI.Persistence;
using Avalonia.Controls;
using Avalonia.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AtlasGameTrackerUI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private double _zoom = 1.0;
        private const double ZoomStep = 0.10;

        public double Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            DBUtil.EnsureDatabaseExists();
            InitializeComponent();
            LoadZoomSettings();

            DataContext = this;
            KeyDown += OnKeyDown;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyModifiers == KeyModifiers.Control &&
                (e.Key == Key.Add || e.Key == Key.OemPlus))
            {
                ZoomIn();
                e.Handled = true;
            }
            else if (e.KeyModifiers == KeyModifiers.Control &&
                     (e.Key == Key.Subtract || e.Key == Key.OemMinus))
            {
                ZoomOut();
                e.Handled = true;
            }
        }

        private void ZoomIn()
        {
            SetZoom(_zoom * (1 + ZoomStep));
        }

        private void ZoomOut()
        {
            SetZoom(_zoom / (1 + ZoomStep));
        }
        private void SetZoom(double newScale)
        {
            Zoom = System.Math.Round(System.Math.Clamp(newScale, 0.25, 4.0), 1);
            SaveZoomSettings();
        }

        private void LoadZoomSettings()
        {
            AppSettings? settings = Settings.LoadSettings();
            if (settings != null)
            {
                Zoom = settings.ZoomScale;
            }
        }

        private void SaveZoomSettings()
        {
            AppSettings settings = Settings.LoadSettings() ?? new AppSettings();
            if (settings.ZoomScale != Zoom)
            {
                settings.ZoomScale = Zoom;
                Settings.SaveSettings(settings);
            }
        }
    }
}