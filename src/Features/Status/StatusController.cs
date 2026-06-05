using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_AchadosEPerdidos.Features.Status.ListarStatus;

namespace API_AchadosEPerdidos.Features.Status;

[ApiController]
[Route("status")]
public class StatusController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObterStatus()
    {
        var resultado = await _mediator.Send(new ListarStatusQuery());
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }
}