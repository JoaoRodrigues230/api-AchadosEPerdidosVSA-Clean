namespace API_AchadosEPerdidos.Shared.Infrastructure.Storage;

public interface IStorageService
{
    //faz o upload de uma imagem para uma pasta específica do item e retorna a URL pública dela
    Task<string> UploadItemImageAsync(Stream arquivoStream, string itemFolderGuid, string nomeArquivo, string contentType);
    
    //remove a pasta ou um arquivo específico do item
    Task DeleteItemFolderAsync(string itemFolderGuid);
}