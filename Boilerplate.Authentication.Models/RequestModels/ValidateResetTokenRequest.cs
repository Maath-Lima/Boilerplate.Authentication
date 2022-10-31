using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Authentication.Models.RequestModels
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
