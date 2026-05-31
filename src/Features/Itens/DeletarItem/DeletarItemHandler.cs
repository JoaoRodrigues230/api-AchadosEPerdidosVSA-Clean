using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace API_AchadosEPerdidos.Features.Itens.DeletarItem;

public record DeletarItemCommand(int Id) : IRequest<IResult>;

public class DeletarItemHandler : IRequestHandler<DeletarItemCommand, IResult>
{
    private readonly AppDbContext _context;
    private readonly IStorageService _storageService;

    public DeletarItemHandler(AppDbContext context, IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<IResult> Handle(DeletarItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Itens
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item == null)
        {
            return Results.NotFound(new { mensagem = "Item não encontrado no sistema." });
        }

        //guarda o link do storage antes de apagar o objeto do banco
        string? storageLink = item.StorageLink;

        try
        {
            _context.Itens.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(storageLink))
            {
                var partes = storageLink.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length >= 2)
                {
                    string folderGuid = partes[1];
                    
                    await _storageService.DeleteItemFolderAsync(folderGuid);
                }
            }

            return Results.Ok(new { mensagem = "Item removido com sucesso!" });
        }
        catch (Exception ex)
        {
            return Results.Json(new { 
                erro = "Falha ao tentar deletar o item do sistema.", 
                detalhes = ex.Message 
            }, statusCode: 500);
        }
    }
}