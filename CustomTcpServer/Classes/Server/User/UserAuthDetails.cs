﻿namespace InfinityServer.Classes.Server.User
{
    public class UserAuthDetails
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
