<!--
 * @Author: your name
 * @Date: 2021-10-30 16:34:52
 * @LastEditTime: 2022-06-04 16:04:37
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\Register.vue
-->
<template>
    <div class="container-fluid h-100">
        <div class="row align-items-center justify-content-center" style="height:95vh">
            <div class="col-xl-4 col-lg-4 col-md-6 col-sm-8 col-10">
                <div class="card">
                    <div class="card-header text-center" style="border-top: 2px solid var(--blue)">
                        <img class="logoImg" src="../assets/img/logo.png" />
                    </div>
                    <div class="card-body">
                        <p class="login-box-msg h3 text-center">注册</p>
                        <form class="needs-validation">
                            <div class="input-group mb-3">
                                <input ref="Mail" type="text" class="form-control"
                                    :class="Mail.length!=0? MailInVailTip==''?'is-valid':'is-invalid':''"
                                    placeholder="邮箱" :pattern="MailRegex" v-model="Mail" @input="CheckHaveMail"
                                    required>
                                <div class="invalid-feedback">
                                    {{MailInVailTip}}
                                </div>
                            </div>
                            <div class="input-group mb-3">
                                <input ref="Phone" type="text" class="form-control"
                                    :class="Phone.length!=0? PhoneInVailTip==''?'is-valid':'is-invalid':''"
                                    placeholder="手机" :pattern="PhoneRegex" v-model="Phone" @input="CheckHavePhone"
                                    required>
                                <div class="invalid-feedback">
                                    {{PhoneInVailTip}}
                                </div>
                            </div>
                            <div class="input-group mb-3" :class="IsSumbit||PassWord.length!=0?'was-validated':''">
                                <input ref="PassWord" type="password" class="form-control" placeholder="密码"
                                    :pattern="PassWordRegex" v-model="PassWord" required>
                                <div class="invalid-feedback">
                                    要同时含有数字和字母，且长度要在8-16位之间
                                </div>
                            </div>
                            <div class="input-group mb-3" :class="IsSumbit||VCode.length!=0?'was-validated':''">
                                <div class="input-group-prepend">
                                    <loadingbtn class="btn-main" style="background-color:var(--orange);
                                border-color:var(--orange)" :awaitAction="SendVCode" :btnText="VCodeTip"></loadingbtn>
                                </div>
                                <!-- /btn-group -->
                                <input ref="VCode" type="text" class="form-control" pattern="\d{4}$" v-model="VCode"
                                    required>
                                <div class="invalid-feedback">
                                    请输入正确的验证码
                                </div>
                            </div>
                        </form>
                        <div class="d-grid gap-2 text-center mt-2 mb-3">
                            <loadingbtn class="btn-block btn-main" :awaitAction="RequestRegisterUser" :btnText="'注册'">
                            </loadingbtn>
                        </div>
                        <p class="mb-0">
                            <a href="/view/login" class="text-center">我已经有了账号</a>
                        </p>
                    </div>
                </div>
                <div class="mt-2 text-center" style="height:5vh">
                    <a href="http://beian.miit.gov.cn" target="_blank" class="h6 mt-5 text-black-50">©TBlog -
                        粤ICP备20006712号。</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import {
        RequestVCode,
        RegisterUser,
        VerifyRegex,
        CheckHavePhoneOrMail
    } from '../assets/js/interface.js'
    export default {
        data() {
            return {
                VCodeTip: "发送验证码",
                PhoneRegex: "",
                MailRegex: "",
                PassWordRegex: "",
                Phone: "",
                PassWord: "",
                Mail: "",
                VCode: "",
                IsSumbit: "",
                MailInVailTip: "",
                PhoneInVailTip: ""
            }
        },
        methods: {
            async GetVerifyRegex() {
                let regexs = (await VerifyRegex({
                    regexName: "Phone,Mail,PassWord"
                })).Data;

                for (var regex in regexs) {
                    if (regexs[regex].Key == 'Phone') {
                        this.PhoneRegex = regexs[regex].Value;
                    } else if (regexs[regex].Key == 'Mail') {
                        this.MailRegex = regexs[regex].Value;
                    } else if (regexs[regex].Key == 'PassWord') {
                        this.PassWordRegex = regexs[regex].Value;
                    }
                }
            },
            async CheckHavePhone() {
                if (this.Phone.length != 0) {
                    if (this.$refs.Phone.checkValidity()) {
                        var respone = await CheckHavePhoneOrMail({
                            'phoneOrMail': this.Phone
                        });
                        if (respone != null && respone.Status == 500) {
                            this.PhoneInVailTip = '该手机号码已被注册';
                        } else {
                            this.PhoneInVailTip = '';
                        }
                    } else {
                        this.PhoneInVailTip = '手机号码格式不正确';
                    }
                } else {
                    this.PhoneInVailTip = '';
                }
            },
            async CheckHaveMail() {
                if (this.Mail.length != 0) {
                    if (this.$refs.Mail.checkValidity()) {
                        var respone = await CheckHavePhoneOrMail({
                            'phoneOrMail': this.Mail
                        });
                        if (respone != null && respone.Status == 500) {
                            this.MailInVailTip = '该邮箱已被注册';
                        } else {
                            this.MailInVailTip = '';
                        }
                    } else {
                        this.MailInVailTip = '邮箱格式不正确';
                    }
                } else {
                    this.MailInVailTip = '';
                }
            },
            async SendVCode() {
                if (this.CheckPhoneOrMail() == false) {
                    this.$toast.warning('请输入正确的邮箱或手机号码');
                    return;
                }

                if (this.VCodeTip == "发送验证码") {
                    let phoneValid = this.$refs.Phone.checkValidity();
                    let phoneOrMail = phoneValid ? this.Phone : this.Mail;
                    let respone = await RequestVCode({
                        phoneOrMail
                    });
                    if (respone != undefined && respone.Status == 200) {
                        this.Countdown();
                    }
                } else {
                    this.$toast.info("验证码已发送，请耐心等候");
                }
            },
            async Countdown() {
                if (this.VCodeTip == "发送验证码") {
                    this.VCodeTip = 60;
                    this.Countdown();
                } else {
                    if (this.VCodeTip > 0) {
                        let that = this;
                        setTimeout(() => {
                            that.VCodeTip--;
                            that.Countdown()
                        }, 1000);
                    } else {
                        this.VCodeTip = "发送验证码";
                    }
                }
            },
            async RequestRegisterUser() {
                this.IsSumbit = true;

                if (this.CheckPhoneOrMail() == false ||
                    this.$refs.PassWord.checkValidity() == false ||
                    this.$refs.VCode.checkValidity() == false) {
                    return;
                }

                let content = {
                    "email": this.Mail,
                    "phone": this.Phone,
                    "password": this.PassWord,
                    "vCode": this.VCode
                }

                let resopne = (await RegisterUser(content));
                if (resopne != undefined && resopne.Status == 200) {
                    let that = this;
                    this.$toast.info("将在3秒后跳转至登陆页面");
                    setTimeout(() => {
                        that.$router.push("/view/login");
                    }, 3000);
                }
            },
            CheckPhoneOrMail() {
                if (this.PhoneInVailTip == '' && this.MailInVailTip == '') {
                    let phoneValid = this.$refs.Phone.checkValidity();
                    let mailValid = this.$refs.Mail.checkValidity();
                    if (!phoneValid && !mailValid) {
                        this.$toast.warning("请输入正确的手机或邮箱");
                        return false;
                    }
                    return true;
                } else {
                    return false;
                }
            }
        },
        mounted() {
            this.GetVerifyRegex();
        },
    }
</script>


<style scoped>
    input {
        box-sizing: border-box;
        padding: 15px;
        height: auto;
        margin-bottom: 10px;
        height: 2.5rem;
    }
</style>