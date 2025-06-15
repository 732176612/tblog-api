using Microsoft.Extensions.Configuration;
namespace TBlog.Common
{
    public class ApiConfig
    {
        #region 公共部分
        private static IConfiguration Configuration { get; set; }

        public ApiConfig(IConfiguration configuration, string rootPath)
        {
            Configuration = configuration;
            BaseSetting.RootPath = rootPath;
        }

        // 重新加载配置
        public void Reload()
        {
            _BaseSetting = null;
            _DBSetting = null;
            _JwtBearer = null;
            _IdentityServer4 = null;
            _AOPSetting = null;
            _CorsSetting = null;
            _Middleware = null;
            _ConsulSetting = null;
            _RabbitMQSetting = null;
            _RedisSetting = null;
            _TencentCloud = null;
            _Elasticsearch = null;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        public static T GetConfig<T>(string name) where T : new()
        {
            T result = new T();
            Configuration.Bind(name, result);
            return result;
        }
        #endregion

        #region 基础配置(BaseSetting)
        public static BaseSettingConfig BaseSetting
        {
            get
            {
                if (_BaseSetting == null)
                {
                    _BaseSetting = GetConfig<BaseSettingConfig>("BaseSetting");
                }
                return _BaseSetting;
            }
        }
        private static BaseSettingConfig _BaseSetting;

        /// <summary>
        /// 跨域配置
        /// </summary>
        public class BaseSettingConfig
        {
            /// <summary>
            /// 基础
            /// </summary>
            public string ApiName { get; set; }

            /// <summary>
            /// 域名
            /// </summary>
            public string Host { get; set; }

            /// <summary>
            /// 开启测试环境
            /// </summary>
            public bool EnabledTest { get; set; }

            /// <summary>
            /// 是否开启性能分析
            /// </summary>
            public bool MiniProfiler { get; set; }

            /// <summary>
            /// 根路径
            /// </summary>
            public string RootPath { get; set; }
        }
        #endregion

        #region 数据库配置(DBSetting)
        public static DBSettingConfig DBSetting
        {
            get
            {
                if (_DBSetting == null)
                {
                    _DBSetting = GetConfig<DBSettingConfig>("DBSetting");
                }
                return _DBSetting;
            }
        }
        private static DBSettingConfig _DBSetting;

        /// <summary>
        /// 数据库配置
        /// </summary>
        public class DBSettingConfig
        {
            /// <summary>
            /// MongoDB连接字符串
            /// </summary>
            public string MongoConnection
            {
                get
                {
                    if (string.IsNullOrEmpty(_MongoConnection))
                    {
                        _MongoConnection = Configuration["DBSetting:MongoConnection"];
                    }
                    return _MongoConnection;

                }
                set
                {
                    _MongoConnection = value;
                }
            }

            private string _MongoConnection { get; set; }

            /// <summary>
            /// MongoDBBase名称
            /// </summary>
            public string MongoDbName { get; set; }

            /// <summary>
            /// 生成数据库表
            /// </summary>
            public bool SeedDBEnabled { get; set; }

            /// <summary>
            /// 生成数据库种子数据
            /// </summary>
            public bool SeedDBDataEnabled { get; set; }

            /// <summary>
            /// 主库
            /// </summary>
            public DBConfig MainDB
            {
                get
                {
                    if (_MainDB == null)
                    {
                        _MainDB = new DBConfig
                        {
                            Connection = Configuration["DBSetting:MainDB:Connection"],
                            DBType = Configuration["DBSetting:MainDB:DBType"]
                        };
                        _MainDB.Enabled = true;
                        _MainDB.ConfigId = ConstHelper.MainDBConfig;
                    }
                    return _MainDB;
                }
                set
                {
                    _MainDB = value;
                }
            }

            public DBConfig _MainDB { get; set; }

            /// <summary>
            /// 从库
            /// </summary>
            public DBConfig[] SlaveDBs { get; set; } = Array.Empty<DBConfig>();
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        public class DBConfig
        {
            /// <summary>
            /// 数据库ID
            /// </summary>
            public string ConfigId { get; set; }

            /// <summary>
            /// 是否开启
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 数据库类型名称
            /// </summary>
            public string DBType { get; set; }

            /// <summary>
            /// 连接字符串
            /// </summary>
            public string Connection { get; set; }
        }
        #endregion

        #region JwtBearerq权限验证配置(JwtBearer)
        public static JwtBearerConfig JwtBearer
        {
            get
            {
                if (_JwtBearer == null)
                {
                    _JwtBearer = GetConfig<JwtBearerConfig>("JwtBearer");
                }
                return _JwtBearer;
            }
        }
        private static JwtBearerConfig _JwtBearer;

        /// <summary>
        /// JwtBearer配置
        /// </summary>
        public class JwtBearerConfig
        {
            /// <summary>
            /// 是否启用
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 秘钥
            /// </summary>
            public string Secret
            {
                get
                {
                    if (string.IsNullOrEmpty(_Secret))
                    {
                        _Secret = Configuration["JwtBearer:Secret"];
                    }
                    return _Secret;
                }
                set
                {
                    _Secret = value;
                }
            }

            private string _Secret { get; set; }

            /// <summary>
            /// 订阅人
            /// </summary>
            public string Audience { get; set; }

            /// <summary>
            /// 听众
            /// </summary>
            public string Issuer { get; set; }

            /// <summary>
            /// 过期时间
            /// </summary>
            public int ExpressTime { get; set; }
        }
        #endregion

        #region Identity身份验证配置(IdentityServer4)
        public static IdentityServer4Config IdentityServer4
        {
            get
            {
                if (_IdentityServer4 == null)
                {
                    _IdentityServer4 = GetConfig<IdentityServer4Config>("IdentityServer4");
                }
                return _IdentityServer4;
            }
        }
        private static IdentityServer4Config _IdentityServer4;

        /// <summary>
        /// IdentityServer4配置
        /// </summary>
        public class IdentityServer4Config
        {
            /// <summary>
            /// ApiName
            /// </summary>
            public string ApiName { get; set; }

            /// <summary>
            /// 是否启用，启用将取消JwtBearer认证
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// AuthorizationUrl
            /// </summary>
            public string AuthorizationUrl { get; set; }
        }
        #endregion

        #region 拦截器配置(AOPSetting)
        public static AOPSettingConfig AOPSetting
        {
            get
            {
                if (_AOPSetting == null)
                {
                    _AOPSetting = GetConfig<AOPSettingConfig>("AOPSetting");
                }
                return _AOPSetting;
            }
        }
        private static AOPSettingConfig _AOPSetting;

        /// <summary>
        /// AOPSetting配置
        /// </summary>
        public class AOPSettingConfig
        {
            /// <summary>
            /// 是否开启Redis缓存拦截器
            /// </summary>
            public bool RedisCacheAOP { get; set; }

            /// <summary>
            /// 是否开启内存缓存
            /// </summary>
            public bool MemoryCachingAOP { get; set; }

            /// <summary>
            /// 是否开启服务日记
            /// </summary>
            public bool ServerLogAOP { get; set; }

            /// <summary>
            /// 是否开启事务
            /// </summary>
            public bool TransactionProcessAOP { get; set; }

            /// <summary>
            /// 是否开启SQL日记
            /// </summary>
            public bool SqlLogAOP { get; set; }
        }
        #endregion

        #region 跨域配置(CorsSetting)
        public static CorsSettingConfig Cors
        {
            get
            {
                if (_CorsSetting == null)
                {
                    _CorsSetting = GetConfig<CorsSettingConfig>("Cors");
                }
                return _CorsSetting;
            }
        }
        private static CorsSettingConfig _CorsSetting;

        /// <summary>
        /// 跨域配置
        /// </summary>
        public class CorsSettingConfig
        {
            /// <summary>
            /// 是否过滤IP
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 策略名称
            /// </summary>
            public string PolicyName { get; set; }

            /// <summary>
            /// 过滤的IP
            /// </summary>
            public string[] FilterIPs { get; set; }
        }
        #endregion

        #region 中间件配置(Middleware)
        public static MiddlewareConfig Middleware
        {
            get
            {
                if (_Middleware == null)
                {
                    _Middleware = GetConfig<MiddlewareConfig>("Middleware");
                }
                return _Middleware;
            }
        }
        private static MiddlewareConfig _Middleware;

        /// <summary>
        /// 跨域配置
        /// </summary>
        public class MiddlewareConfig
        {
            /// <summary>
            /// 接口请求数据记录路径
            /// </summary>
            public string[] HTTPLogMatchPath { get; set; } = System.Array.Empty<string>();

            /// <summary>
            /// 
            /// </summary>
            public bool SignalR { get; set; }

            /// <summary>
            /// 是否开启IP限流
            /// </summary>
            public bool IpRateLimit { get; set; }
        }
        #endregion

        #region 微服务Consul配置(ConsulSetting)
        public static ConsulSettingConfig Consul
        {
            get
            {
                if (_ConsulSetting == null)
                {
                    _ConsulSetting = GetConfig<ConsulSettingConfig>("Consul");
                }
                return _ConsulSetting;
            }
        }
        private static ConsulSettingConfig _ConsulSetting;

        /// <summary>
        /// 微服务Consul配置
        /// </summary>
        public class ConsulSettingConfig
        {
            /// <summary>
            /// 是否开启
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 服务器名称
            /// </summary>
            public string ServiceName { get; set; }

            /// <summary>
            /// 服务器IP
            /// </summary>
            public string ServiceIP { get; set; }

            /// <summary>
            /// 服务器端口
            /// </summary>
            public string ServicePort { get; set; }

            /// <summary>
            /// 健康检查地址
            /// </summary>
            public string ServiceHealthCheck { get; set; }

            public string HealthURL { get { return $"http://{ServiceIP}:{ServicePort}{ServiceHealthCheck}"; } }

            /// <summary>
            /// Consul地址
            /// </summary>
            public string ConsulAddress { get; set; }

            /// <summary>
            /// 注册延迟时间
            /// </summary>
            public int RegisterDealyTime { get; set; }

            /// <summary>
            /// 健康检查时间间隔
            /// </summary>
            public int CheckHelpInterval { get; set; }

            /// <summary>
            /// 超时时间
            /// </summary>
            public int Timeout { get; set; }
        }
        #endregion

        #region 消息队列RabbitMQ配置(RabbitMQSetting)
        public static RabbitMQSettingConfig RabbitMQ
        {
            get
            {
                if (_RabbitMQSetting == null)
                {
                    _RabbitMQSetting = GetConfig<RabbitMQSettingConfig>("RabbitMQ");
                }
                return _RabbitMQSetting;
            }
        }
        private static RabbitMQSettingConfig _RabbitMQSetting;

        /// <summary>
        /// 消息队列RabbitMQ配置
        /// </summary>
        public class RabbitMQSettingConfig
        {
            /// <summary>
            /// 是否开启
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 连接地址
            /// </summary>
            public string Connection
            {
                get
                {
                    if (string.IsNullOrEmpty(_Connection))
                    {
                        _Connection = Configuration["RabbitMQ.Connection"];
                    }
                    return _Connection;
                }
                set
                {
                    _Connection = value;
                }
            }

            private string _Connection { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string Password
            {
                get
                {
                    if (string.IsNullOrEmpty(_Password))
                    {
                        _Password = Configuration["RabbitMQ.Password"];
                    }
                    return _Password;
                }
                set
                {
                    _Password = value;
                }
            }

            private string _Password { get; set; }

            /// <summary>
            /// 重试次数
            /// </summary>
            public int RetryCount { get; set; }
        }
        #endregion

        #region Redis配置(RedisSetting)
        public static RedisSettingConfig Redis
        {
            get
            {
                if (_RedisSetting == null)
                {
                    _RedisSetting = GetConfig<RedisSettingConfig>("Redis");
                }
                return _RedisSetting;
            }
        }
        private static RedisSettingConfig _RedisSetting;

        /// <summary>
        /// Redis配置
        /// </summary>
        public class RedisSettingConfig
        {
            /// <summary>
            /// 连接连接
            /// </summary>
            public string _Connection { get; set; }

            public string Connection
            {
                get
                {
                    if (string.IsNullOrEmpty(_Connection))
                    {
                        _Connection = Configuration["Redis.Connection"];
                    }
                    return _Connection;
                }
                set
                {
                    _Connection = value;
                }
            }

            /// <summary>
            /// 密码
            /// </summary>
            public string PassWord
            {
                get
                {
                    if (string.IsNullOrEmpty(_PassWord))
                    {
                        _Connection = Configuration["Redis.PassWord"];
                    }
                    return _PassWord;
                }
                set
                {
                    _PassWord = value;
                }
            }

            public string _PassWord { get; set; }
        }
        #endregion

        #region 腾讯云配置(TencentCloud)
        public static TencentCloudConfig TencentCloud
        {
            get
            {
                if (_TencentCloud == null)
                {
                    _TencentCloud = GetConfig<TencentCloudConfig>("TencentCloud");
                }
                return _TencentCloud;
            }
        }

        private static TencentCloudConfig _TencentCloud;

        /// <summary>
        /// Redis配置
        /// </summary>
        public class TencentCloudConfig
        {
            /// <summary>
            /// 设置腾讯云账户的账户标识 APPID
            /// </summary>
            public string Appid
            {
                get
                {
                    if (string.IsNullOrEmpty(_Appid))
                    {
                        _Appid = Configuration["TencentCloud.Appid"];
                    }
                    return _Appid;
                }
                set
                {
                    _Appid = value;
                }
            }

            public string _Appid { get; set; }

            /// <summary>
            /// 设置一个默认的存储桶地域
            /// </summary>
            public string Region
            {
                get
                {
                    if (string.IsNullOrEmpty(_Region))
                    {
                        _Region = Configuration["TencentCloud.Region"];
                    }
                    return _Region;
                }
                set
                {
                    _Region = value;
                }
            }
            public string _Region { get; set; }

            /// <summary>
            /// 云 API 密钥 SecretId
            /// </summary>
            public string SecretId
            {
                get
                {
                    if (string.IsNullOrEmpty(_SecretId))
                    {
                        _SecretId = Configuration["TencentCloud.SecretId"];
                    }
                    return _SecretId;
                }
                set
                {
                    _SecretId = value;
                }
            }
            public string _SecretId { get; set; }

            /// <summary>
            /// 云 API 密钥 SecretKey
            /// </summary>
            public string SecretKey
            {
                get
                {
                    if (string.IsNullOrEmpty(_SecretKey))
                    {
                        _SecretKey = Configuration["TencentCloud.SecretKey"];
                    }
                    return _SecretKey;
                }
                set
                {
                    _SecretKey = value;
                }
            }

            public string _SecretKey { get; set; }

            /// <summary>
            /// 每次请求签名有效时长，单位为秒
            /// </summary>
            public long DurationSecond { get; set; }

            /// <summary>
            /// COS存储桶名称，格式：BucketName-APPID
            /// </summary>
            public string Bucket
            {
                get
                {
                    if (string.IsNullOrEmpty(_Bucket))
                    {
                        _Bucket = Configuration["TencentCloud.Bucket"];
                    }
                    return _Bucket;
                }
                set
                {
                    _Bucket = value;
                }
            }
            public string _Bucket { get; set; }

            /// <summary>
            /// COS存储桶访问域名
            /// </summary>
            public string DoMain
            {
                get
                {
                    if (string.IsNullOrEmpty(_DoMain))
                    {
                        _DoMain = Configuration["TencentCloud.DoMain"];
                    }
                    return _DoMain;
                }
                set
                {
                    _DoMain = value;
                }
            }

            public string _DoMain { get; set; }

        }
        #endregion

        #region Elasticsearch配置(Elasticsearch)
        public static ElasticsearchConfig Elasticsearch
        {
            get
            {
                if (_Elasticsearch == null)
                {
                    _Elasticsearch = GetConfig<ElasticsearchConfig>("Elasticsearch");
                }
                return _Elasticsearch;
            }
        }
        private static ElasticsearchConfig _Elasticsearch;

        /// <summary>
        /// Redis配置
        /// </summary>
        public class ElasticsearchConfig
        {
            /// <summary>
            /// 链接地址
            /// </summary>
            public string Url
            {
                get
                {
                    if (string.IsNullOrEmpty(_Url))
                    {
                        return Configuration["Elasticsearch.Url"];
                    }
                    else
                    {
                        return _Url;
                    }
                }
                set
                {
                    _Url = value;
                }
            }

            private string _Url { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName
            {
                get
                {
                    if (string.IsNullOrEmpty(_UserName))
                    {
                        _UserName = Configuration["Elasticsearch.UserName"];
                    }
                    return _UserName;
                }
                set
                {
                    _UserName = value;
                }
            }

            private string _UserName { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            public string Password
            {
                get
                {
                    if (string.IsNullOrEmpty(_Password))
                    {
                        _Password = Configuration["Elasticsearch.Password"];
                    }
                    return _Password;
                }
                set
                {
                    _Password = value;
                }
            }

            private string _Password { get; set; }

            /// <summary>
            /// 默认索引名
            /// </summary>
            public string DefaultIndex { get; set; }
        }
        #endregion
    }
}
