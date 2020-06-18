using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlertsBot.Domain.Events.Core;
using AlertsBot.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedKernel.DomainEvents.Core;
using SharedKernel.DomainEvents.CrossDomainEvents;

namespace AlertsBot.WebApi.Controllers
{
    /// <summary>
    /// Message
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {

        private readonly ILogger<MessageController> _logger;
        public static IHandler<ICrossDomainEvent> _handler;


        public MessageController(IHandler<ICrossDomainEvent> handler, ILogger<MessageController> logger)
        {
            _logger = logger;
            _handler = handler;

        }

        /// <summary>
        /// Sends a message in a telegram group in which AlertsBot is a member.
        /// </summary>
        /// <remarks>
        /// Ao utilizar esse endpoint você se responsabiliza por qualquer spam realizado com o nosso bot. Use com cuidado e autorização!
        /// </remarks>
        /// <response code="200">Message queued for sending!</response>
        /// <response code="400">The submission has validation errors!</response>
        /// <response code="500">Oops, I can't queue anything right now!</response>
        [HttpPost]
        public IActionResult SendMessage([FromBody] MessageViewModel message)
        {
            _handler.Handle(new SendMessageTelegramGroup(long.Parse(message.ChatId), message.Message));
            return Ok(new
            {
                success = true,
                data = message,
                SentIn = DateTime.Now
            });
        }


    }
}
