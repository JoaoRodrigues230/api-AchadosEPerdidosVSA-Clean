using MediatR;
using Microsoft.AspNetCore.Mvc;
using API_AchadosEPerdidos.Features.Users.CreateUser;
using API_AchadosEPerdidos.Features.Users.Login;

namespace API_AchadosEPerdidos.Features.Users;

[ApiController]
[Route("usuario")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CriarUsuario(CreateUserRequest request)
    {
        var userId = await _mediator.Send(new CreateUserCommand(request));
        return Ok(new { id = userId, message = "Usuário cadastrado com sucesso!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _mediator.Send(new LoginCommand(request));

        if (response == null)
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(response);
    }
}