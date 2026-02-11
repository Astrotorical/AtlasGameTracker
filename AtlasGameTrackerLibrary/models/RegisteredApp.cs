namespace AtlasGameTrackerLibrary.models
{
    public class RegisteredApp
    {
        public int RegisteredAppId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public bool IsTracked { get; set; } = true;
        public List<Session> Sessions { get; set; } = new List<Session>();
        public double TotalPlaytime
        {
            get
            {
                double total = 0.00;
                foreach (Session session in Sessions) 
                {
                    total += session.SessionDuration;
                }
                return double.Round(total, 2);
            }
        }
    }
}
