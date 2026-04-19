/*
 * @Author: your name
 * @Date: 2021-11-06 13:36:46
 * @LastEditTime: 2022-07-02 16:41:24
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\assets\js\interface.js
 */
import request from "./axios"
/**
 * @des: 获取正则表达式 (Phone,Mail)
 * @param {regexName} params
 * @return {*}
 */
export function VerifyRegex(params) {
  return request({
    url: "/api/Common/VerifyRegex",
    method: "get",
    params
  }, false)
}

/**
 * @des: 
 * @param {phoneOrMail} params
 * @return {*}
 */
export function CheckHavePhoneOrMail(params) {
  return request({
    url: "/api/User/CheckHavePhoneOrMail",
    method: "get",
    params
  }, false)
}

/**
 * @des: 请求验证码
 * @param {phoneOrMail} params
 * @return {*}
 */
export function RequestVCode(params) {
  return request({
    url: "/api/User/RequestVCode",
    method: "get",
    params
  })
}

/**
 * @des: 普通用户注册
 * @param {email,phone,password,vCode} params
 * @return {*}
 */
export function RegisterUser(params) {
  return request({
    url: "/api/User/RegisterUser",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 用户登录
 * @param {PhoneOrMail,Password} params
 * @return {token}
 */
export function LoginUser(params) {
  return request({
    url: "/api/User/LoginUser",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

export function LogOut() {
  return request({
    url: "/api/User/LogOut",
    method: "POST",
  })
}

/**
 * @des: 申请找回密码
 * @param {phoneOrMail} params
 * @return {*}
 */
export function RequestRecoverPwd(params) {
  return request({
    url: "/api/User/RequestRecoverPwd",
    method: "get",
    params
  })
}

/**
 * @des: 找回密码-重设密码
 * @param {vcode,phoneOrMail,password} params
 * @return {*}
 */
export function ResponeRecoverPwd(params) {
  return request({
    url: "/api/User/ResponeRecoverPwd",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 根据博客名称获取用户信息
 * @param {blogName} 
 * @return {*}
 */
export function GetUserInfo(blogName) {
  return request({
    url: "/api/User/GetUserInfo?blogName=" + (blogName == undefined ? '' : blogName),
    method: "GET"
  }, false)
}

/**
 * @des: 根据Token获取身份信息
 * @param {token} params
 * @return {*}
 */
export function SerializeJwt(params) {
  return request({
    url: "/api/User/SerializeJwt",
    method: "get",
    params
  }, false)
}

/**
 * @des: 检查博客名称是否存在
 * @param {BlogName} params
 * @return {*}
 */
export function CheckHaveBlogName(params) {
  return request({
    url: "/api/User/CheckHaveBlogName",
    method: "get",
    params
  }, false)
}

/**
 * @des: 保存用户信息
 * @param {blogName,headImgUrl,introduction,userName,sex,birthday} params
 * @return {*}
 */
export function SaveUserInfo(params) {
  return request({
    url: "/api/User/SaveUserInfo",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  }, false)
}

/**
 * @des: 上传图片
 * @param {path(保存路径),formFile} params
 * @return {*}
 */
export function UpLoadImgByFile(path, formFile) {
  let formData = new FormData();
  formData.append("files", formFile)
  return request({
    url: "/api/Media/UpLoadImgByFile?path=" + path,
    method: "POST",
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  }, false)
}

/**
 * @des: 上传图片
 * @param {path(保存路径),base64} params
 * @return {*}
 */
export function UpLoadImgByBase64(path, base64) {
  return request({
    url: "/api/Media/UpLoadImgByBase64",
    method: "POST",
    data: {
      path,
      base64
    },
  }, false)
}

/**
 * @des: 上传简历
 * @param {path(保存路径),formFile} params
 * @return {*}
 */
export function UpLoadResumeByFile(path, formFile) {
  let formData = new FormData();
  formData.append("files", formFile)
  return request({
    url: "/api/Media/UpLoadResumeByFile?path=" + path,
    method: "POST",
    data: formData,
    headers: {
      'content-type': 'multipart/form-data'
    }
  }, false)
}

/**
 * @des: 根据角色权限获取菜单
 * @return {*}
 */
export function GetMenus() {
  return request({
    url: "/api/Menu/GetMenus",
    method: "get",
  }, false)
}

/**
 * @des: 检查重复标题
 * @return {*}
 */
export function CheckRepeatTitle(title) {
  return request({
    url: "/api/Acticle/CheckRepeatTitle?title=" + title,
    method: "get",
  }, false)
}

/**
 * @des: 保存文章
 * @param {title,content,posterUrl,tags[...],acticleType,releaseForm} params
 * @return {*}
 */
export function SaveActicle(params) {
  return request({
    url: "/api/Acticle/SaveActicle",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 获取文章
 * @return {*}
 */
export function GetActicle(id) {
  return request({
    url: "/api/Acticle/GetActicle?id=" + id,
    method: "get",
  }, false)
}

/**
 * @des: 获取文章标题汇总
 * @return {*}
 */
export function GetTags(blogName, releaseForm) {
  return request({
    url: "/api/Acticle/GetTags?blogname=" + blogName + "&releaseForm=" + releaseForm,
    method: "get",
  }, false)
}

/**
 * @des: 获取枚举值
 * @return {*}
 */
export function GetEnums(enumNames) {
  return request({
    url: "/api/Common/GetEnums?enumNames=" + enumNames,
    method: "get",
  }, false)
}

/**
 * @des: 获取文章列表
 * @return {*}
 */
export function GetActicleList(params) {
  return request({
    url: "/api/Acticle/GetActicleList",
    method: "get",
    params
  }, false)
}

/**
 * @des: 点赞文章
 * @return {*}
 */
export function LikeArticle(id) {
  return request({
    url: "/api/Acticle/LikeArticle?id=" + id,
    method: "get"
  }, false)
}

/**
 * @des: 查阅文章
 * @return {*}
 */
export function LookArticle(id) {
  return request({
    url: "/api/Acticle/LookArticle?id=" + id,
    method: "get"
  }, false)
}

/**
 * @des: 删除文章
 * @return {*}
 */
export function DeleteArticle(id) {
  return request({
    url: "/api/Acticle/DeleteArticle?id=" + id,
    method: "get"
  }, false)
}

/**
 * @des: 保存项目经历
 * @param [
 * {
 *    Project,
 *    Role,
 *    City,
 *    StartDate,
 *    EndDate,
 *    Introduction
 * }] params
 * @return {token}
 */
export function SaveProjectInfo(params) {
  return request({
    url: "/api/ProjectInfo/Save",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 获取项目经历
 * @param {blogName}
 * @return {token}
 */
export function GetProjectInfo(blogName) {
  return request({
      url: "/api/ProjectInfo/Get?blogname=" + blogName,
      method: "get"
    },
    false)
}

/**
 * @des: 保存工作经历
 * @param [
 * {
 *    Company,
 *    Position,
 *    Department,
 *    City,
 *    StartDate,
 *    EndDate,
 *    Introduction
 * }] params
 * @return {token}
 */
export function SaveCompanyInfo(params) {
  return request({
    url: "/api/CompanyInfo/Save",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 获取工作经历
 * @param {blogName}
 * @return {token}
 */
export function GetCompanyInfo(blogName) {
  return request({
      url: "/api/CompanyInfo/Get?blogname=" + blogName,
      method: "get"
    },
    false)
}

/**
 * @des: 保存教育经历
 * @param [
 * {
 *    School,
 *    Major,
 *    StartDate,
 *    EndDate,
 *    Introduction
 * }] params
 * @return {token}
 */
export function SaveEduInfo(params) {
  return request({
    url: "/api/EduInfo/Save",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 获取教育经历
 * @param {blogName}
 * @return {token}
 */
export function GetEduInfo(blogName) {
  return request({
      url: "/api/EduInfo/Get?blogname=" + blogName,
      method: "get"
    },
    false)
}

/**
 * @des: 保存专业技能
 * @param [
 * {
 *    Skill,
 *    Progress,
 *    Sort
 * }] params
 * @return {token}
 */
export function SaveSkillInfo(params) {
  return request({
    url: "/api/SkillInfo/Save",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 获取专业技能
 * @param {blogName}
 * @return {token}
 */
export function GetSkillInfo(blogName) {
  return request({
      url: "/api/SkillInfo/Get?blogname=" + blogName,
      method: "get"
    },
    false)
}

/**
 * @des: 获取文章评论列表
 * @param {acticleId, pageIndex, pageSize} 
 * @return {*}
 */
export function GetCommentList(acticleId, pageIndex = 1, pageSize = 10) {
  return request({
    url: "/api/Comment/GetCommentList",
    method: "GET",
    params: {
      acticleId,
      pageIndex,
      pageSize
    }
  }, false)
}

/**
 * @des: 获取评论的子评论
 * @param {rootId, pageIndex, pageSize} 
 * @return {*}
 */
export function GetChildComments(rootId, pageIndex = 1, pageSize = 10) {
  return request({
    url: "/api/Comment/GetChildComments",
    method: "GET",
    params: {
      rootId,
      pageIndex,
      pageSize
    }
  }, false)
}

/**
 * @des: 创建评论
 * @param {ActicleId, Content, ParentId} params
 * @return {*}
 */
export function CreateComment(params) {
  return request({
    url: "/api/Comment/CreateComment",
    method: "POST",
    data: params,
    headers: {
      'content-type': 'application/json'
    }
  })
}

/**
 * @des: 点赞评论
 * @param {commentId} 
 * @return {*}
 */
export function LikeComment(commentId) {
  return request({
    url: "/api/Comment/LikeComment",
    method: "GET",
    params: {
      commentId
    }
  })
}

/**
 * @des: 删除评论
 * @param {commentId} 
 * @return {*}
 */
export function DeleteComment(commentId) {
  return request({
    url: "/api/Comment/DeleteComment",
    method: "GET",
    params: {
      commentId
    }
  })
}

/**
 * @des: 获取评论详情
 * @param {commentId} 
 * @return {*}
 */
export function GetCommentDetail(commentId) {
  return request({
    url: "/api/Comment/GetCommentDetail",
    method: "GET",
    params: {
      commentId
    }
  }, false)
}