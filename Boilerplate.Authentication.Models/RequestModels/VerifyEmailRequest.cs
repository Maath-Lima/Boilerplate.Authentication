using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Authentication.Models.RequestModels
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
