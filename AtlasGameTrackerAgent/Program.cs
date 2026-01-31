using AtlasGameTrackerAgent;
using AtlasGameTrackerLibrary;
using AtlasGameTrackerLibrary.models;

internal class Program
{
    internal static async Task Main()
    {
        DBUtil.EnsureDatabaseExists();

        var agent = new GameTrackingAgent();

        // Poll every 10 seconds
        agent.StartPolling(10000, apps =>
        {
            Console.WriteLine($"=== {DateTime.Now:HH:mm:ss} ===");

            List<RegisteredApp> trackedRegisteredApps = DBUtil.GetTrackedRegisteredApps();

            // Should only create snapshots for tracked registered apps
            List<RegisteredApp> matchedRegisteredApps = trackedRegisteredApps
                .Where(registeredApp => apps.Any(app => app.ProcessName.Equals(registeredApp.ProcessName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (RegisteredApp app in matchedRegisteredApps)
            {
                try
                {
                    Snapshot newSnapshot = new Snapshot
                    {
                        RegisteredAppId = app.RegisteredAppId,
                        PollTime = DateTime.Now,
                    };
                    Snapshot? latestSnapshot = DBUtil.GetLatestSnapshot(app.RegisteredAppId);

                    // New session detected
                    if (latestSnapshot == null || latestSnapshot.EndTime != null)
                    {
                        newSnapshot.StartTime = DateTime.Now;
                        DBUtil.SaveSnapshot(newSnapshot);
                        Console.WriteLine($"[New session detected for {app.ProcessName}, saved new snapshot]");
                        continue;
                    }

                    // Ongoing or unclosed session
                    if (latestSnapshot.EndTime == null)
                    {
                        // Session timeout, close previous session and start a new one. Otherwise, update the ongoing session.
                        if ((newSnapshot.PollTime - latestSnapshot.PollTime).TotalMinutes > 1) // 1 minute timeout
                        {
                            latestSnapshot.EndTime = latestSnapshot.PollTime;
                            DBUtil.UpdateSnapshot(latestSnapshot);
                            newSnapshot.StartTime = DateTime.Now;
                            DBUtil.SaveSnapshot(newSnapshot);
                            Console.WriteLine($"[Session timeout for {app.ProcessName}, closed previous snapshot and saved a new one]");
                            continue;
                        }
                        else
                        {
                            newSnapshot.SnapshotId = latestSnapshot.SnapshotId;
                            newSnapshot.StartTime = latestSnapshot.StartTime;
                            DBUtil.UpdateSnapshot(newSnapshot);
                            Console.WriteLine($"[Updated ongoing session for {app.ProcessName}]");
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error processing {app.ProcessName}: {ex.Message}]");
                }
            }
        });

        // Keep the console alive until the user presses Ctrl+C.
        await Task.Delay(-1);
    }
}