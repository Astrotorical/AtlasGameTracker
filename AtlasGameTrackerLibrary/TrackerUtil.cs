using AtlasGameTrackerLibrary.models;
using System.Diagnostics;

namespace AtlasGameTrackerLibrary
{
    public static class TrackerUtil
    {
        public static IReadOnlyList<ProcessInfo> getAppsFromProcesses()
        {
            var processes = Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero).ToList();
            var apps = new List<ProcessInfo>();
            foreach (var process in processes)
            {
                try
                {
                    var appInfo = new ProcessInfo
                    {
                        ProcessId = process.Id,
                        ProcessName = process.ProcessName
                    };
                    apps.Add(appInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to access process info for PID {process.Id}, {process.ProcessName} - {ex.Message}");
                }
            }

            return apps;
        }
    }
}