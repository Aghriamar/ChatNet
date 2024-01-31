using ChatNet.Models;

namespace ChatNet.Abstractions
{
    public interface IMessageService
    {
        public Task<List<Message>> GetMessagesAsync(int userId);
        public Task<int> SendMessageAsync(int senderId, int receiverId, string content);
    }
}
