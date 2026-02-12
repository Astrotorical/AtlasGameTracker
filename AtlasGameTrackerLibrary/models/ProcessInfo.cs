namespace AtlasGameTrackerLibrary.models
{
    public record ProcessInfo
    {
        public int ProcessId { get; init; }
        public string ProcessName { get; init; } = string.Empty;
        public string DisplayName {  get; set; } = string.Empty;
    }
}
