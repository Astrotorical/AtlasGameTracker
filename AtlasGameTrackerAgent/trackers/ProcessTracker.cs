using AtlasGameTrackerLibrary;
using AtlasGameTrackerLibrary.models;

namespace AtlasGameTrackerAgent.trackers
{
    public class ProcessTracker
    {

        public async Task<IReadOnlyList<ProcessInfo>> GetRunningApplicationsAsync()
        {
            return await Task.Run(() =>
            {
                return TrackerUtil.getAppsFromProcesses();
            });
        }
    }
}