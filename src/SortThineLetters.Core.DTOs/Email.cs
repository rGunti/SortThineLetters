namespace SortThineLetters.Core.DTOs
{
    public class Email
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
        public string[] Bcc { get; set; }

        public string Subject { get; set; }
        public byte[] Body { get; set; }
    }

    public class Attachment
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
