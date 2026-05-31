using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace API_AchadosEPerdidos.Shared.Infrastructure.Storage;

public class CloudflareR2Service : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _publicUrlDomain;

    public CloudflareR2Service(IConfiguration configuration)
    {
        var accountId = configuration["CloudflareR2:AccountId"];
        var accessKey = configuration["CloudflareR2:AccessKey"];
        var secretKey = configuration["CloudflareR2:SecretKey"];
        _bucketName = configuration["CloudflareR2:BucketName"]!;
        _publicUrlDomain = configuration["CloudflareR2:PublicUrlDomain"]!;

        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);
    }

    public async Task<string> UploadItemImageAsync(Stream arquivoStream, string itemFolderGuid, string nomeArquivo, string contentType)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);

        string caminhoCompletoR2 = $"itens/{itemFolderGuid}/{nomeArquivo}";

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = arquivoStream,
            Key = caminhoCompletoR2,
            BucketName = _bucketName,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        await fileTransferUtility.UploadAsync(uploadRequest);

        //url pública do arquivo no R2 pro front acessar
        return $"{_publicUrlDomain}/{caminhoCompletoR2}";
    }

    public async Task DeleteItemFolderAsync(string itemFolderGuid)
    {
        var listRequest = new Amazon.S3.Model.ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = $"itens/{itemFolderGuid}/"
        };

        var listResponse = await _s3Client.ListObjectsV2Async(listRequest);

        foreach (var obj in listResponse.S3Objects)
        {
            await _s3Client.DeleteObjectAsync(_bucketName, obj.Key);
        }
    }
}