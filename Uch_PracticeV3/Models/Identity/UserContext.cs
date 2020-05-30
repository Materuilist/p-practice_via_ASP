using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Uch_PracticeV3.Models.Identity
{
    public class UserContext : DbContext
    {
        public UserContext() : base("IdentityDb")
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}