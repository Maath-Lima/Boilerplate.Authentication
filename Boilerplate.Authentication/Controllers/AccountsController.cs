using Boilerplate.Authentication.Data.Entities;
using Boilerplate.Authentication.Helpers;
using Boilerplate.Authentication.Models.Constants;
using Boilerplate.Authentication.Models.RequestModels;
using Boilerplate.Authentication.Models.ResponseModels;
using Boilerplate.Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> AuthenticateAsync(AuthenticateRequest authenticateRequest)
        {
            var response = await _accountService.Authenticate(authenticateRequest, AccountsControllerHelper.GetIpAddress(Request));

            AccountsControllerHelper.SetTokenCookie(response.RefreshToken, Response);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies[AccountConstants.REFRESH_TOKEN];
            var response = await _accountService.RefreshToken(refreshToken, AccountsControllerHelper.GetIpAddress(Request));

            AccountsControllerHelper.SetTokenCookie(response.RefreshToken, Response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
        {
            var token = model.Token ?? Request.Cookies[AccountConstants.REFRESH_TOKEN];

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(MessageResponse(AccountConstants.TOKEN_REQUIRED_MESSAGE));
            }

            if (!Account.OwnsToken(token) && Account.Role != Role.Admin)
            {
                return Unauthorized(MessageResponse(AccountConstants.UNAUTHORIZED_MESSAGE));
            }

            await _accountService.RevokeToken(token, AccountsControllerHelper.GetIpAddress(Request));

            return Ok(MessageResponse(AccountConstants.TOKEN_REVOKED_MESSAGE));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            await _accountService.Register(registerRequest, AccountConstants.ORIGIN);

            return Ok(MessageResponse(AccountConstants.REGISTER_MESSAGE));
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequest verifyEmailRequest)
        {
            await _accountService.VerifyEmail(verifyEmailRequest.Token);

            return Ok(MessageResponse(AccountConstants.VERIFY_EMAIL_MESSAGE));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPassword(model, Request.Headers[AccountConstants.ORIGIN]);

            return Ok(MessageResponse(AccountConstants.FORGOT_PASSWORD_MESSAGE));
        }

        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
        {
            await _accountService.ValidateResetToken(model);

            return Ok(MessageResponse(AccountConstants.VALIDATE_TOKEN_MESSAGE));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            await _accountService.ResetPassword(model);

            return Ok(MessageResponse(AccountConstants.RESET_PASSWORD_MESSAGE));
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAll()
        {
            var accounts = await _accountService.GetAll();

            return Ok(accounts);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccountResponse>> GetById(int id)
        {
            if (id != Account.Id && Account.Role != Role.Admin)
            {
                return Unauthorized();
            }

            var account = await _accountService.GetById(id);

            return Ok(account);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<AccountResponse>> Create(CreateRequest model)
        {
            var account = _accountService.Create(model);

            return Ok(account);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AccountResponse>> Update(int id, UpdateRequest model)
        {
            if (id != Account.Id && Account.Role != Role.Admin)
            {
                return Unauthorized();
            }

            if (Account.Role != Role.Admin)
            {
                model.Role = null;
            }

            var account = await _accountService.Update(id, model);

            return Ok(account);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != Account.Id && Account.Role != Role.Admin)
            {
                return Unauthorized();
            }

            await _accountService.Delete(id);

            return Ok(MessageResponse(AccountConstants.DELETE_MESSAGE));
        }
    }
}
