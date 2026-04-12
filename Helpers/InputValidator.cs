using System.Text.RegularExpressions;

namespace HotelBooking_CA2.Helpers
{
    public static class InputValidator
    {
        public static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // strip HTML tags
            input = Regex.Replace(input, "<.*?>", string.Empty);
            return input.Trim();
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsValidLength(string input, int min, int max)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return input.Length >= min && input.Length <= max;
        }
    }
}
