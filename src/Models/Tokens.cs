using System;

namespace OfflineMessagingAPI.Models
{
    public class Tokens
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; internal set; }
    }
}
