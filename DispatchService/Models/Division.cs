using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Division
    {
        [Key]
        public int division_id { get; set; }
        public string division_name { get; set; }
        public ICollection<Street> Streets { get; set; } = new List<Street>();
    }
}