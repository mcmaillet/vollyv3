using System;

namespace VollyV3.Data
{
    public class LoggedError
    {
        public string Id { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string Path { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
