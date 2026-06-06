using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_AchadosEPerdidos.Features.Reivindicacao.Solicitacao;
using API_AchadosEPerdidos.Features.Reivindicacao.Analise;
using API_AchadosEPerdidos.Features.Reivindicacao.ObterPendentes;

namespace API_AchadosEPerdidos.Controllers;

[ApiController]
[Route("reivindicacoes")]
public class ReivindicacaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReivindicacaoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("itens/{itemId:int}/solicitar")]
    public async Task<IActionResult> SolicitarReivindicacaoHandler([FromRoute] int itemId, [FromBody] SolicitarReivindicacaoRequest request)
    {
        var resultado = await _mediator.Send(new SolicitarReivindicacaoCommand(itemId, request));
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }

    [HttpPost("{id:int}/analisar")]
    public async Task<IActionResult> AnalisarSolicitacao([FromRoute] int id, [FromBody] AnalisarSolicitacaoRequest request)
    {
        var resultado = await _mediator.Send(new AnalisarSolicitacaoCommand(id, request));
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }

    [HttpGet("solicitacoes-pendentes")]
    public async Task<IActionResult> ObterSolicitacoes()
    {
        var resultado = await _mediator.Send(new ObterSolicitacoesPendentesQuery());
        await resultado.ExecuteAsync(HttpContext);
        return new EmptyResult();
    }
}