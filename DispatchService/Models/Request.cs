using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Request
    {
        [Key]
        public int request_id { get; set; }
        public int address_id { get; set; }
        public Address Address { get; set; }
        public int category_id { get; set; }
        public Category Category { get; set; }
        public int type_id { get; set; }
        public Type Type { get; set; }
        public string description { get; set; }
        public DateTime date_begin { get; set; }
        public DateTime? date_end { get; set; }
        public DateTime due_date { get; set; }
        public ICollection<RequestStep> RequestSteps { get; set; } = new List<RequestStep>();
    }
}