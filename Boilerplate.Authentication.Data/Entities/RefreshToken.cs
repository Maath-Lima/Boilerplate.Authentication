using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Authentication.Data.Entities
{
    [Owned]
    public class RefreshToken : BaseEntity
    {
        public Account Account { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => Revoked is null && !IsExpired;
    }
}