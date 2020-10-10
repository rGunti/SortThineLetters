using System;
using System.Linq;
using System.Text;

namespace SortThineLetters.Core.DTOs
{
    public class Email
    {
        public EmailAddress[] From { get; set; }
        public EmailAddress[] To { get; set; }
        public EmailAddress[] Cc { get; set; }
        public EmailAddress[] Bcc { get; set; }

        public DateTime SentOn { get; set; }

        public string Subject { get; set; }
        public EmailBody[] BodyParts { get; set; }

        public override string ToString()
        {
            return $"\"{Subject}\" from {From.FirstOrDefault()}";
        }
    }

    public class EmailAddress
    {
        public string Address { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(DisplayName) ?
                $"{Address}" :
                $"{DisplayName} <{Address}>";
        }
    }

    public class EmailBody
    {
        public byte[] Body { get; set; }
        public string BodyAsText => Encoding.Default.GetString(Body);

        public override string ToString()
        {
            return $"Email Body ({Body?.Length} bytes)";
        }
    }

    public class Attachment
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
