﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Systems.ViewModels
{
    public class PermissionScreenVm
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public bool HasCreate { get; set; }

        public bool HasUpdate { get; set; }

        public bool HasDelete { get; set; }

        public bool HasView { get; set; }

        public bool HasApprove { get; set; }
    }
}
