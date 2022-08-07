using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using TBlog.Common;
using TBlog.IService;
using TBlog.Model;
using TBlog.Repository;
using TBlog.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Xunit;
using TBlog.Api;
using Microsoft.DotNet.PlatformAbstractions;
using TBlog.IRepository;
using TBlog.Extensions;
using MongoDB.Driver;

namespace TBlog.Test
{
    public class ApiTest
    {
        Init_Test Init = new Init_Test();

        [Fact]
        public async void InitRole()
        {
            var roleRepository = ContainerHelper.Resolve<IRoleRepository>();

            var systemRole = new RoleEntity()
            {
                Id = 10000,
                Desc = "超级管理员",
                Enabled = true,
                Name = ConstHelper.SystemRole,
                OrderSort = 0
            };

            await roleRepository.AddEntity(systemRole);

            var userRole = new RoleEntity()
            {
                Id = 20000,
                OrderSort = 1,
                PId = (await roleRepository.GetByName(ConstHelper.SystemRole)).Id,
                Name = ConstHelper.UserRole,
                Enabled = true,
                Desc = "普通用户"
            };

            await roleRepository.AddEntity(userRole);
        }

        [Fact]
        public async void InitMenu()
        {
            var menuRepository = ContainerHelper.Resolve<IMenuRepository>();

             await menuRepository.Delete(c=>true);

            var indexEntity = new MenuEntity()
            {
                Id = 10000,
                Url = "/view/index",
                Enabled = true,
                Name = "首页",
                RoleIds = new long[] { 10000, 20000 },
                OrderSort = 1
            };
            await menuRepository.AddEntity(indexEntity);

            var articleEntity = new MenuEntity()
            {
                Id = 20000,
                OrderSort = 2,
                Name = "文章",
                Enabled = true,
                RoleIds = new long[] { 10000, 20000 },
                Url = "/view/index/article"
            };
            await menuRepository.AddEntity(articleEntity);

            var userInfoEntity = new MenuEntity()
            {
                Id = 30000,
                OrderSort = 3,
                Name = "个人信息",
                Enabled = true,
                RoleIds = new long[] { 10000, 20000 },
                Url = "/view/index/userinfo"
            };

            await menuRepository.AddEntity(userInfoEntity);
        }
    }
}