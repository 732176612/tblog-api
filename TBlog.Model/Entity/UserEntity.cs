namespace TBlog.Model
{
    /// <summary>
    /// 用户
    /// </summary>
    [BsonIgnoreExtraElements]
    public class UserEntity : IDeleteEntity
    {
        #region 基础属性
        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; } = SnowFlakeSingle.instance.NextId();

        /// <summary>
        /// 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime MDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 实体ID
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object EntityId => Id;
        #endregion

        #region 实体属性
        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        [RegularExpression(ConstHelper.MailRegex, ErrorMessage = "邮箱格式不正确")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 20)]
        [StringLength(20, MinimumLength = 6)]
        public string Email { get; set; }

        [RegularExpression(ConstHelper.PhoneRegex, ErrorMessage = "手机号格式不正确")]
        [Description("手机号")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 11)]
        [StringLength(11, MinimumLength = 11)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 登录密码
        /// </summary>
        [Description("密码")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 32)]
        [RegularExpression(ConstHelper.PassWordRegex, ErrorMessage = "要同时含有数字和字母，且长度要在8-16位之间")]
        [StringLength(32, MinimumLength = 32)]
        public string Password { get; set; }

        /// <summary>
        /// 博客名称
        /// </summary>
        [Description("博客名称")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 20)]
        [StringLength(20, MinimumLength = 1)]
        public string BlogName { get; set; } = string.Empty;

        /// <summary>
        /// 头像链接
        /// </summary>
        [Description("头像链接")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 255)]
        public string HeadImgUrl { get; set; } = string.Empty;

        /// <summary>
        /// 个人介绍
        /// </summary>
        [Description("个人介绍")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 510)]
        public string Introduction { get; set; } = string.Empty;

        /// <summary>
        /// 签名
        /// </summary>
        [Description("签名")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 100)]
        [StringLength(100)]
        public string Sign { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = ConstHelper.UserNameLength)]
        [StringLength(ConstHelper.UserNameLength, MinimumLength = 1)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 性别 (男：1，女：2，无：0)
        /// </summary>
        [Description("性别")]
        public EnumSex Sex { get; set; } = 0;

        /// <summary>
        /// 生日
        /// </summary>
        [Description("生日")]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 简历链接
        /// </summary>
        [Description("简历链接")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 255)]
        public string ResumeUrl { get; set; } = string.Empty;

        /// <summary>
        /// 简历名称
        /// </summary>
        [Description("简历名称")]
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 100)]
        public string ResumeName { get; set; } = string.Empty;

        /// <summary>
        /// 微博主题颜色
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 7)]
        public string StyleColor { get; set; } = string.Empty;

        /// <summary>
        /// 背景海报链接
        /// </summary>
        [SugarColumn(ColumnDataType = "VARCHAR", Length = 255)]
        public string BackgroundUrl { get; set; } = string.Empty;

        /// <summary>
        ///最后登录时间 
        /// </summary>
        [Description("最后登录时间")]
        [SugarColumn(IsNullable = true)]
        public DateTime? LoginDate { get; set; }

        /// <summary>
        /// 角色权限
        /// </summary>
        [SugarColumn(IsJson = true, Length = 2000)]
        public long[] RoleIds { get; set; } = [];
        #endregion
    }
}
