namespace bacit_dotnet.MVC.Entities
{
    public class TimestampEntity
    {
        public int timestamp_id { get; set; }
        public int suggestion_id{ get; set; }
        public DateTime createdTimestamp { get; set; }
        public DateTime planTimestamp { get; set; }
        public DateTime doTimestamp { get; set; }
        public DateTime studyTimestamp { get; set; }
        public DateTime actTimestamp { get; set; }
        public DateTime finishedTimestamp { get; set; }
        public DateTime lastUpdatedTimestamp { get; set; }

        public DateTime dueByTimestamp { get; set; }
    }
}
