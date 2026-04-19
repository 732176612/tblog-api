<!--
 * @Author: your name
 * @Date: 2021-10-16 15:31:30
 * @LastEditTime: 2022-02-11 20:02:47
 * @LastEditors: Please set LastEditors
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\Login.vue
-->
<template>
  <div class="container-fluid h-100">
    <div class="row align-items-center justify-content-center" style="height:90vh">
      <div class="col-xl-4 col-lg-4 col-md-6 col-sm-8 col-10">
        <div class="card card-outline card-primary">
          <div class="card-header text-center" style="border-top: 2px solid var(--blue)">
            <img class="logoImg" src="../assets/img/logo.png" />
          </div>
          <div class="card-body">
            <p class="login-box-msg h3 text-center">找回密码</p>

            <form>
              <div class="input-group mb-3" :class="PhoneOrMail.length!=0?'was-validated':''">
                <input ref="PhoneOrMail" type="text" class="form-control" placeholder="账号" autocomplete required
                  v-model="PhoneOrMail" :readonly="IsSumbit?'readonly':false">
              </div>

              <div v-show="IsSumbit" class="input-group mb-3" :class="IsSumbit||VCode.length!=0?'was-validated':''">
                <input ref="VCode" type="text" class="form-control" placeholder="验证码" pattern="\d{4}$" v-model="VCode"
                  required>
                <div class="invalid-feedback">
                  请输入正确的验证码
                </div>
              </div>

              <div v-show="IsSumbit" class="input-group mb-3" :class="PassWord.length!=0?'was-validated':''">
                <input ref="PassWord" type="password" @keyup.enter="OnClickLoginUser" class="form-control"
                  placeholder="新密码" :pattern="PassWordRegex" v-model="PassWord" required>
                <div class="invalid-feedback">
                  要同时含有数字和字母，且长度要在8-16位之间
                </div>
              </div>

              <div class="d-grid gap-2 social-auth-links text-center mt-2 mb-4">
                <loadingbtn v-show="!IsSumbit" class="btn-block btn-main" :awaitAction="OnClickRequestRecoverPwd"
                  :btnText="'找回密码'" style="height:3rem">
                </loadingbtn>
                <loadingbtn v-show="IsSumbit" class="btn-block btn-main" :awaitAction="OnClickResponeRecoverPwd"
                  :btnText="'重设密码'" style="height:3rem">
                </loadingbtn>
              </div>
            </form>

            <p class="mb-0" style="font-size:1rem">
              <a href="/view/login" class="text-center">登陆</a>
            </p>
          </div>
        </div>
        <div class="mt-2 text-center" style="height:10vh">
          <a href="http://beian.miit.gov.cn" target="_blank" class="h6 mt-5 text-black-50">©TBlog - 粤ICP备20006712号。</a>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import {
    VerifyRegex,
    RequestRecoverPwd,
    ResponeRecoverPwd,
  } from '../assets/js/interface.js'
  export default {
    name: "RecoverPwd",
    data() {
      return {
        PhoneOrMail: "",
        PassWord: "",
        PassWordRegex: "",
        IsSumbit: false,
        VCode: ""
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
      async OnClickRequestRecoverPwd() {
        if (this.PhoneOrMail == "") {
          this.$toast.warning('账号不能为空');
          return;
        }
        var respone = await RequestRecoverPwd({
          "phoneOrMail": this.PhoneOrMail
        });
        console.log(respone);
        if (respone != null && respone.Status == 200) {
          this.IsSumbit = true;
        }
      },
      async OnClickResponeRecoverPwd() {
        if (this.$refs.PassWord.checkValidity() == false) {
          this.$toast.info("请输入格式正确的密码");
          return;
        }

        if (this.$refs.VCode.checkValidity() == false) {
          this.$toast.info("请输入正确的验证码");
          return;
        }

        var respone = await ResponeRecoverPwd({
          "vcode": this.VCode,
          "phoneOrMail": this.PhoneOrMail,
          "password": this.PassWord
        })

        console.log(respone);

        if (respone.Status == 200) {
          let that = this;
          this.$toast.success("重设成功，3秒后将跳转到登录页面");
          setTimeout(() => {
            that.$router.push("/view/login");
          }, 3000);
        }
      }
    },
    mounted() {
      this.GetVerifyRegex();
    }
  }
</script>

<style scoped>
  input {
    box-sizing: border-box;
    padding: 15px;
    height: auto;
    margin-bottom: 10px;
    margin-top: 10px;
    height: 3rem;
  }
</style>