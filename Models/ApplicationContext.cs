using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplication2.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<authors> authors { get; set; }
    public DbSet<readers> readers { get; set; }
    public DbSet<books> books { get; set; }
    public DbSet<orders> orders { get; set; }

   
}

