using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int? role_id { get; set; }
        public Role Role { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int? department_id { get; set; }
        public Department Department { get; set; }
        public ICollection<RequestStep> RequestSteps { get; set; } = new List<RequestStep>();

    }
}