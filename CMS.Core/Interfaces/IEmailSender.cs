﻿using System.Threading.Tasks;

namespace CMS.Core.Interfaces
{

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
