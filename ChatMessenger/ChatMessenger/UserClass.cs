namespace ChatMessenger
{
    public class User
    {
        public string Username { get; set; }
        public string PhoneNumber { get; set; }

        public string IP { get; set; } = "";
        public int Port { get; set; } = 0;

        public User(string username, string phoneNumber, string ip = "", int port = 0)
        {
            Username = username;
            PhoneNumber = phoneNumber;
            IP = ip;
            Port = port;
        }

        public override string ToString()
        {
            return $"{Username} ({PhoneNumber})";
        }
    }
}
