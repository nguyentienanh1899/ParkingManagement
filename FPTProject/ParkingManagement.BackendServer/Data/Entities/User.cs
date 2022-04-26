using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagement.BackendServer.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50)]
        [Column(TypeName = "varchar(25)")]
        [Required]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar(25)")]
        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime Dob { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Address { get; set; }
        [MaxLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string Department { get; set; }
        [MaxLength(1)]
        [Column(TypeName = "varchar(1)")]
        public string Sex { get; set; }

    }
}