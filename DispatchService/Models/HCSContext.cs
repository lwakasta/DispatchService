using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class HCSContext : DbContext
    {
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestStep> RequestSteps { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Step> Steps { get; set; }        
    }
}