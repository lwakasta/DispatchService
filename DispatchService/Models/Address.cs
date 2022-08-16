using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Address
    {
        [Key]
        public int address_id { get; set; }
        public int street_id { get; set; }
        public Street Street { get; set; }
        public string house { get; set; }
        public string flat { get; set; }        
    }
}