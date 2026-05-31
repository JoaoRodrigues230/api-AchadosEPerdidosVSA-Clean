using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_AchadosEPerdidos.Features.Itens.CreateItem;
using API_AchadosEPerdidos.Features.Itens.BuscarItensHandler;

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

    [HttpGet]
    public async Task<IActionResult> BuscarItens([FromQuery] BuscarItensQuery query)
    {
        var resultado = await _mediator.Send(query);
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }
}