using API_AchadosEPerdidos.Features.Itens.CreateItem;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_AchadosEPerdidos.Controllers;

[ApiController]
[Route("api/itens")]
public class ItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CriarItem([FromForm] CriarItemCommand command)
    {
        var resultado = await _mediator.Send(command);
        
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }
}