using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace connections.Helpers
{
    public class UserParams
    {
        private const int maxSize = 100;
        public int PageNumber { get; set; } = 1;
        private int PageSize = 10;
        public int pageSize
        {
            get { return PageSize;}
            set { PageSize = (value > maxSize) ? maxSize : value;}
        }
        public string Gender { get; set; }  
        public int UserId { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public string OrderBy { get; set; }
    }

}