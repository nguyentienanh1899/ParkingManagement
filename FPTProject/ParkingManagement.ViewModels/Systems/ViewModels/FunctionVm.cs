﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Systems.ViewModels
{
    public class FunctionVm
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public int SortOrder { get; set; }

        public string ParentId { get; set; }
    }
}
