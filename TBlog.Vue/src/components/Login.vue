<!--
 * @Author: your name
 * @Date: 2021-10-16 15:31:30
 * @LastEditTime: 2022-06-01 19:17:00
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\Login.vue
-->
<template>
  <div class="d-flex align-items-center justify-content-center" style="height:90%">
    <div class="col-xl-3 col-lg-4 col-md-6 col-sm-10 col-10" style="max-width:400px">
      <div class="card" style="border-top: 2px solid var(--blue)">
        <div class="card-header text-center">
          <img class="logoImg" src="../assets/img/logo.png" />
        </div>
        <div class="card-body">
          <p class="h3 text-center">登录</p>
          <form>
            <div class="input-group mb-3" :class="PhoneOrMail.length!=0?'was-validated':''">
              <input ref="PhoneOrMail" type="text" class="form-control text-input" placeholder="账号" autocomplete
                required v-model="PhoneOrMail">
            </div>
            <div class="input-group mb-3" :class="PassWord.length!=0?'was-validated':''">
              <input ref="PassWord" type="password" @keyup.enter="OnClickLoginUser" class="form-control text-input"
                placeholder="密码" :pattern="PassWordRegex" v-model="PassWord" autocomplete required>
              <div class="invalid-feedback">
                要同时含有数字和字母，且长度要在8-16位之间
              </div>
            </div>

            <div class="d-grid gap-2 mt-2 mb-2">
              <loadingbtn class="btn-block btn-primary" :awaitAction="OnClickLoginUser" :btnText="'登录'"
                style="height:3rem">
              </loadingbtn>
            </div>

            <div class="row">
              <div class="col-4 offset-8 text-end">
                <input type="checkbox" id="AutoLogin" v-model="AutoLogin" />
                <label for="AutoLogin">自动登录</label>
              </div>
            </div>
          </form>

          <p class="mb-1" style="font-size:1rem">
            <a href="/view/RecoverPwd">忘记密码</a>
          </p>
          <p class="mb-0" style="font-size:1rem">
            <a href="/view/register" class="text-center">注册新账号</a>
          </p>
        </div>
      </div>
    </div>
  </div>
  <div class="text-center" style="height:10%">
    <a href="http://beian.miit.gov.cn" target="_blank" class="h6 mt-5 text-black-50">©TBlog - 粤ICP备20006712号。</a>
  </div>
</template>

<script>
  import {
    VerifyRegex,
    LoginUser,
    SerializeJwt
  } from '../assets/js/interface.js'
  export default {
    name: "Login",
    data() {
      return {
        PhoneOrMail: "",
        PassWord: "",
        PassWordRegex: "",
        AutoLogin: true
      }
    },
    methods: {
      async GetVerifyRegex() {
        let regexs = (await VerifyRegex({
          regexName: "PassWord"
        })).Data;

        for (var regex in regexs) {
          if (regexs[regex].Key == 'PassWord') {
            this.PassWordRegex = regexs[regex].Value;
          }
        }
      },
      async OnClickLoginUser() {
        if (this.PhoneOrMail == "") {
          this.$toast.warning('账号不能为空');
          return;
        }
        if (!this.$refs.PassWord.checkValidity()) {
          this.$toast.warning('密码格式有误，请检查');
          return;
        }

        let respone = await LoginUser({
          'PassWord': this.PassWord,
          'PhoneOrMail': this.PhoneOrMail
        });

        if (respone != null && respone.Status == 200) {
          this.$cookie.set("PhoneOrMail", this.PhoneOrMail);
          this.$cookie.set('AutoLogin', this.AutoLogin);
          this.Config.token=this.$cookie.get('token');
          if (respone.Data == '') {
            this.$router.push("/view/userinfo");
          } else {
            this.$router.push("/view/index/" + respone.Data);
          }
        }
      },
      async CheckAutoLogin() {
        let autoLogin = this.$cookie.get('AutoLogin');
        if (autoLogin != undefined) {
          this.AutoLogin = autoLogin;
          if (autoLogin==true) {
            console.log("自动登录！");
            let token = this.$cookie.get('token');
            if (token != undefined) {
              let respone = await SerializeJwt({
                token
              });
              if (respone.Data.BlogName != null && respone.Data.BlogName != '') {
                this.$router.push("/view/index/" + respone.Data.BlogName);
              } else {
                this.$router.push("/view/userinfo");
              }
            }
          }
        }
      }
    },
    async mounted() {
      await this.CheckAutoLogin();

      let phoneOrMail = this.$cookie.get('PhoneOrMail');
      if (phoneOrMail != undefined) {
        this.PhoneOrMail = phoneOrMail;
      }
      this.GetVerifyRegex();
    }
  }
</script>

<style scoped>
  .login-page {
    align-items: center;
    display: flex;
    flex-direction: column;
    height: 100vh;
    justify-content: center;
  }

  .text-input {
    box-sizing: border-box;
    padding: 15px;
    height: auto;
    margin-bottom: 10px;
    margin-top: 10px;
    height: 3rem;
  }
</style>