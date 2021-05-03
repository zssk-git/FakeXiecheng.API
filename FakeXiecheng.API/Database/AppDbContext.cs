using FakeXiecheng.API.Moldes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Database
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>//DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            //{
            //    Id = Guid.NewGuid(),
            //    Title = "ceshititle",
            //    Description = "shouming",
            //    OriginalPrice = 0,
            //    CreateTime = DateTime.UtcNow
            //});

            var touristRoutesMockData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRoutesMockData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePicturesMockData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePicturesMockData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            //初始化用户与角色的种子数据
            //1.更新用户与角色的外键
            modelBuilder.Entity<ApplicationUser>(u => u.HasMany(x => x.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired());
            //2.添加管理员角色
            var adminRoleId = "822bb235-faba-485f-816e-7da4bd47170a";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
            //3.添加用户
            var adminUserId = "4eb5db6d-b327-4aed-9785-a7863fd7d5d5";
            ApplicationUser adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@zssk.com",
                NormalizedUserName = "admin@zssk.com".ToUpper(),
                Email = "admin@zssk.com",
                NormalizedEmail = "admin@zssk.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = false
            };
            var ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Zssk@123");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            //4.给用户加入管理员角色
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });


            base.OnModelCreating(modelBuilder);

        }
    }
}
