using System.Collections.Generic;
using connections.Data;
using connections.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace connections.Data
{
    public class UserFeedDummyData
    {
        private readonly DataContext _context;
        public UserFeedDummyData(DataContext context)
        {
            _context = context;

        }

        public void SeedUsers(){
            var users = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var userlist= JsonConvert.DeserializeObject<List<User>>(users);
            foreach(var user in userlist)
            {
            byte [] passwordHash,passwordSalt;
            this.CreateHashPassword("password", out passwordHash, out passwordSalt);
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;
            user.Username= user.Username.ToLower();

            _context.Users.Add(user);
            }
            _context.SaveChanges();
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