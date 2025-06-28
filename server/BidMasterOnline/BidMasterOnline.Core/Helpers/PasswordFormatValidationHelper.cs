using System.Text.RegularExpressions;

namespace BidMasterOnline.Core.Helpers
{
    public static class PasswordFormatValidationHelper
    {
        public static bool ValidatePasswordFormat(string password, out string errors)
        {
            errors = string.Empty;

            bool result = true;

            if (string.IsNullOrEmpty(password))
            {
                errors += "Password should not be empty.\n";
                result = false;

                return result;
            }

            if (password.Length < 8 || password.Length > 64)
            {
                errors += "Password should contain 8-64 symbols.\n";
                result = false;
            }

            if (!password.Any(char.IsLetter))
            {
                errors += "Password should contain at least one letter.\n";
                result = false;
            }

            if (!password.Any(char.IsDigit))
            {
                errors += "Password should contain at least one digit.\n";
                result = false;
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
            {
                errors += "Password should contain at least one special symbol.\n";
                result = false;
            }

            return result;
        }
    }
}
