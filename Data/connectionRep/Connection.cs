using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using connections.Helpers;
using connections.Model;
using Microsoft.EntityFrameworkCore;

namespace connections.Data.connectionRep
{
    public class connection : IConnection
    {
         private readonly DataContext _context;
        public connection(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public Task<Photo> getMainPhoto(int userId)
        {
            var photo = _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            return photo;
        }

        public Task<Photo> GetSinglePhotoDetails(int id)
        {
            var photo = _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var users = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return users;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId).Where(u => u.Gender == userParams.Gender);
            if(userParams.MinAge != 18 || userParams.MaxAge !=99 ){
                users = users.Where(u => u.DateOfBirth.CalculateAge() >= userParams.MinAge 
                && u.DateOfBirth.CalculateAge() <= userParams.MaxAge);
            }
             if(!string.IsNullOrEmpty(userParams.OrderBy)){
               switch(userParams.OrderBy)
               {
                   case "age" :
                   users = users.OrderByDescending(u => u.DateOfBirth.CalculateAge());
                   break;

                   default:
                   users = users.OrderByDescending(u => u.LastActive);
                   break;
               }
            }
            return await PagedList<User>.createPagedList( users, userParams.pageSize , userParams.PageNumber);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}