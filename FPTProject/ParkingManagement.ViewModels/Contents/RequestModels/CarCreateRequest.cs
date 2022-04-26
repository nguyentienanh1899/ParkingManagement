using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Contents.RequestModels
{
    public class CarCreateRequest
    {
        public string LicensePlate { get; set; }
        public string CarColor { get; set; }
        public string CarType { get; set; }
        public string Company { get; set; }
        public int ParkId { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
