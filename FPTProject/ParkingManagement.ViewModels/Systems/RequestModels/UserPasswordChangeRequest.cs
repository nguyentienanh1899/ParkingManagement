﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Systems.RequestModels
{
    public class UserPasswordChangeRequest
    {
        public string UserId { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
