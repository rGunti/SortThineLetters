using AutoMapper;
using MailKit;
using MimeKit;
using SortThineLetters.Base.Services;
using SortThineLetters.Core.DTOs;
using System;
using System.Linq;

namespace SortThineLetters.Core.Mapping
{
    public class MailKitMappingProfile : ExtendedMappingProfile
    {
        public MailKitMappingProfile()
        {
            CreateMap<InternetAddress, EmailAddress>()
                .ConvertUsing(MapEmailAddress);
            CreateMap<IMessageSummary, Email>()
                .ConvertUsing(MapMessageSummaryToDto);
        }

        private EmailAddress MapEmailAddress(InternetAddress address, EmailAddress emailAddress, ResolutionContext context)
        {
            if (address is MailboxAddress emailAddr)
            {
                emailAddress = new EmailAddress();
                emailAddress.Address = emailAddr.Address;
                emailAddress.DisplayName = emailAddr.Name;
            }
            else
            {
                throw new NotImplementedException($"Address type {address.GetType()} not implemented");
            }
            return emailAddress;
        }

        private Email MapMessageSummaryToDto(IMessageSummary summary, Email email, ResolutionContext context)
        {
            email = new Email();

            var envelope = summary.Envelope;
            email.From = context.Mapper.Map<EmailAddress[]>(envelope.From?.Select(i => i));
            email.To = context.Mapper.Map<EmailAddress[]>(envelope.To?.Select(i => i));
            email.Cc = context.Mapper.Map<EmailAddress[]>(envelope.Cc?.Select(i => i));
            email.Bcc = context.Mapper.Map<EmailAddress[]>(envelope.Bcc?.Select(i => i));
            email.Subject = envelope.Subject;

            return email;
        }
    }
}
