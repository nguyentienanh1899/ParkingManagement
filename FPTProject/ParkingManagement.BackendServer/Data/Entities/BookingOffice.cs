using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagement.BackendServer.Data.Entities
{
    [Table("BookingOffices")]
    public class BookingOffice
    {
        [MaxLength(20)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime EndContractDeadline { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OfficeName { get; set; }
        [Column(TypeName = "varchar(11)")]
        public string OfficePhone { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string OfficePlace { get; set; }
        public int OfficePrice { get; set; }
        public  DateTime StartContractDeadLine { get; set; }
        [ForeignKey("Trip")]
        public int TripId { get; set; }
        public Trip Trip { get; set; }
    }
}
