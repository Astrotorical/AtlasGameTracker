namespace AtlasGameTrackerLibrary.models
{
    public class Session
    {
        public int SessionId { get; set; }
        public int RegisteredAppId { get; set; }
        public DateTime PollTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        // Properties that aren't stored in the database
        public double SessionDuration
        {
            get
            {
                if (StartTime.HasValue && EndTime.HasValue)
                {
                    double hours = (EndTime.Value - StartTime.Value).TotalHours;
                    if (hours < 0)
                    {
                        return 0.00;
                    }
                    return double.Round(hours, 2);
                }
                if (StartTime.HasValue)
                {
                    double hours = (PollTime - StartTime.Value).TotalHours;
                    if (hours < 0)
                    {
                        return 0.00;
                    }
                    return double.Round(hours, 2);
                }
                return 0.00;
            }
        }
    }
}
