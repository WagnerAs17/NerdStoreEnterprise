using System;

namespace NSE.Identidade.API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid Token { get; set; }
        public string UserName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public RefreshToken()
        {
            Id = Guid.NewGuid();
            Token = Guid.NewGuid();
        }
    }
}
