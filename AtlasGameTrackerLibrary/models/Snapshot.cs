namespace AtlasGameTrackerLibrary.models
{
    public class Snapshot
    {
        public int SnapshotId { get; set; }
        public int RegisteredAppId { get; set; }
        public DateTime PollTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
