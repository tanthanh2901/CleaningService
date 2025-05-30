﻿namespace UserService.Models
{
    public class CreateUserRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } = "customer";

    }
}
