using ChatNet.Models;

namespace ChatNet.Abstractions
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessagesAsync(int userId);
        Task<int> SendMessageAsync(Message message);
        Task<Message> GetMessageByIdAsync(int messageId);
        Task UpdateMessageAsync(Message message);
    }
}
