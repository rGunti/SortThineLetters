using SortThineLetters.Base.Storage.Entities;

namespace SortThineLetters.Storage.Entities
{
    public class MailBoxEO : StringKeyEntityObject
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
