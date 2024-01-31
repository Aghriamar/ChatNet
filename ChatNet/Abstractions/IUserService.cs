using ChatNet.Models;

namespace ChatNet.Abstractions
{
    public interface IUserService
    {
        public Task<int> AddAdminAsync(string email, string password);
        public Task<int> AddUserAsync(string email, string password);
        public Task<List<User>> GetUsersAsync();
        public Task<bool> DeleteUserAsync(int userId);
        public Task<int> GetUserIdFromTokenAsync(string token);
        public Task<RegisterUserResult> RegisterUserAsync(string email, string password);
    }
}
