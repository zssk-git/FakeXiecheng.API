using FakeXiecheng.API.Moldes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<TouristRoute> touristRoutes { get; set; }
        public DbSet<TouristRoutePicture> touristRoutePictures { get; set; }
    }
}
