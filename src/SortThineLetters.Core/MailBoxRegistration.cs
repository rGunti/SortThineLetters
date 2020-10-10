using MailKit.Net.Imap;
using SortThineLetters.Core.DTOs;
using System.Threading;

namespace SortThineLetters.Core
{
    static class MailBoxRegistration
    {
        public static void Connect(
            this MailBoxDto mailBox,
            IImapClient client,
            CancellationTokenSource cancelToken)
        {
            client.Connect(mailBox.Server, mailBox.Port, MailKit.Security.SecureSocketOptions.Auto, cancelToken.Token);
        }
    }
}
