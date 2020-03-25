namespace VollyV3.Data
{
    public class OccurrenceApplication
    {
        public int OccurrenceId { get; set; }
        public virtual Occurrence Occurrence { get; set; }
        public int ApplicationId { get; set; }
        public virtual Application Application { get; set; }
    }
}
