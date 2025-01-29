namespace EmailApi
{
    public class EmailMessage
    {
        public Guid Id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsSent { get; set; }
    }

}
