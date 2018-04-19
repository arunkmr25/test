using System.Threading.Tasks;
using connections.Model;

namespace connections.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User ser, string password);
         Task<User> Login(string userName, string password);
         Task<bool> ExistingUser(string userName);

    }
}