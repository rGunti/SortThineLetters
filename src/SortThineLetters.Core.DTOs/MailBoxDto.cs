using SortThineLetters.Base.DTOs;

namespace SortThineLetters.Core.DTOs
{
    public class MailBoxDto : StringKeyedDto
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
