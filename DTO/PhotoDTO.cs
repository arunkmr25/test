using System;

namespace connections.DTO
{
    public class PhotoDTO
    {  
        public int Id { get; set; } 

        public string Url { get; set; }  

        public string Description { get; set; } 

        public DateTime DateAdded { get; set; } 

        public bool IsMain { get; set; }
        
    }
}