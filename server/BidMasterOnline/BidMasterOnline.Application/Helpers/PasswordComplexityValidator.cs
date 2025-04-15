namespace BidMasterOnline.Application.Helpers
{
    public static class PasswordComplexityValidator
    {
        public const int DigitsCount = 8;
        public const int UpperCaseCount = 1;
        public const int LowerCaseCount = 1;

        public static void Validate(string password)
        {
            if (password.Count(char.IsDigit) < DigitsCount)
                throw new ArgumentException($"Password must contain at least {DigitsCount} digits.");

            if (password.Count(char.IsUpper) < UpperCaseCount)
                throw new ArgumentException($"Password must contain at least {UpperCaseCount} upper case symbols.");

            if (password.Count(char.IsLower) < LowerCaseCount)
                throw new ArgumentException($"Password must contain at least {LowerCaseCount} lower case symbols.");
        }
    }
}
