using System;
using System.Collections.Generic;
using System.Text;
using AlertMe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlertMe.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {

        public DbSet<Alert> Alerts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<UsersAlerts> UserAlerts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AlertForSubscribed> AlertForSubscribers { get; set; }
        public DbSet<Follow> Follows { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
