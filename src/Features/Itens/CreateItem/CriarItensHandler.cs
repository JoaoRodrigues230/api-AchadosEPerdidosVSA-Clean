using API_AchadosEPerdidos.Shared.Infrastructure.Storage;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Features.Itens.Models;

namespace API_AchadosEPerdidos.Features.Itens.CreateItem;

public record CriarItemCommand(
    string Nome,
    DateTime DataAchado,
    int LocalId,
    int CategoriaId,
    string? Descricao,
    int StatusId,
    string? InfoAdd,
    List<IFormFile> Fotos
) : IRequest<IResult>;

public class CriarItensHandler : IRequestHandler<CriarItemCommand, IResult>
{
    private readonly AppDbContext _context; 
    private readonly IStorageService _storageService;

    public CriarItensHandler(AppDbContext context, IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<IResult> Handle(CriarItemCommand request, CancellationToken cancellationToken)
    {
        if (request.Fotos != null && request.Fotos.Count > 3)
        {
            return Results.BadRequest(new { mensagem = "O limite máximo permitido é de até 3 imagens por item." });
        }

        string pastaItemId = Guid.NewGuid().ToString();
        string? storageLink = request.Fotos != null && request.Fotos.Count > 0 ? $"itens/{pastaItemId}/" : null;

        var novoItem = new Item
        {
            Nome = request.Nome,
            DataAchado = request.DataAchado,
            LocalId = request.LocalId,
            CategoriaId = request.CategoriaId,
            Descricao = request.Descricao,
            StatusId = request.StatusId,
            StorageLink = storageLink,
            InfoAdd = request.InfoAdd
        };

        try
        {
            if (request.Fotos != null && request.Fotos.Count > 0)
            {
                int contador = 1;
                foreach (var foto in request.Fotos)
                {
                    var extensao = Path.GetExtension(foto.FileName);
                    var nomeArquivoFormatado = $"foto_{contador}{extensao}";

                    using var stream = foto.OpenReadStream();
                    
                    //upload para o Cloudflare R2
                    string urlPublica = await _storageService.UploadItemImageAsync(
                        stream, 
                        pastaItemId, 
                        nomeArquivoFormatado, 
                        foto.ContentType
                    );

                    novoItem.Imagens.Add(new ItemImage
                    {
                        Url = urlPublica,
                        IsMain = (contador == 1) //primeira imagem vira a principal por padrão
                    });

                    contador++;
                }
            }

            _context.Set<Item>().Add(novoItem);
            await _context.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { 
                mensagem = "Item cadastrado com sucesso!", 
                itemId = novoItem.Id,
                pastaStorage = storageLink 
            });
        }
        catch (Exception ex)
        {
            //fallback de segurança: remove do R2 se o banco falhar
            await _storageService.DeleteItemFolderAsync(pastaItemId);
            
            return Results.Json(new { 
                erro = "Falha ao cadastrar o item no banco de dados.", 
                detalhes = ex.InnerException?.Message ?? ex.Message 
            }, statusCode: 500);
        }
    }
}