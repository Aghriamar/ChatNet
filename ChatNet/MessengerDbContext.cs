using ChatNet.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatNet
{
    public class MessengerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Message> Messages { get; set; }
        private readonly string _connectionString;

        public MessengerDbContext(DbContextOptions<MessengerDbContext> options) : base(options)
        {
        }

        public MessengerDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseLazyLoadingProxies().UseNpgsql(_connectionString);

        public static MessengerDbContext CreateInMemoryDatabase()
        {
            var options = new DbContextOptionsBuilder<MessengerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new MessengerDbContext(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SenderId).IsRequired();
                entity.Property(e => e.ReceiverId).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.IsReceived).IsRequired();
            });
        }
    }
}
