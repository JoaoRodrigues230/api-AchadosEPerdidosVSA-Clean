using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_AchadosEPerdidos.Features.Categorias.ListarCategorias;

namespace API_AchadosEPerdidos.Features.Categorias;

[ApiController]
[Route("categorias")]
public class CategoriaController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObterCategorias()
    {
        var resultado = await _mediator.Send(new ListarCategoriasQuery());
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }
}