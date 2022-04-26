using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagement.BackendServer.Data.Entities
{
    [Table("Tickets")]
    public class Ticket
    {
        [MaxLength(20)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime BookingTime { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string CustomerName { get; set; }
        [ForeignKey("Car")]
        public string LicensePlate { get; set; }
        [ForeignKey("Trip")]
        public int TripId { get; set; }
        public Car Car { get; set; }
        public Trip Trip { get; set; }
    }
}
