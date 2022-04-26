using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagement.BackendServer.Data.Entities
{
    [Table("Cars")]
    public class Car
    {
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Key]
        public string LicensePlate { get; set; }

        [Column(TypeName = "varchar(11)")]
        public string CarColor { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CarType { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Company { get; set; }
        [ForeignKey("ParkingLot")]
        public int ParkId { get; set; }
        public ParkingLot ParkingLot { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}
