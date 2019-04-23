using System;
using System.Collections.Generic;

namespace APIWithJwt.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool? isActive { get; set; }
        public string Salt { get; set; }
    }
}
