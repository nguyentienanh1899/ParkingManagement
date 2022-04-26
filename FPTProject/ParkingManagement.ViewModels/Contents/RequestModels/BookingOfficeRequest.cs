using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Contents.RequestModels
{
    public class BookingOfficeRequest
    {
        public DateTime EndContractDeadline { get; set; }
        public string OfficeName { get; set; }
        public string OfficePhone { get; set; }
        public string OfficePlace { get; set; }
        public int OfficePrice { get; set; }
        public DateTime StartContractDeadLine { get; set; }
        public int TripId { get; set; }
    }
}
