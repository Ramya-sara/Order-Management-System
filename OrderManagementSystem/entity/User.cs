using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.entity
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Admin or User

        public User() { }

        public User(int id, string username, string password, string role)
        {
            UserId = id;
            Username = username;
            Password = password;
            Role = role;
        }
    }
}
