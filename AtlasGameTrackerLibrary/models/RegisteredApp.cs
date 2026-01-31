namespace AtlasGameTrackerLibrary.models
{
    public class RegisteredApp
    {
        public int RegisteredAppId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public bool IsTracked { get; set; } = true;
    }
}
