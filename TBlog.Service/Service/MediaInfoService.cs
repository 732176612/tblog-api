using Microsoft.AspNetCore.Http;
namespace TBlog.Service
{
    public class MediaInfoService : BaseService<MediaInfoEntity>, IMediaInfoService
    {
        public MediaInfoService(IMongoRepository<MediaInfoEntity> baseRepository) : base(baseRepository) { }

        public async Task<string> UpLoadFile(long userId, string path, IFormFile formFile, string fileName = "")
        {
            var url = await TencentCloudCos.UpLoadFile(formFile, $"resource/{userId}/{path}", fileName);
            if (string.IsNullOrEmpty(url) == false)
            {
                await baseRepository.AddEntity(new MediaInfoEntity
                {
                    Id = IdBuilder.CreateId(),
                    Size = formFile.Length,
                    CUserId = userId,
                    FileName = formFile.FileName,
                    MediaType = EnumMediaType.Image,
                    Url = url
                });
            }
            return url;
        }

        public async Task<string> UpLoadFile(long userId, string path, byte[] fileByte, string fileName)
        {
            var url = TencentCloudCos.UpLoadFile(fileByte, $"resource/{userId}/{path}", fileName);
            if (string.IsNullOrEmpty(url) == false)
            {
                await baseRepository.AddEntity(new MediaInfoEntity
                {
                    Id = IdBuilder.CreateId(),
                    Size = fileByte.Length,
                    CUserId = userId,
                    FileName = fileName,
                    MediaType = EnumMediaType.Image,
                    Url = url
                });
            }
            return url;
        }
    }
}
