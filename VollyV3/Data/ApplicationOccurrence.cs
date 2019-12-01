namespace VollyV3.Data
{
    public class ApplicationOccurrence
    {
        public int ApplicationId { get; set; }
        public virtual Application Application { get; set; }
        public int OccurrenceId { get; set; }
        public virtual Occurrence Occurrence { get; set; }
    }
}
