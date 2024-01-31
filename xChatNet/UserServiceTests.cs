using System;
using System.Threading.Tasks;
using ChatNet;
using ChatNet.Abstractions;
using ChatNet.Models;
using ChatNet.Repositories;
using ChatNet.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace xChatNet
{
    public class UserServiceTests
    {
        [Fact]
        public async Task AddAdminAsync_Should_Add_Admin_User()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkProxies()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<MessengerDbContext>()
                .UseInMemoryDatabase(databaseName: "ChatNet")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var dbContext = new MessengerDbContext(builder))
            {
                dbContext.Roles.Add(new Role { Name = "Admin" });
                dbContext.Roles.Add(new Role { Name = "User" });
                await dbContext.SaveChangesAsync();

                // Arrange
                var userRepository = new UserRepository(dbContext);
                var userService = new UserService(userRepository);

                // Act
                await userService.AddAdminAsync("admin@example.com", "adminpassword");

                // Assert
            }
            using (var dbContext = new MessengerDbContext(builder))
            {
                await dbContext.Database.EnsureDeletedAsync();
            }
        }

        [Fact]
        public async Task RegisterUserAsync_Should_Register_User_With_Valid_Password()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            // Act
            var result = await userService.RegisterUserAsync("user@example.com", "validpassword");

            // Assert
            userRepositoryMock.Verify(repo => repo.RegisterUserAsync("user@example.com", "validpassword"), Times.Once);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.True(result.UserId > 0);
        }

        [Fact]
        public async Task RegisterUserAsync_Should_Throw_Exception_For_Short_Password()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await userService.RegisterUserAsync("user@example.com", "short"));
        }

        [Fact]
        public async Task RegisterUserAsync_Should_Throw_Exception_For_Weak_Password()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(userRepositoryMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await userService.RegisterUserAsync("user@example.com", "weakpassword"));
        }
    }
}
