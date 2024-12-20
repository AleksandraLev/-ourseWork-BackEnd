using OrdersService.Model;
using Microsoft.EntityFrameworkCore;

namespace OrdersService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=StoreOrdersDb;Username=postgres;Password=12345");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id); // Установка первичного ключа
                entity.Property(o => o.UserId).IsRequired(); // Обязательное поле UserId
                entity.Property(o => o.OrderDate).IsRequired(); // Обязательное поле OrderDate
                entity.Property(o => o.Status).IsRequired(); // Обязательное поле Status
                entity.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(512); // ShippingAddress

                // Связь Order -> OrderItem (один ко многим)
                entity.HasMany(o => o.Items)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
            });

            // Конфигурация OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id); // Установка первичного ключа
                entity.Property(oi => oi.OrderId).IsRequired(); // Обязательное поле OrderId
                entity.Property(oi => oi.ProductId).IsRequired(); // Обязательное поле ProductId
                entity.Property(oi => oi.Quantity).IsRequired(); // Обязательное поле Quantity
                entity.Property(oi => oi.Price).IsRequired().HasColumnType("decimal(18,2)"); // Обязательное поле Price

                // Связь OrderItem -> Order
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление
            });
        }
    }
}
