using ContactManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;



// dotnet aspnet-codegenerator razorpage -m Contact -dc ApplicationDbContext -udl -outDir Pages\Contacts --referenceScriptLibraries

namespace ContactManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw = "")
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
     
           // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything
                var adminID = await EnsureUser(serviceProvider, testUserPw, "gauravmehta@workoholic.com");
                await EnsureRole(serviceProvider, adminID, "ContactAdministrators");

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, testUserPw, "rupalbhatia@workoholic.com");
                await EnsureRole(serviceProvider, managerID, "ContactManagers");

                SeedDB(context, adminID);
            }
        }
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                            string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Contact.Any())
            {
                return;   // DB has been seeded
            }

            context.Contact.AddRange(
                new Contact
                {
                    Name = "Gaurav Mehta",
                    Address = "144 Bell Farm Road",
                    City = "Barrie",
                    State = "ON",
                    Zip = "L4M 5K5",
                    Email = "gauravmehta@workoholic.com",
                    Status = ContactStatus.Approved,
                    OwnerID = adminID
                },
                new Contact
                {
                    Name = "Rupal Bhatia",
                    Address = "18 Drive View",
                    City = "Barrie",
                    State = "ON",
                    Zip = "L4N G5K",
                    Email = "rupalbhatia@workoholic.com",
                    Status = ContactStatus.Rejected,
                    OwnerID = adminID
                },
                new Contact
                {
                    Name = "Andrew Tate",
                    Address = "9012 State Street",
                    City = "Redmond",
                    State = "BC",
                    Zip = "P9K Q2K",
                    Email = "atate@workoholic.com"
                },
                new Contact
                {
                    Name = "Drew McIntyre",
                    Address = "3456 Maple Street",
                    City = "Nevada",
                    State = "LA",
                    Zip = "10999",
                    Email = "dmcIntyre@workoholic.com"
                },
                new Contact
                {
                    Name = "Sonam Bajwa",
                    Address = "7890 2nd Ave E",
                    City = "Vancouver",
                    State = "BC",
                    Zip = "4K1 Q1P",
                    Email = "sbajwa@workoholic.com"
                }
             ) ;
            context.SaveChanges();
        }

    }
}