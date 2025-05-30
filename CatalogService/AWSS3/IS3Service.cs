namespace CatalogService.AWSS3
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(Stream fileStream);
    }
}
