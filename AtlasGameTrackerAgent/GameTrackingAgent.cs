using AtlasGameTrackerAgent.trackers;
using AtlasGameTrackerLibrary.models;

namespace AtlasGameTrackerAgent
{
    public sealed class GameTrackingAgent
    {
        private readonly ProcessTracker _processTracker;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Task? _pollingTask;

        public GameTrackingAgent()
        {
            _processTracker = new ProcessTracker();
        }

        public void StartPolling(int interval, Action<IReadOnlyList<ProcessInfo>> onAppsUpdated)
        {
            if (_pollingTask != null)
            {
                throw new InvalidOperationException("Polling is already running.");
            }

            _pollingTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {

                    try
                    {
                        var apps = await _processTracker.GetRunningApplicationsAsync();
                        onAppsUpdated?.Invoke(apps);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error while tracking applications: {ex.Message}");
                    }

                    try
                    {
                        await Task.Delay(interval, _cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("Polling task was cancelled.");
                        break;
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void StopPolling()
        {
            _cancellationTokenSource.Cancel();
            _pollingTask?.Wait();
        }

        public void Dispose()
        {
            StopPolling();
            Console.WriteLine("Disposed agent.");
        }
    }
}
