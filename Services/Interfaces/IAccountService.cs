using Boilerplate.Authentication.Models.RequestModels;
using Boilerplate.Authentication.Models.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        Task RevokeToken(string token, string ipAddress);
        Task Register(RegisterRequest model, string origin);
        Task VerifyEmail(string token);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task ValidateResetToken(ValidateResetTokenRequest model);
        Task ResetPassword(ResetPasswordRequest model);
        Task<IEnumerable<AccountResponse>> GetAll();
        Task<AccountResponse> GetById(int id);
        Task<AccountResponse> Create(CreateRequest model);
        Task<AccountResponse> Update(int id, UpdateRequest model);
        Task Delete(int id);
    }
}
