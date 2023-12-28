using Caching.Models;
using Microsoft.EntityFrameworkCore;

namespace Caching.Data;

public class ApplicationDbContext:DbContext
{
    
 public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options){}
    
    public DbSet<Driver> Drivers { get; set; }
}