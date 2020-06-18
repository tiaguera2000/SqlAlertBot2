using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlertsBot.Domain.Events.Core;
using AlertsBot.WebApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using SharedKernel.DomainEvents.Core;
using SharedKernel.DomainEvents.CrossDomainEvents;
using Microsoft.Extensions.Configuration;
using BitlyAPI;

namespace AlertsBot.WebApi.Controllers
{
    /// <summary>
    /// TFS
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TfsController : ControllerBase
    {

        private readonly ILogger<MessageController> _logger;
        public static IHandler<ICrossDomainEvent> _handler;
        public static IConfiguration _configuration;
        public static string _genericAccessToken = "b40fb45da30abc9db95308062a007b431c5df92a";//Conta leonardooliveira BNE


        public TfsController(IHandler<ICrossDomainEvent>  handler, ILogger<MessageController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _handler = handler;
            _configuration = configuration;

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
        public async Task<IActionResult> PullRequestCreatedAsync([FromBody] PullRequestCreatedViewModel pullRequestCreatedViewModel)
        {
            long chatId = long.Parse(_configuration["chatIdTFS"]);
           

            var a = pullRequestCreatedViewModel;
            var urlTfs = $"http://tfs.employer.com.br:8080/tfs/CaquiCollection/{a.resource.repository.project.name}/_git/{a.resource.repository.name}/pullrequest/{a.resource.pullRequestId}";
            
            var bitly = new Bitly(_genericAccessToken);
            var linkResponse = await bitly.PostShorten(urlTfs);

            var newLink = linkResponse.Link;
            var prMessage = $"⚡️ [PR-{a.resource.pullRequestId}] {a.resource.createdBy.displayName} criou um pull request em {a.resource.repository.name}" +
                $"\n -> {a.resource.title}"+
                (a.resource.description != null ? (!a.resource.description.Equals(a.resource.title) ? $"\n  {a.resource.description}" : "") : "") +
                $"\n {newLink}";
            

            _handler.Handle(new SendMessageTelegramGroup(chatId, prMessage));


            return Ok(new
            {
                success = true,
                data = "ok",
                SentIn = DateTime.Now
            });
        }


    }
}
