using Microsoft.AspNetCore.DataProtection;

namespace HotelBooking_CA2.Helpers
{
    public class SessionHelper
    {
        private const string Purpose = "SessionProtection";
        private readonly IDataProtector _protector;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionHelper(IDataProtectionProvider provider, IHttpContextAccessor httpContextAccessor)
        {
            _protector = provider.CreateProtector(Purpose);
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public void SetString(string key, string value)
        {
            var encrypted = _protector.Protect(value);
            Session.SetString(key, encrypted);
        }

        public string? GetString(string key)
        {
            var encrypted = Session.GetString(key);
            if (string.IsNullOrEmpty(encrypted))
                return null;

            try
            {
                return _protector.Unprotect(encrypted);
            }
            catch
            {
                return null;
            }
        }

        public void SetInt32(string key, int value)
        {
            var encrypted = _protector.Protect(value.ToString());
            Session.SetString(key, encrypted);
        }

        public int? GetInt32(string key)
        {
            var decrypted = GetString(key);
            if (decrypted != null && int.TryParse(decrypted, out var result))
                return result;
            return null;
        }

        public void Clear()
        {
            Session.Clear();
        }
    }
}
