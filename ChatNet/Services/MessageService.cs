using ChatNet.Abstractions;
using ChatNet.Models;

namespace ChatNet.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<Message>> GetMessagesAsync(int userId)
        {
            return await _messageRepository.GetMessagesAsync(userId);
        }

        public async Task<int> SendMessageAsync(int senderId, int receiverId, string content)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content
            };

            return await _messageRepository.SendMessageAsync(message);
        }
    }
}
