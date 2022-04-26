using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Contents.RequestModels
{
    public class ParkingLotCreateRequest
    {
        public int ParkArea { get; set; }
        public string ParkName { get; set; }
        public string ParkPlace { get; set; }
        public int ParkPrice { get; set; }
        public string ParkStatus { get; set; }
        public int? TotalParkingSpace { get; set; }
    }
}
