namespace AtlasGameTrackerLibrary.models
{
    public record ProcessInfo
    {
        public int ProcessId { get; init; }
        public string ProcessName { get; init; } = string.Empty;
    }
}
