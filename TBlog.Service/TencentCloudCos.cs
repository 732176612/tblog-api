using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Utils;
using Microsoft.AspNetCore.Http;
using System.IO;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ses.V20201002;
using TencentCloud.Ses.V20201002.Models;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;
using TimeUnit = COSXML.Utils.TimeUnit;

namespace TBlog.Service
{
    public static class TencentCloudCos
    {
        #region 初始化
        static CosXmlConfig config;
        static Credential Cred;

        static TencentCloudCos()
        {
            Cred = new Credential
            {
                SecretId = ApiConfig.TencentCloud.SecretId,
                SecretKey = ApiConfig.TencentCloud.SecretKey
            };
            config = new CosXmlConfig.Builder()
            .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位毫秒，默认45000ms
            .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位毫秒，默认45000ms
            .IsHttps(false)  //设置默认 HTTPS 请求
            .SetAppid(ApiConfig.TencentCloud.Appid)  //设置腾讯云账户的账户标识 APPID
            .SetRegion(ApiConfig.TencentCloud.Region)  //设置一个默认的存储桶地域
            .SetDebugLog(true)  //显示日志
            .Build();  //创建 CosXmlConfig 对象
        }

        public static QCloudCredentialProvider GetQCloudCredentialProvider()
        {
            return new DefaultQCloudCredentialProvider(ApiConfig.TencentCloud.SecretId, ApiConfig.TencentCloud.SecretKey, ApiConfig.TencentCloud.DurationSecond);
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        private static string UpLoadFile(PutObjectRequest request)
        {
            CosXml cosXml = new CosXmlServer(config, GetQCloudCredentialProvider());
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.Seconds), 600);//设置签名有效时长

            var respone = cosXml.PutObject(request);
            if (respone.IsSuccessful())
            {
                return $"{ApiConfig.TencentCloud.DoMain}/{request.Key}";
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path">Oss存储路径</param>
        public async static Task<string> UpLoadFile(IFormFile formFile, string path, string fileName = "")
        {
            var timeStamp = DateTimeHelper.ToUnixTimestampByMilliseconds(DateTime.Now).ToString();

            var filePath = $"{AppDomain.CurrentDomain.BaseDirectory}{path}";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath = $"{filePath}/{timeStamp}.tmp";

            using (var stream = File.Create(filePath))
            {
                await formFile.CopyToAsync(stream);
            }
            return UpLoadFile(new PutObjectRequest(ApiConfig.TencentCloud.Bucket, $"{path}/{(string.IsNullOrEmpty(fileName) ? timeStamp : fileName)}{Path.GetExtension(formFile.FileName)}", filePath));
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path">Oss存储路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="data">字节数据</param>
        public static string UpLoadFile(byte[] data, string path, string fileName)
        {
            return UpLoadFile(new PutObjectRequest(ApiConfig.TencentCloud.Bucket, $"{path}/{fileName}", data));
        }
        #endregion

        #region 邮箱
        /// <summary>
        /// 发送注册验证码邮箱
        /// </summary>
        public static bool SendResigterMail(string email, string vcode)
        {
            return SendResigterMail(new string[] { email }, vcode);
        }

        /// <summary>
        /// 发送注册验证码邮箱
        /// </summary>
        public static bool SendResigterMail(string[] emails, string vcode)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("ses.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;
            SesClient client = new SesClient(Cred, "ap-hongkong", clientProfile);
            SendEmailRequest req = new SendEmailRequest();
            req.Destination = emails;
            req.FromEmailAddress = "tblog@mail.falseendless.cn";
            TencentCloud.Ses.V20201002.Models.Template template1 = new TencentCloud.Ses.V20201002.Models.Template();
            template1.TemplateID = 20217;
            template1.TemplateData = "{\"VCode\":\"" + vcode + "\"}";
            req.Template = template1;
            req.Subject = "请验证您的邮箱";
            SendEmailResponse resp = client.SendEmailSync(req);
            var json = AbstractModel.ToJsonString(resp);
            return true;
        }

        /// <summary>
        /// 发送找回密码邮箱
        /// </summary>
        public static bool SendRecoverPwdMail(string email, string vcode)
        {
            return SendRecoverPwdMail(new string[] { email }, vcode);
        }

        /// <summary>
        /// 发送找回密码邮箱
        /// </summary>
        public static bool SendRecoverPwdMail(string[] emails, string vcode)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("ses.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;
            SesClient client = new SesClient(Cred, "ap-hongkong", clientProfile);
            SendEmailRequest req = new SendEmailRequest();
            req.Destination = emails;
            req.FromEmailAddress = "tblog@mail.falseendless.cn";
            TencentCloud.Ses.V20201002.Models.Template template1 = new TencentCloud.Ses.V20201002.Models.Template();
            template1.TemplateID = 23081;
            template1.TemplateData = "{\"VCode\":\"" + vcode + "\"}";
            req.Template = template1;
            req.Subject = "您正在进行找回密码操作(半小时失效)";
            SendEmailResponse resp = client.SendEmailSync(req);
            var json = AbstractModel.ToJsonString(resp);
            return true;
        }
        #endregion

        #region 手机短信
        /// <summary>
        /// 发送注册短信
        /// </summary>
        public static void SendResigterSMS(string phone, string vcode, int vaildMinute = 1)
        {
            SendResigterSMS(new string[] { phone }, vcode, vaildMinute);
        }

        /// <summary>
        /// 发送注册短信
        /// </summary>
        public static void SendResigterSMS(string[] phones, string vcode, int vaildMinute = 1)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("sms.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;

            SmsClient client = new SmsClient(Cred, ApiConfig.TencentCloud.Region, clientProfile);
            SendSmsRequest req = new SendSmsRequest();
            req.PhoneNumberSet = phones;
            req.SmsSdkAppId = "1400587989";
            req.SignName = "覃颖卓个人生活日记";
            req.TemplateId = "1169299";
            req.TemplateParamSet = new string[] { vcode, vaildMinute.ToString() };
            SendSmsResponse resp = client.SendSmsSync(req);
            var json = AbstractModel.ToJsonString(resp);
        }

        /// <summary>
        /// 发送找回密码短信
        /// </summary>
        public static void SendRecoverPwdSMS(string phone, string vcode, int vaildMinute = 1)
        {
            SendRecoverPwdSMS(new string[] { phone }, vcode, vaildMinute);
        }

        /// <summary>
        /// 发送找回密码短信
        /// </summary>
        public static void SendRecoverPwdSMS(string[] phones, string vcode, int vaildMinute = 1)
        {
            ClientProfile clientProfile = new ClientProfile();
            HttpProfile httpProfile = new HttpProfile();
            httpProfile.Endpoint = ("sms.tencentcloudapi.com");
            clientProfile.HttpProfile = httpProfile;

            SmsClient client = new SmsClient(Cred, ApiConfig.TencentCloud.Region, clientProfile);
            SendSmsRequest req = new SendSmsRequest();
            req.PhoneNumberSet = phones;
            req.SmsSdkAppId = "1400587989";
            req.SignName = "覃颖卓个人生活日记";
            req.TemplateId = "1282459";
            req.TemplateParamSet = new string[] { vcode, vaildMinute.ToString() };
            SendSmsResponse resp = client.SendSmsSync(req);
            var json = AbstractModel.ToJsonString(resp);
        }
        #endregion
    }
}