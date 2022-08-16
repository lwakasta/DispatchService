using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Type
    {
        [Key]
        public int type_id { get; set; }
        public string type_name { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}