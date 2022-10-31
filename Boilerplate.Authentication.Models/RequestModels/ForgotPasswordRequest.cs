using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Authentication.Models.RequestModels
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
