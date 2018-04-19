using System;
using System.Threading.Tasks;
using connections.Model;
using Microsoft.EntityFrameworkCore;

namespace connections.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<bool> ExistingUser(string userName)
        {
            if( await _context.Users.AnyAsync(x => x.Username == userName ))
            return true;
            return false;
        }

        public async Task<User> Login(string userName, string password)
        {
            var userContext= await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x=>x.Username == userName);

            if(userContext==null)
            return null;

            if(!VerifyPassword(password, userContext.PasswordHash,userContext.PasswordSalt))
            return null;

            //Auth Success
            return userContext;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac= new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var Computedhash= hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0; i< Computedhash.Length;i++){
                    if(Computedhash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreateHashPassword(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user; 
        }

        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hamac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hamac.Key;
                passwordHash = hamac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}