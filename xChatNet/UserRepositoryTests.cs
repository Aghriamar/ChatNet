using System;
using System.Threading.Tasks;
using ChatNet;
using ChatNet.Models;
using ChatNet.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace xChatNet
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task AddAdminAsync_Should_Add_Admin_User_With_Role()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkProxies()
                .BuildServiceProvider();

            var dbContextOptions = new DbContextOptionsBuilder<MessengerDbContext>()
                .UseInMemoryDatabase(databaseName: "ChatNet")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new MessengerDbContext(dbContextOptions))
            {
                var userRepository = new UserRepository(dbContext);

                // Act
                await userRepository.AddAdminAsync("admin@example.com", "adminpassword");

                // Assert
                var adminUser = await dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == "admin@example.com");
                Assert.NotNull(adminUser);
                Assert.Equal("Admin", adminUser.Role?.Name);
            }
            using (var dbContext = new MessengerDbContext(dbContextOptions))
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task AddUserAsync_Should_Add_User()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkProxies()
                .BuildServiceProvider();

            var dbContextOptions = new DbContextOptionsBuilder<MessengerDbContext>()
                .UseInMemoryDatabase(databaseName: "ChatNet")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new MessengerDbContext(dbContextOptions))
            {
                var userRepository = new UserRepository(dbContext);
                var newUser = new User { Email = "user@example.com", Password = "userpassword" };

                // Act
                await userRepository.AddUserAsync(newUser);

                // Assert
                var addedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "user@example.com");
                Assert.NotNull(addedUser);
                Assert.Equal("user@example.com", addedUser.Email);
            }
            using (var dbContext = new MessengerDbContext(dbContextOptions))
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task GetUsersAsync_Should_Return_List_Of_Users()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkProxies()
                .BuildServiceProvider();

            var dbContextOptions = new DbContextOptionsBuilder<MessengerDbContext>()
                .UseInMemoryDatabase(databaseName: "ChatNet")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new MessengerDbContext(dbContextOptions))
            {
                dbContext.Database.EnsureCreated();

                var userRepository = new UserRepository(dbContext);

                // Add some users with roles
                var adminRole = new Role { Name = "Admin" };
                dbContext.Roles.Add(adminRole);

                var adminUser = new User { Email = "admin@example.com", Password = "adminpassword", Role = adminRole };
                dbContext.Users.Add(adminUser);

                var userRole = new Role { Name = "User" };
                dbContext.Roles.Add(userRole);

                var regularUser = new User { Email = "user@example.com", Password = "userpassword", Role = userRole };
                dbContext.Users.Add(regularUser);

                await dbContext.SaveChangesAsync();

                // Act
                var users = await userRepository.GetUsersAsync();

                // Assert
                Assert.NotNull(users);
                Assert.Equal(2, users.Count);
            }
            using (var dbContext = new MessengerDbContext(dbContextOptions))
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }
    }
}