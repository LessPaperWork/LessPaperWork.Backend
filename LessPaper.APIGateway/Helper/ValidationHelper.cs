using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LessPaper.APIGateway.Helper
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates an email address
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>True if the address is valid</returns>
        public static bool IsValidEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
                return false;

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
