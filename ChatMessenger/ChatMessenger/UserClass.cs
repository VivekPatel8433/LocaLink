using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessenger
{
    public class User
    {
        public string Username { get; }
        public string phonenumber { get; }

        public User (string username, string phoneNumber)
        {
            Username = username;
            phonenumber = phoneNumber;
        }


    }
}
