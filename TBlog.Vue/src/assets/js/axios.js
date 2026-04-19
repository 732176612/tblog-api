/*
 * @Author: your name
 * @Date: 2021-11-06 13:36:46
 * @LastEditTime: 2022-09-20 15:40:55
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\assets\js\axios.js
 */
/*
 * @Author: your name
 * @Date: 2021-11-06 13:36:46
 * @LastEditTime: 2022-01-17 16:21:38
 * @LastEditors: Please set LastEditors
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\assets\js\axios.js
 */
import axios from "axios";
import toast from "./toast"
// 创建axios实例
const service = axios.create({
  baseURL: "/", // api 的 base_url
  timeout: 60000 // 请求超时时间
})

let showToast = false;

// request 拦截器
service.interceptors.request.use(
  config => {
    return config
  },
  error => {
    Promise.reject(error)
  }
)
// response 拦截器
service.interceptors.response.use(
  response => {
    //console.log(response)
    if (response != null && response != undefined) {
      if (response.status == 200) {

        if (response.data.Status === 200) {
          if (showToast)
            toast.success(response.data.Msg);
        } else {
          toast.warning(response.data.Msg);
        }

        return response.data;
      } else {
        throw response.statusText;
      }
    } else {
      throw null;
    }
  },
  error => {
    toast.error("网络错误：[请刷新页面重试]")
    console.log(error)
    return Promise.reject(error)
  }
)

export default (content, show) => {
  showToast = show == undefined ? true : show;
  return service(content);
}