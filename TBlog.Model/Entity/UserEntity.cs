namespace TBlog.Model
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserEntity : IEntity
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
        /// 登录账号
        /// </summary>
        [Description("账号")]
        [RegularExpression(ConstHelper.MailRegex, ErrorMessage = "邮箱格式不正确")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 20, IsNullable = true)]
        [StringLength(20, MinimumLength = 6)]
        public string Email { get; set; }

        [RegularExpression(ConstHelper.PhoneRegex, ErrorMessage = "手机号格式不正确")]
        [Description("手机号")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 11, IsNullable = true)]
        [StringLength(11, MinimumLength = 11)]
        public string Phone { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Description("密码")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 32, IsNullable = true)]
        [RegularExpression(ConstHelper.PassWordRegex, ErrorMessage = "要同时含有数字和字母，且长度要在8-16位之间")]
        [StringLength(32, MinimumLength = 32)]
        public string Password { get; set; }

        /// <summary>
        /// 博客名称
        /// </summary>
        [Description("博客名称")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 8, IsNullable = true)]
        [StringLength(8, MinimumLength = 1)]
        public string BlogName { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [Description("头像链接")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 个人介绍
        /// </summary>
        [Description("个人介绍")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 140, IsNullable = true)]
        public string Introduction { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [Description("签名")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 40, IsNullable = true)]
        [StringLength(40)]
        public string Sign { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = ConstHelper.UserNameLength, IsNullable = true)]
        [StringLength(ConstHelper.UserNameLength, MinimumLength = 1)]
        public string UserName { get; set; }

        /// <summary>
        /// 性别 (男：1，女：2，无：0)
        /// </summary>
        [Description("性别")]
        [SugarColumn(IsNullable = true)]
        public EnumSex Sex { get; set; } = 0;

        /// <summary>
        /// 生日
        /// </summary>
        [Description("生日")]
        [SugarColumn(IsNullable = true)]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 简历链接
        /// </summary>
        [Description("简历链接")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string ResumeUrl { get; set; }

        /// <summary>
        /// 简历名称
        /// </summary>
        [Description("简历名称")]
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string ResumeName { get; set; }

        /// <summary>
        /// 微博主题颜色
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 6, IsNullable = true)]
        public string StyleColor { get; set; }

        /// <summary>
        /// 背景海报链接
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 100, IsNullable = true)]
        public string BackgroundUrl { get; set; }

        /// <summary>
        ///最后登录时间 
        /// </summary>
        [Description("最后登录时间")]
        public DateTime LoginDate { get; set; }

        /// <summary>
        /// 角色权限
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public long[] RoleIds { get; set; } = new long[] { };
        #endregion
    }
}
