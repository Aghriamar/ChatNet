using ChatNet.Abstractions;
using ChatNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatNet.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageService messageService, IMessageRepository messageRepository)
        {
            _messageService = messageService;
            _messageRepository = messageRepository;
        }

        [HttpGet("{userId}/get")]
        public async Task<IActionResult> GetMessages(int userId)
        {
            try
            {
                var messages = await _messageService.GetMessagesAsync(userId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = "Internal Server Error" });
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var messageId = await _messageService.SendMessageAsync(request.SenderId, request.ReceiverId, request.Content);
                return Ok(new { MessageId = messageId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = "Internal Server Error" });
            }
        }

        [HttpPut("{messageId}/mark-as-received")]
        public async Task<IActionResult> MarkMessageAsReceived(int messageId)
        {
            try
            {
                var message = await _messageRepository.GetMessageByIdAsync(messageId);

                if (message == null)
                {
                    return NotFound(new { ErrorMessage = "Message not found" });
                }

                if (message.IsReceived)
                {
                    return BadRequest(new { ErrorMessage = "Message has already been marked as received" });
                }

                message.IsReceived = true;
                await _messageRepository.UpdateMessageAsync(message);

                return Ok(new { MessageId = messageId, Status = "Marked as received" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = "Internal Server Error" });
            }
        }
    }
}
