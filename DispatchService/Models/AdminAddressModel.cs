using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class AdminAddressModel
    {
        public string street { get; set; }
        public ICollection<HouseModel> Houses { get; set; } = new List<HouseModel>();
    }

    public class HouseModel
    {
        public string house { get; set; }
        public List<string> flats { get; set; } = new List<string>();
    }
}