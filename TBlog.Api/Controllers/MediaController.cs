using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace TBlog.Api
{
    /// <summary>
    /// 媒体文件
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MediaController : TblogController
    {
        private readonly IMediaInfoService _mediaServer;
        private static ILogger<MediaController> _logger;

        public MediaController(IMediaInfoService mediaServer, ILogger<MediaController> logger)
        {
            this._mediaServer = mediaServer;
            _logger = logger;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns>成功返回图片链接，失败返回失败原因</returns>
        [HttpPost]
        public async Task<APITResult<string>> UpLoadImgByFile(string path = "")
        {
            var imgType = new string[] { "image/jpg", "image/png", "image/jpeg" };
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                return APITResult<string>.Fail("请添加要上传的图片");
            }

            if (!imgType.Contains(file.ContentType))
            {
                return APITResult<string>.Fail("图片格式有误，请重新选择");
            }

            if (file.Length > 1024 * 1024 * 2)
            {
                return APITResult<string>.Fail("图片大小不能超过2MB，请重新选择");
            }

            if (file.Length <= 8)
            {
                return APITResult<string>.Fail("不支持该图片");
            }

            string url = await _mediaServer.UpLoadFile(GetToken().UserId, path.ToLower(), file);

            if (string.IsNullOrEmpty(url))
            {
                return APITResult<string>.Fail("上传失败,请稍后再试");
            }
            else
            {
                return APITResult<string>.Success("上传成功", url);
            }
        }

        /// <summary>
        /// 上传图片(支持jpg、png)
        /// </summary>
        /// <returns>成功返回图片链接，失败返回失败原因</returns>
        [HttpPost]
        public async Task<APITResult<string>> UpLoadImgByBase64(UpLoadImageBase64Dto dto)
        {
            if (string.IsNullOrEmpty(dto.Base64))
            {
                return APITResult<string>.Fail("请添加要上传的图片");
            }

            var parseBase64 = FileHelper.IsBase64String(dto.Base64);
            if (!parseBase64.Item1)
            {
                return APITResult<string>.Fail("图片格式有误，请重新选择");
            }

            var data = Convert.FromBase64String(parseBase64.Item2);
            if (data.Length > 1024 * 1024 * 2)
            {
                return APITResult<string>.Fail("图片大小不能超过2MB，请重新选择");
            }

            string url = await _mediaServer.UpLoadFile(GetToken().UserId, dto.Path.ToLower(), data, data.GetHashCode().ToString()+".png");

            if (string.IsNullOrEmpty(url))
            {
                return APITResult<string>.Fail("上传失败,请稍后再试");
            }
            else
            {
                return APITResult<string>.Success("上传成功", url);
            }
        }

        /// <summary>
        /// 上传简历
        /// </summary>
        /// <returns>成功返回简历链接，失败返回失败原因</returns>
        [HttpPost]
        public async Task<APITResult<string>> UpLoadResumeByFile(string path = "")
        {
            var imgType = new string[] { "image/jpg", "image/png", "image/jpeg",
                "application/msword" , "application/pdf",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

            var file = Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                return APITResult<string>.Fail("请添加要上传的简历");
            }

            if (!imgType.Contains(file.ContentType))
            {
                return APITResult<string>.Fail("简历格式只支持jpg,png,jpeg,docx,doc,pdf");
            }

            if (file.Length > 1024 * 1024 * 2)
            {
                return APITResult<string>.Fail("简历大小不能超过2MB，请重新选择");
            }

            string url = await _mediaServer.UpLoadFile(GetToken().UserId, $"resume/{path.ToLower()}", file, Path.GetFileNameWithoutExtension(file.FileName));

            if (string.IsNullOrEmpty(url))
            {
                return APITResult<string>.Fail("上传失败,请稍后再试");
            }
            else
            {
                return APITResult<string>.Success("上传成功", url);
            }
        }
    }
}
