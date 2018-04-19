using System;

namespace connections.DTO
{
    public class UserListDTO
    {
         public int Id { get; set; }
        
        public string Username { get; set; }

        public string Gender { get; set; }

        public int Age { get; set; }

        public string knownAs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string PhotoUrl { get; set; }
    }
}