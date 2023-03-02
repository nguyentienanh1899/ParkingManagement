using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagement.BackendServer.Data.Entities
{
    [Table("ParkingLots")]
    public class ParkingLot
    {
        [MaxLength(20)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(20)]
        public int ParkArea { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ParkName { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ParkPlace { get; set; }
        public int ParkPrice { get; set; }
        public int? NumberOfCarsInTheParkingLot { get; set; }
        public int? TotalParkingSpace { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string ParkStatus { get; set; }
        public List<Car> Cars { get; set; }
        public List<Attachment> Attachments { get; set; }

    }
}
