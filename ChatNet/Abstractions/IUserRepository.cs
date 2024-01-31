using ChatNet.Models;

namespace ChatNet.Abstractions
{
    public interface IUserRepository
    {
        public Task<int> AddAdminAsync(string email, string password);
        public Task<int> AddUserAsync(User user);
        public Task<List<User>> GetUsersAsync();
        public Task<List<User>> GetUsersAsync(Func<User, bool> value);
        public Task<bool> DeleteUserAsync(int userId);
        public Task<int> GetUserIdFromTokenAsync(string token);
        public Task<int> RegisterUserAsync(string email, string password);
        public Task<int> GetUserIdByEmailAsync(string email);
    }
}
