using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TempoAppApi.Context;

public class ApplicationAuthDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationAuthDbContext(DbContextOptions<ApplicationAuthDbContext> options) : base(options)
    {
    }
}