using System.Collections.Generic;
using System.Threading.Tasks;
using connections.Helpers;
using connections.Model;

namespace connections.Data.connectionRep
{
    public interface IConnection
    {
         void Add<T>(T entity) where T:class;
         void Delete<T>(T entity) where T:class;
         Task<bool> SaveAll();
         Task<PagedList<User>>  GetUsers(UserParams userParams);
         Task<User> GetUser(int id);

         Task<Photo> GetSinglePhotoDetails( int id);
         Task<Photo> getMainPhoto( int userId);
         
    }
}