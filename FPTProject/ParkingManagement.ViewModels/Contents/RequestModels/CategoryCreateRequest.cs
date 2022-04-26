using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.Contents.RequestModels
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }

        public string SeoAlias { get; set; }

        public string SeoDescription { get; set; }

        public int SortOrder { get; set; }

        public int? ParentId { get; set; }
    }
}
