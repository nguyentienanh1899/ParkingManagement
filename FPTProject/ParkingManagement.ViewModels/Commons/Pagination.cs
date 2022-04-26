using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Commons
{
    public  class Pagination<T> : PaginationBase where T : class
    {
        public List<T> Items { get; set; }

    }
}
