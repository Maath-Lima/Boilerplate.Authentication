using Boilerplate.Authentication.Data;
using Boilerplate.Authentication.Data.Entities;
using Boilerplate.Authentication.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(DataContext dataContext) : base(dataContext)
        {
        }

        public async Task<Account> GetByEmail(string email)
        {
            return await DbSet.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account> GetByVerificationToken(string token)
        {
            return await DbSet.FirstOrDefaultAsync(a => a.VerificationToken == token);
        }

        public async Task<Account> GetByRefreshToken(string token)
        {
            return await DbSet.SingleOrDefaultAsync(a => a.RefreshTokens.Any(rt => rt.Token == token));
        }

        public async Task<Account> GetByResetToken(string token)
        {
            return await DbSet.SingleOrDefaultAsync(a => a.ResetToken == token && a.ResetTokenExpires > DateTime.UtcNow);
        }
    }
}
