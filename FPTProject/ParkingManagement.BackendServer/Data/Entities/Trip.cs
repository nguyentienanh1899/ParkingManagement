using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagement.BackendServer.Data.Entities
{
    [Table("Trips")]
    public class Trip
    {
        [MaxLength(20)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    
        public int CategoryId { get; set; }

        public int BookedTicketNumber { get; set; }
        [Column(TypeName = "varchar(11)")]
        public string CarType { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime DepartureTime { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Destination { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Driver { get; set; }
        public int? MaximumOnlineTicketNumber { get; set;}
        public int? NumberOfTicketsAvailable { get; set; }
        public int? NumberOfAvailableTicketOffices { get; set; } 
        public List<Ticket> Tickets { get; set; }
        public List<BookingOffice> BookingOffices { get; set; }
    }
}
