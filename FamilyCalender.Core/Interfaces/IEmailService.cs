using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces
{
    public interface IEmailService
    {
        void SendVerificationEmail(string userEmail, string verificationToken);
        void SendPasswordResetEmail(string userEmail, string resetToken);

    }
}
