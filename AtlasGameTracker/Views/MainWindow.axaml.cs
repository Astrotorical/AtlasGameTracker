using AtlasGameTrackerLibrary;
using Avalonia.Controls;

namespace AtlasGameTrackerUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DBUtil.EnsureDatabaseExists();
            InitializeComponent();
        }
    }
}