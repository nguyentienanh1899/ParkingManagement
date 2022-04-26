using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Contents.ViewModels
{
    public class TicketVm
    {
        public int Id { get; set; }
        public DateTime BookingTime { get; set; }
        public string CustomerName { get; set; }
        public string LicensePlate { get; set; }
        public int TripId { get; set; }
    }
}
