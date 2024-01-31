using ChatNet.Abstractions;
using ChatNet.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatNet.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> AddAdminAsync(string email, string password)
        {
            var existingAdmin = await _userRepository.GetUsersAsync(u => u.Role.Name == "Admin");

            if (existingAdmin.Any())
            {
                throw new InvalidOperationException("Admin already exists");
            }
            return await _userRepository.AddAdminAsync(email, password);
        }

        public async Task<int> AddUserAsync(string email, string password)
        {
            var newUser = new User { Email = email, Password = password };
            return await _userRepository.AddUserAsync(newUser);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<int> GetUserIdFromTokenAsync(string token)
        {
            return await _userRepository.GetUserIdFromTokenAsync(token);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<RegisterUserResult> RegisterUserAsync(string email, string password)
        {
            if (password.Length < 6)
            {
                throw new ArgumentException("Password should be at least 6 characters long");
            }
            if (!IsStrongPassword(password))
            {
                throw new ArgumentException("Password should meet complexity requirements");
            }
            var userId = await _userRepository.GetUserIdByEmailAsync(email);
            var token = GenerateJwtToken(userId);
            return new RegisterUserResult
            {
                UserId = userId,
                Token = token
            };
        }

        private bool IsStrongPassword(string password)
        {
            return password.Length >= 6 && password.Any(char.IsDigit) && password.Any(char.IsUpper);
        }

        public string GenerateJwtToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_secret_key_here");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim("UserId", userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
