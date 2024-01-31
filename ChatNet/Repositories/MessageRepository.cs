using ChatNet.Abstractions;
using ChatNet.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessengerDbContext _dbContext;

        public MessageRepository(MessengerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Message>> GetMessagesAsync(int userId)
        {
            return await _dbContext.Messages
                .Where(m => m.ReceiverId == userId && !m.IsReceived)
                .ToListAsync();
        }

        public async Task<int> SendMessageAsync(Message message)
        {
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
            return message.Id;
        }

        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _dbContext.Messages.FindAsync(messageId);
        }

        public async Task UpdateMessageAsync(Message message)
        {
            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync();
        }
    }
}
