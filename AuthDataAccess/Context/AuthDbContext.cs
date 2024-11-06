using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDataAccess.Context
{
        public class AuthDbContext : IdentityDbContext<IdentityUser>
        {
            public AuthDbContext(DbContextOptions<AuthDbContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                // Konfigurer tabellnavn for Identity
                builder.Entity<IdentityUser>().ToTable("Users");
                builder.Entity<IdentityRole>().ToTable("Roles");
                builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
                builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
                builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
                builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
                builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            }
        }
    }

}
