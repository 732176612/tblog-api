using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBlog.Model;

namespace TBlog.IService
{
    public interface IMediaInfoService : IBaseService<MediaInfoEntity>
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userId">上传者Id</param>
        /// <param name="path">oos文件夹路径</param>
        /// <param name="formFile">文件</param>
        public Task<string> UpLoadFile(long userId, string path, IFormFile formFile, string fileName = "");

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userId">上传者Id</param>
        /// <param name="path">oos文件夹路径</param>
        /// <param name="fileByte">文件字节数据</param>
        public Task<string> UpLoadFile(long userId, string path, byte[] fileByte, string fileName = "");
    }
}
