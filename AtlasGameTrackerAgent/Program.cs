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

            // Should only create Sessions for tracked registered apps
            List<RegisteredApp> matchedRegisteredApps = trackedRegisteredApps
                .Where(registeredApp => apps.Any(app => app.ProcessName.Equals(registeredApp.ProcessName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (RegisteredApp app in matchedRegisteredApps)
            {
                try
                {
                    Session newSession = new Session
                    {
                        RegisteredAppId = app.RegisteredAppId,
                        PollTime = DateTime.Now,
                    };
                    Session? latestSession = DBUtil.GetLatestSession(app.RegisteredAppId);

                    // New session detected
                    if (latestSession == null || latestSession.EndTime != null)
                    {
                        newSession.StartTime = DateTime.Now;
                        DBUtil.SaveSession(newSession);
                        Console.WriteLine($"[New session detected for {app.ProcessName}, saved new Session]");
                        continue;
                    }

                    // Ongoing or unclosed session
                    if (latestSession.EndTime == null)
                    {
                        // Session timeout, close previous session and start a new one. Otherwise, update the ongoing session.
                        if ((newSession.PollTime - latestSession.PollTime).TotalMinutes > 1) // 1 minute timeout
                        {
                            latestSession.EndTime = latestSession.PollTime;
                            DBUtil.UpdateSession(latestSession);
                            newSession.StartTime = DateTime.Now;
                            DBUtil.SaveSession(newSession);
                            Console.WriteLine($"[Session timeout for {app.ProcessName}, closed previous Session and saved a new one]");
                            continue;
                        }
                        else
                        {
                            newSession.SessionId = latestSession.SessionId;
                            newSession.StartTime = latestSession.StartTime;
                            DBUtil.UpdateSession(newSession);
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