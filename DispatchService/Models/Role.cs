using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Role
    {
        [Key]
        public int role_id { get; set; }
        public string role_name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}