using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace connections.Helpers
{
    public class PaginationHeaders
    {
        public PaginationHeaders(int currentPage, int itemsPerPage, int totalPage, int totalItems)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalPage = totalPage;
            this.TotalItems = totalItems;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }


    }
}