/*
 * @Author: your name
 * @Date: 2021-12-25 14:14:59
 * @LastEditTime: 2022-01-02 11:00:22
 * @LastEditors: Please set LastEditors
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\assets\js\toast.js
 */
import {
    useToast
} from "vue-toastification";
const toast = useToast();

const options = {
    maxToasts: 3,
    newestOnTop: true,
    position: 'top-right',
    timeout: 2000,
    closeOnClick: true,
    pauseOnFocusLoss: true,
    pauseOnHover: false,
    draggable: true,
    draggablePercent: 0.7,
    showCloseButtonOnHover: false,
    hideProgressBar: true,
    closeButton: 'button',
    icon: true,
    rtl: false
};

export default {
    info(msg, option) {
        toast.info(msg, option || options);
    },
    success(msg, option) {
        toast.success(msg, option || options);
    },
    error(msg, option) {
        toast.error(msg, option || options);
    },
    warning(msg, option) {
        toast.warning(msg, option || options);
    },
}