using AppServer.Application.Email.QueueEmail;
using AppServer.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;
    private readonly IQueueEmailUseCase _queueEmailUseCase;

    public EmailController(ILogger<EmailController> logger, IQueueEmailUseCase queueEmailUseCase)
    {
        _logger = logger;
        _queueEmailUseCase = queueEmailUseCase;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendAsync([FromBody] EmailMessageModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _queueEmailUseCase.QueueAsync(request, HttpContext.RequestAborted);
            _logger.LogInformation("E-mail enfileirado para {To}", request.To);
            return Ok(new { message = "E-mail enfileirado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enfileirar e-mail");
            return StatusCode(500, new { error = "Erro ao processar requisicao" });
        }
    }
}
