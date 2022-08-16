using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Street
    {
        [Key]
        public int street_id { get; set; }
        public string street_name { get; set; }
        public int division_id { get; set; }
        public Division Division { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}