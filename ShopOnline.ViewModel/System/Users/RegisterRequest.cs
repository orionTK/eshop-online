﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShopOnline.ViewModel.Users.System
{
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        //123456TK@vt
        public string ConfirmPassword { get; set; }

    }
}
