using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
    public class AspDotNetDbContext : IdentityDbContext
    {
        public AspDotNetDbContext(DbContextOptions<AspDotNetDbContext> options) : base(options)
        {

        }
    }
}
