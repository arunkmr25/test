using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using connections.Model;

namespace connections.DTO
{
    public class UserUpdateDTO
    {

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}
