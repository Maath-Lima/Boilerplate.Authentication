using Boilerplate.Authentication.Data.Entities;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Repositories.Interfaces
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<Account> GetByEmail(string email);
        Task<Account> GetByVerificationToken(string token);
        Task<Account> GetByRefreshToken(string token);
        Task<Account> GetByResetToken(string token);
    }
}
