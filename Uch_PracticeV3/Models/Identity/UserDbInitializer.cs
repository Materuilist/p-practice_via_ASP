using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Uch_PracticeV3.Models.Identity
{
    public class UserDbInitializer : DropCreateDatabaseAlways<UserContext>
    {
        protected override void Seed(UserContext db)
        {
            db.Roles.Add(new Role { Id = 1, Name = "admin" });
            db.Roles.Add(new Role { Id = 2, Name = "manager" });
            db.Users.Add(new User
            {
                Email = "admin@mail.ru",
                Password = "qwerty",
                RoleId = 1
            });
            db.Users.Add(new User
            {
                Email = "managerone@mail.ru",
                Password = "manager1",
                RoleId = 2
            });
            db.Users.Add(new User
            {
                Email = "managertwo@mail.ru",
                Password = "manager2",
                RoleId = 2
            });
            base.Seed(db);
        }
    }
}