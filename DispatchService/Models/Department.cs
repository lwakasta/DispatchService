using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Department
    {
        [Key]
        public int department_id { get; set; }
        public string department_name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}