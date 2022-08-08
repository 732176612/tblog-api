/*
 * @Author: your name
 * @Date: 2022-01-28 16:29:07
 * @LastEditTime: 2022-07-02 15:22:09
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\assets\js\utils.js
 */
/**
 * @des: 获取网页链接参数
 * @param {网页链接} URL
 * @return {所有参数对象}
 */
function getParameters(URL) {
    if (URL == undefined || URL == "")
        URL = location.href;
    URL = URL.split("?");
    if (URL[1] != undefined && URL[1] != "") {
        URL = JSON.parse(
            '{"' +
            decodeURI(URL[1])
            .replace(/"/g, '\\"')
            .replace(/&/g, '","')
            .replace(/=/g, '":"') +
            '"}'
        );
        return URL;
    } else {
        return {};
    }
}

function getObjectURL(file) {
    var url = null;
    if (window.createObjectURL != undefined) { // basic
        url = window.createObjectURL(file);
    } else if (window.URL != undefined) { // mozilla(firefox)
        url = window.URL.createObjectURL(file);
    } else if (window.webkitURL != undefined) { // webkit or chrome
        url = window.webkitURL.createObjectURL(file);
    }
    return url;
}

function getCookie(cookie_name) {
    var allcookies = document.cookie;
    var cookie_pos = allcookies.indexOf(cookie_name);
    if (cookie_pos != -1) {
        cookie_pos = cookie_pos + cookie_name.length + 1;
        var cookie_end = allcookies.indexOf(";", cookie_pos);
        if (cookie_end == -1) {
            cookie_end = allcookies.length;
        }
        var value = unescape(allcookies.substring(cookie_pos, cookie_end));
    }
    return value;
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms))
}

//hex颜色转rgb颜色
function HexToRgb(str) {
    var r = /^#?[0-9A-F]{6}$/;
    str = str.replace("#", "");
    var hxs = str.match(/../g);
    for (var i = 0; i < 3; i++) hxs[i] = parseInt(hxs[i], 16);
    return hxs;
}

//GRB颜色转Hex颜色
function RgbToHex(a, b, c) {
    var r = /^d{1,3}$/;
    var hexs = [a.toString(16), b.toString(16), c.toString(16)];
    for (var i = 0; i < 3; i++)
        if (hexs[i].length == 1) hexs[i] = "0" + hexs[i];
    return "#" + hexs.join("");
}

//得到hex颜色值为color的减淡颜色值，level为加深的程度，限0-1之间
function GetLightColor(color, level) {
    var r = /^#?[0-9A-F]{6}$/;
    var rgbc = HexToRgb(color);
    for (var i = 0; i < 3; i++) rgbc[i] = Math.floor((255 - rgbc[i]) * level + rgbc[i]);
    return RgbToHex(rgbc[0], rgbc[1], rgbc[2]);
}

//得到hex颜色值为color的加深颜色值，level为加深的程度，限0-1之间
function getDarkColor(color, level) {
    var r = /^#?[0-9A-F]{6}$/;
    var rgbc = this.HexToRgb(color.toUpperCase());
    for (var i = 0; i < 3; i++) rgbc[i] = Math.floor(rgbc[i] * (1 - level));
    return this.RgbToHex(rgbc[0], rgbc[1], rgbc[2]);
}

//改变主题颜色
function ChangeStyleColor(color) {
    document.documentElement.style.setProperty("--main_color", color);
    document.documentElement.style.setProperty("--main_dark_color", getDarkColor(color, 0.5));
    document.documentElement.style.setProperty("--main_light_color", GetLightColor(color, 0.5));
}

function AutoExtendTextArea() {
    let textareas = document.getElementsByTagName('textarea');
    for (var i = 0; i < textareas.length; i++) {
        let textarea = textareas[i];
        textarea.style.height = textarea.scrollHeight + 'px';
        textarea.addEventListener('input', (e) => {
            console.log(e.target.scrollHeight)
            textarea.style.height = e.target.scrollHeight + 'px';
        });
    }
}

function IsBase64(str){
    if(str === '' || str.trim() === ''){
        return false;
    }
    try{
        return btoa(atob(str)) == str;
    }catch(err){
        return false;
    }
}