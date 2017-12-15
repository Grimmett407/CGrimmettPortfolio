namespace CGrimmettPortfolio.Migrations
{
    using CGrimmettPortfolio.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CGrimmettPortfolio.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CGrimmettPortfolio.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "cgrimmett407@gmail.com"))
            {
                userManager.Create(new ApplicationUser         //Creating new user for the application using required fields
                {
                    UserName = "cgrimmett407@gmail.com",
                    DisplayName = "Christian Grimmett",
                    Email = "cgrimmett407@gmail.com",
                    FirstName = "Christian",
                    LastName = "Grimmett",
                }, "Chris407!!");
            }

            if (!context.Users.Any(u => u.Email == "ewatkins@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser         //Creating new user for the application using required fields
                {
                    UserName = "ewatkins@coderfoundry.com",
                    DisplayName = "E. Watkins",
                    Email = "ewatkins@coderfoundry.com",
                    FirstName = "Eric",
                    LastName = "Watkins",
                }, "password1!");
            }

            var adminId = userManager.FindByEmail("cgrimmett407@gmail.com").Id;
            userManager.AddToRole(adminId, "Admin");

            var moderatorId = userManager.FindByEmail("ewatkins@coderfoundry.com").Id;
            userManager.AddToRole(moderatorId, "Moderator");
        }
    }
}
