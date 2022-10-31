using AutoMapper;
using Boilerplate.Authentication.Data.Entities;
using Boilerplate.Authentication.Models.Constants;
using Boilerplate.Authentication.Models.Exceptions;
using Boilerplate.Authentication.Models.RequestModels;
using Boilerplate.Authentication.Models.ResponseModels;
using Boilerplate.Authentication.Models.StartupModels;
using Boilerplate.Authentication.Repositories.Interfaces;
using Boilerplate.Authentication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Boilerplate.Authentication.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public AccountService(IAccountRepository accountRepository,
            ITokenService tokenService,
            IMapper mapper,
            AppSettings appSettings)
        {
            _accountRepository = accountRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _appSettings = appSettings;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = await _accountRepository.GetByEmail(model.Email);

            if (account is null || !account.IsVerified)
            {
                throw new AppException(AccountConstants.EMAIL_EXCPETION);
            }

            if (!BC.Verify(model.Password, account.PasswordHash))
            {
                throw new AppException(AccountConstants.PASSWORD_EXCPETION);
            }

            var jwtToken = _tokenService.GenerateJwtToken(account.Id);
            var refreshToken = _tokenService.GenerateRefreshToken(ipAddress);

            account.RefreshTokens.Add(refreshToken);

            RemoveOldRefreshTokens(account);

            await _accountRepository.Update(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;

            return response;
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = await GetRefreshToken(token);

            var newRefreshToken = _tokenService.GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            account.RefreshTokens.Add(newRefreshToken);

            RemoveOldRefreshTokens(account);

            await _accountRepository.Update(account);

            var jwtToken = _tokenService.GenerateJwtToken(account.Id);
            var response = _mapper.Map<AuthenticateResponse>(account);

            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;

            return response;
        }

        public async Task RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = await GetRefreshToken(token);

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _accountRepository.Update(account);
        }

        public async Task Register(RegisterRequest model, string origin)
        {
            var isAlreadyRegistred = await _accountRepository.GetByEmail(model.Email);

            if (isAlreadyRegistred != null)
            {
                return;
            }

            var account = _mapper.Map<Account>(model);

            account.Role = await IsFirstAccount() ? Role.Admin : Role.User;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = _tokenService.RandomTokenString();

            account.PasswordHash = BC.HashPassword(model.Password);

            await _accountRepository.Create(account);
        }

        public async Task VerifyEmail(string token)
        {
            var account = await _accountRepository.GetByVerificationToken(token);

            if (account is null)
            {
                throw new AppException(AccountConstants.VERIFY_EMAIL_MESSAGE);
            }

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            await _accountRepository.Update(account);
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _accountRepository.GetByEmail(model.Email);

            if (account is null)
            {
                return;
            }

            account.ResetToken = _tokenService.RandomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            await _accountRepository.Update(account);
        }

        public async Task ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = await _accountRepository.GetByResetToken(model.Token);

            if (account is null)
            {
                throw new AppException(AccountConstants.TOKEN_EXCPETION);
            }
        }

        public async Task ResetPassword(ResetPasswordRequest model)
        {
            var account = await _accountRepository.GetByResetToken(model.Token);

            if (account is null)
            {
                throw new AppException(AccountConstants.TOKEN_EXCPETION);
            }

            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            await _accountRepository.Update(account);
        }

        public async Task<IEnumerable<AccountResponse>> GetAll()
        {
            var accounts = await _accountRepository.GetAll();

            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public async Task<AccountResponse> GetById(int id)
        {
            var account = await GetAccount(id);

            return _mapper.Map<AccountResponse>(account);
        }

        public async Task<AccountResponse> Create(CreateRequest model)
        {
            var isAlreadyRegistered = await _accountRepository.GetByEmail(model.Email);

            if (isAlreadyRegistered != null)
            {
                throw new AppException(string.Format(AccountConstants.EMAIL_REGISTERED_EXCPETION, model.Email));
            }

            var account = _mapper.Map<Account>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            account.PasswordHash = BC.HashPassword(model.Password);

            await _accountRepository.Create(account);

            return _mapper.Map<AccountResponse>(account);
        }

        public async Task<AccountResponse> Update(int id, UpdateRequest model)
        {
            var account = await GetAccount(id);

            var isAlreadyRegistered = await _accountRepository.GetByEmail(model.Email);

            if (account.Email != model.Email && isAlreadyRegistered != null)
            {
                throw new AppException(string.Format(AccountConstants.EMAIL_REGISTERED_EXCPETION, model.Email));
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                account.PasswordHash = BC.HashPassword(model.Password);
            }

            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;

            await _accountRepository.Update(account);

            return _mapper.Map<AccountResponse>(account);
        }

        public async Task Delete(int id)
        {
            var account = await GetAccount(id);

            await _accountRepository.Delete(account);
        }

        private async Task<Account> GetAccount(int id)
        {
            var account = await _accountRepository.GetById(id);

            if (account is null)
            {
                throw new AppException(AccountConstants.ACCOUNT_EXCPETION);
            }

            return account;
        }

        private void RemoveOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(rt =>
            !rt.IsActive &&
            rt.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private async Task<(RefreshToken refreshToken, Account account)> GetRefreshToken(string token)
        {
            var account = await _accountRepository.GetByRefreshToken(token);

            if (account is null)
            {
                throw new AppException(AccountConstants.TOKEN_EXCPETION);
            }

            var refreshToken = account.RefreshTokens.Single(rt => rt.Token == token);

            if (!refreshToken.IsActive)
            {
                throw new AppException(AccountConstants.TOKEN_EXCPETION);
            }

            return (refreshToken, account);
        }

        private async Task<bool> IsFirstAccount()
        {
            var accounts = await _accountRepository.GetAll();

            return accounts.Count is default(int);
        }

    }
}
