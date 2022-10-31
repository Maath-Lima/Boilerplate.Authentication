using Boilerplate.Authentication.Data.Entities;
using System.Threading.Tasks;

namespace Boilerplate.Authentication.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(int id);
        RefreshToken GenerateRefreshToken(string ipAddress);
        string RandomTokenString();
    }
}
