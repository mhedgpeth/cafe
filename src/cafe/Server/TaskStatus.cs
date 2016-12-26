using System;

namespace cafe.Server
{
    public class TaskStatus
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsSuccessful { get; set; }
    }
}