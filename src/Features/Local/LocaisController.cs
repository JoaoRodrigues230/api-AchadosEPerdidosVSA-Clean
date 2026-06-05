using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_AchadosEPerdidos.Features.Locais.ListarLocais;

namespace API_AchadosEPerdidos.Features.Locais;

[ApiController]
[Route("locais")]
public class LocalController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObterLocais()
    {
        var resultado = await _mediator.Send(new ListarLocaisQuery());
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }
}