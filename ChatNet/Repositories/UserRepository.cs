using ChatNet.Abstractions;
using ChatNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ChatNet.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MessengerDbContext _dbContext;

        public UserRepository(MessengerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAdminAsync(string email, string password)
        {
            var adminUser = new User { Email = email, Password = password };

            var adminRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role { Name = "Admin" };
                _dbContext.Roles.Add(adminRole);
                await _dbContext.SaveChangesAsync();
            }

            adminUser.RoleId = adminRole.Id;
            adminUser.Role = adminRole;

            _dbContext.Users.Add(adminUser);
            await _dbContext.SaveChangesAsync();

            return adminUser.Id;
        }

        public async Task<int> AddUserAsync(User user)
        {
            var userRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "User");

            if (userRole == null)
            {
                userRole = new Role { Name = "User" };
                _dbContext.Roles.Add(userRole);
                await _dbContext.SaveChangesAsync();
            }

            user.RoleId = userRole.Id;
            user.Role = userRole;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user.Id;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _dbContext.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<List<User>> GetUsersAsync(Func<User, bool> value)
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var userToDelete = await _dbContext.Users.FindAsync(userId);

            if (userToDelete == null)
            {
                return false;
            }

            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetUserIdByEmailAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user?.Id ?? 0;
        }

        public async Task<int> GetUserIdFromTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "UserId");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new SecurityTokenException("Invalid UserId claim in token");
            }

            return userId;
        }

        public async Task<int> RegisterUserAsync(string email, string password)
        {
            var newUser = new User { Email = email, Password = password };

            var userRole = _dbContext.Roles.FirstOrDefault(r => r.Name == "User");

            if (userRole == null)
            {
                userRole = new Role { Name = "User" };
                _dbContext.Roles.Add(userRole);
                await _dbContext.SaveChangesAsync();
            }

            newUser.Role = userRole;

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser.Id;
        }
    }
}
