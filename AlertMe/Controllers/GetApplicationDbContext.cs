using AlertMe.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertMe.Controllers
{
    public class GetApplicationDbContext
    {
        public static ApplicationDbContext GetApplication()
        {

            ApplicationDbContext application;

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-78TLCS9;Initial Catalog=AlertMe;Integrated Security=True;Pooling=False",
                b => b.UseRowNumberForPaging());
            application = new ApplicationDbContext(optionsBuilder.Options);

            return application;
        }
    }
}
