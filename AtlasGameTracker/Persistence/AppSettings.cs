using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasGameTrackerUI.Persistence
{
    public sealed class AppSettings
    {
        public string Theme { get; set; } = "Dark";
        public double ZoomScale { get; set; } = 1.0;
    }
}
