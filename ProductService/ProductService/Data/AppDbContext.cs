using ProductsService.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ProductsService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=StoreProductsDb;Username=postgres;Password=12345");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id); // Первичный ключ
                entity.Property(p => p.Name).IsRequired().HasMaxLength(128); // Поле Name
                entity.Property(p => p.Price).IsRequired(); // Поле Price
                entity.Property(p => p.Description).HasMaxLength(512); // Поле Description
                entity.Property(p => p.StockQuantity).IsRequired().HasColumnType("decimal(18,2)"); ; // Поле StockQuantity

                // Указание связи "многие к одному" между Product и Category
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade); // При удалении категории удалить связанные продукты
            });

            // Конфигурация модели Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id); // Первичный ключ
                entity.Property(c => c.Name).IsRequired().HasMaxLength(128); // Поле Name
            });
        }
    }
}
