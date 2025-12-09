using System;

namespace ChatMessenger
{
    public class User
    {
        public string Username { get; set; }
        public string PhoneNumber { get; set; }

        public User(string username, string phoneNumber)
        {
            Username = username;
            PhoneNumber = phoneNumber;
        }

        public override string ToString()
        {
            return $"{Username} ({PhoneNumber})";
        }
    }
}
