using Microsoft.EntityFrameworkCore;

namespace SmartHome.API.Infrastructure.Data
{
    public class SmartHomeDbContext : DbContext
    {
        public SmartHomeDbContext(DbContextOptions<SmartHomeDbContext> options) : base(options) { }

        public DbSet<DeviceEntity> Devices { get; set; }
        public DbSet<UserEntity> Users { get; set; } // Yeni tablomuz eklendi

        // Veritabanı ilk oluştuğunda içine varsayılan kullanıcıları ekliyoruz (Data Seeding)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }
    }
}