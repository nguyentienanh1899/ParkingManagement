using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Contents.ViewModels
{
    public class TripVm
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BookedTicketNumber { get; set; }
        public string CarType { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Destination { get; set; }
        public string Driver { get; set; }
        public int? MaximumOnlineTicketNumber { get; set; }
        public int? NumberOfTicketsAvailable { get; set; }
        public int? NumberOfAvailableTicketOffices { get; set; }
    }
}
