<!--
 * @Author: your name
 * @Date: 2022-01-23 15:57:56
 * @LastEditTime: 2022-07-02 15:22:43
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\UserInfo.vue
-->
<template>
  <div class="d-flex flex__container mx-2 pb-3" style="padding-top:80px">
    <div v-show="$route.params.blogname != undefined" class="nav nav-pills nav-pills-main py-3" style="flex-wrap:nowrap"
      id="v-pills-tab" role="tablist" aria-orientation="vertical">
      <button class="nav-link nav-link-main active" id="v-pills-home-tab" data-bs-toggle="pill"
        data-bs-target="#v-pills-home" type="button" role="tab" aria-controls="v-pills-home"
        aria-selected="true">个人信息</button>
      <button class="nav-link nav-link-main" id="v-pills-edu-tab" data-bs-toggle="pill" data-bs-target="#v-pills-edu"
        type="button" role="tab" aria-controls="v-pills-edu" aria-selected="false">教育经历</button>
      <button class="nav-link nav-link-main" id="v-pills-skill-tab" data-bs-toggle="pill"
        data-bs-target="#v-pills-skill" type="button" role="tab" aria-controls="v-pills-skill"
        aria-selected="false">专业技能</button>
      <button class="nav-link nav-link-main" id="v-pills-work-tab" data-bs-toggle="pill" data-bs-target="#v-pills-work"
        type="button" role="tab" aria-controls="v-pills-work" aria-selected="false">工作经历</button>
      <button class="nav-link nav-link-main" id="v-pills-work-tab" data-bs-toggle="pill"
        data-bs-target="#v-pills-project" type="button" role="tab" aria-controls="v-pills-project"
        aria-selected="false">项目经历</button>
    </div>
    <div class="tab-content flex__container w-100" id="v-pills-tabContent">
      <div class="tab-pane show active col-10" id="v-pills-home" role="tabpanel" aria-labelledby="v-pills-home-tab">
        <h3 class="text-center mt-3" v-show="BlogName.length == 0">欢迎来到TBlog,请补充你的个人信息</h3>
        <div class="card">
          <div class="card-header bg-white">
            <div style="font-size:1.25em;font-weight: 700;">个人信息</div>
          </div>
          <div class="card-body pt-0">
            <div class="d-flex flex-column my-3">
              <label class="form-label text-center">我的头像</label>
              <ImageUpload field="img" @crop-success="cropSuccess" v-model="show" :width="300" :height="300"
                img-format="png"></ImageUpload>
              <img ref="ViewHeadImg" class="userHeadImg rounded-circle" :src="UserHeadImg" alt="选择头像"
                @click="toggleShow" />
            </div>
            <form class="needs-validation">
              <div class="row">
                <div class="col  my-1">
                  <label class="form-label">博客名称</label>
                  <input ref="BlogName" type="text" class="form-control"
                    :class="BlogName.length != 0 ? BlogNameInVailTip == '' ? 'is-valid' : 'is-invalid' : ''"
                    :pattern="BlogNameRegex" @input="OnChangeCheckHaveBlogName" placeholder="(保存后不可修改)"
                    v-model="BlogName" required>
                  <div class="invalid-feedback">
                    {{ BlogNameInVailTip }}
                  </div>
                </div>
                <div class="col  my-1">
                  <label class="form-label">姓名</label>
                  <input ref="UserName" type="text" class="form-control" v-model="UserName"
                    :class="IsSumbitUserInfo || UserName.length != 0 ? 'was-validated' : ''" pattern="^.{1,20}$" required>
                  <div class="invalid-feedback">
                    不能为空,且长度不能超过20个字符
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col my-2">
                  <label class="form-label">性别</label>
                  <div class="form-control d-flex">
                    <div class="form-check form-check-inline">
                      <input class="form-check-input" type="radio" name="SexRadio" id="man" value="1" v-model="Sex">
                      <label class="form-check-label" for="man">男</label>
                    </div>
                    <div class="form-check form-check-inline">
                      <input class="form-check-input" type="radio" name="SexRadio" id="women" value="2" v-model="Sex">
                      <label class="form-check-label" for="women">女</label>
                    </div>
                  </div>
                </div>

                <div class="col  my-2">
                  <label class="form-label">生日</label>
                  <input type="date" class="form-control" v-model="Birthday">
                </div>
              </div>

              <div class="row my-2">
                <div class="col">
                  <label class="form-label">签名</label>
                  <textarea ref="Sign" type="text" class="form-control " placeholder="签名"
                    :class="Sign || Sign.length != 0 ? 'was-validated' : ''" pattern="^.{0,40}$" v-model="Sign"></textarea>
                  <div class="invalid-feedback">
                    长度不能超过40个字符
                  </div>
                </div>
              </div>

              <div class="row my-2">
                <div class="col">
                  <label class="form-label">个人介绍</label>
                  <textarea ref="Introduction" type="text" class="form-control " placeholder="个人介绍"
                    :class="IsSumbitUserInfo || Introduction.length != 0 ? 'was-validated' : ''" pattern="^.{0,140}$"
                    v-model="Introduction"></textarea>
                  <div class="invalid-feedback">
                    长度不能超过140个字符
                  </div>
                </div>
              </div>

              <div class="row my-2">
                <div class="col">
                  <label class="form-label">上传简历</label>
                  <div>
                    <a :href="ResumeUrl" target="_blank">{{ ResumeName }}</a>
                  </div>
                  <div class="input-group mt-1 mb-3">
                    <input ref="ResumeFile" type="file" class="form-control" @change="OnResumeFileChage">
                  </div>
                </div>
              </div>


              <div class="row my-1">
                <div class="col">
                  <label class="form-label">主题颜色</label>
                  <div class="input mb-3">
                    <input type="color" ref="StyleColor" class="form-control form-control-color" v-model="StyleColor"
                      title="Choose your color" id="colorPicker">
                  </div>
                </div>
              </div>



              <div class="row my-1">
                <div class="col">
                  <label class="form-label">上传背景图片</label>
                  <div>
                    <img v-show="BackgroundUrl != ''" class="backgroundImg" :src="BackgroundUrl" alt="上传背景图片" />
                  </div>
                  <div class="input mt-1 mb-2">
                    <input class="w-100" type="file" ref="BackgroundImg" @change="OnBackgroundImageChage" />
                  </div>
                </div>
              </div>

              <div class="d-grid mt-3">
                <loadingbtn class="btn-block btn-main" :awaitAction="OnClickOpenBlog"
                  :btnText="BlogName.length == 0 ? '开通博客' : '保存'">
                </loadingbtn>
              </div>
            </form>
          </div>
        </div>
      </div>
      <div class="tab-pane fade col-10" id="v-pills-edu" role="tabpanel" aria-labelledby="v-pills-edu-tab">
        <EduInfo></EduInfo>
      </div>
      <div class="tab-pane fade col-10" id="v-pills-skill" role="tabpanel" aria-labelledby="v-pills-skill-tab">
        <SkillInfo></SkillInfo>
      </div>
      <div class="tab-pane fade col-10" id="v-pills-work" role="tabpanel" aria-labelledby="v-pills-work-tab">
        <CompanyInfo></CompanyInfo>
      </div>
      <div class="tab-pane fade col-10" id="v-pills-project" role="tabpanel" aria-labelledby="v-pills-project-tab">
        <Projectinfo></Projectinfo>
      </div>
    </div>
  </div>
</template>

<script>
import {
  VerifyRegex,
  CheckHaveBlogName,
  SaveUserInfo,
  UpLoadImgByFile,
  UpLoadResumeByFile,
  GetUserInfo,
  UpLoadImgByBase64
} from '../assets/js/interface.js';
import Projectinfo from './ProjectInfo.vue'
import CompanyInfo from './CompanyInfo.vue'
import EduInfo from './EduInfo.vue'
import SkillInfo from './SkillInfo.vue'
export default {
  name: "UserInfo",
  components: {
    Projectinfo,
    CompanyInfo,
    EduInfo,
    SkillInfo
  },
  data() {
    return {
      UserHeadImg: 'https://tblog-1300954268.cos.ap-guangzhou.myqcloud.com/Logo.png',
      Birthday: "",
      BlogName: "",
      BlogNameRegex: "",
      BlogNameInVailTip: "",
      UserName: "",
      Introduction: "",
      Sex: "1",
      IsSumbitUserInfo: false,
      Sign: "",
      ResumeUrl: "",
      ResumeName: "",
      StyleColor: '#0077ff',
      BackgroundUrl: '',
      show: false,
      params: {
        token: '123456798',
        name: 'avatar'
      },
      headers: {
        smail: '*_~'
      },
    }
  },
  methods: {
    toggleShow() {
      this.show = !this.show;
    },
    cropSuccess(imgDataUrl, field) {
      this.UserHeadImg = imgDataUrl;
    },
    async GetVerifyRegex() {
      let regexs = (await VerifyRegex({
        regexName: "BlogName"
      })).Data;

      for (var regex in regexs) {
        if (regexs[regex].Key == 'BlogName') {
          this.BlogNameRegex = regexs[regex].Value;
        }
      }
    },
    OnBackgroundImageChage() {
      if (this.$refs.BackgroundImg.files.length >= 1) {
        let file = this.$refs.BackgroundImg.files[0];
        if (file.size > 2 * 1024 * 1024) {
          this.$toast.warning("图片大小不能超过2MB");
          this.$refs.BackgroundImg.value = '';
          return;
        }
        let imgUrl = getObjectURL(file);
        this.BackgroundUrl = imgUrl;
      }
    },
    OnResumeFileChage() {
      if (this.$refs.ResumeFile.files.length >= 1) {
        let file = this.$refs.ResumeFile.files[0];
        if (file.size > 2 * 1024 * 1024) {
          this.$toast.warning("简历大小不能超过2MB");
          this.$refs.ResumeFile.value = '';
        }
      }
    },
    async OnClickOpenBlog() {
      if (this.BlogNameInVailTip != "" || !this.$refs.UserName.checkValidity() || !this.$refs.Introduction
        .checkValidity() || !this.$refs.Sign.checkValidity()) {
        this.$toast.error("个人信息填报有误");
        return;
      }

      if (this.UserHeadImg.indexOf('base64') != -1) {
        let respone = await UpLoadImgByBase64('headimg', this.UserHeadImg);
        if (respone == null || respone.Status == 500) {
          this.$toast.error("头像上传失败");
          return;
        } else {
          this.UserHeadImg = respone.Data;
        }
      }

      if (this.$refs.BackgroundImg.files.length != 0) {
        let respone = await UpLoadImgByFile('backimg', this.$refs.BackgroundImg.files[0]);
        if (respone == null || respone.Status == 500) {
          this.$toast.error("壁纸上传失败");
          this.BackgroundUrl = '';
          return;
        } else {
          this.BackgroundUrl = respone.Data;
        }
      }

      if (this.$refs.ResumeFile.files.length != 0) {
        let respone = await UpLoadResumeByFile('file', this.$refs.ResumeFile.files[0]);
        if (respone == null || respone.Status == 500) {
          this.$toast.error("简历上传失败");
          return;
        } else {
          this.ResumeUrl = respone.Data;
          this.ResumeName = this.ResumeUrl.substring(this.ResumeUrl.lastIndexOf("/") + 1, this.ResumeUrl
            .lastIndexOf("."));
        }
      }
      let respone = await SaveUserInfo({
        Birthday: this.Birthday,
        BlogName: this.BlogName,
        UserName: this.UserName,
        Introduction: this.Introduction,
        Sex: this.Sex,
        HeadImgUrl: this.UserHeadImg,
        Sign: this.Sign,
        ResumeUrl: this.ResumeUrl,
        ResumeName: this.ResumeName,
        BackgroundUrl: this.BackgroundUrl,
        StyleColor: this.StyleColor
      });
      if (respone != null && respone.Status == 200) {
        if (this.$route.params.blogname == undefined)
          this.$router.push("/view/index/" + this.BlogName);
        else {
          this.$toast.success("保存成功!");
        }
      }
    },
    async OnChangeCheckHaveBlogName() {
      if (this.BlogName.length != 0) {
        if (this.$refs.BlogName.checkValidity()) {
          var respone = await CheckHaveBlogName({
            'BlogName': this.BlogName
          });
          if (respone != null && respone.Status == 500) {
            this.BlogNameInVailTip = '该博客名称已被注册';
          } else {
            this.BlogNameInVailTip = '';
          }
        } else {
          this.BlogNameInVailTip = '博客名称格式不正确';
        }
      } else {
        this.BlogNameInVailTip = '';
      }
    }
  },
  async mounted() {
    this.Birthday = this.$dayjs().format('YYYY-MM-DD');
    if (this.$route.params.blogname != undefined && this.Config.token != '') {
      this.BlogName = this.$route.params.blogname;
      this.$refs.BlogName.readOnly = 'readonly';
      let respone = await GetUserInfo();
      if (respone != null && respone.Status == 200) {
        let userDto = respone.Data;
        this.UserName = userDto.UserName;
        this.Sex = userDto.Sex;
        this.Birthday = userDto.Birthday;
        this.Introduction = userDto.Introduction;
        this.Sign = userDto.Sign;
        if (userDto.HeadImgUrl != '')
          this.UserHeadImg = userDto.HeadImgUrl;
        this.ResumeUrl = userDto.ResumeUrl;
        this.ResumeName = userDto.ResumeName;
        this.BackgroundUrl = userDto.BackgroundUrl;
        if (userDto.StyleColor != '')
          this.StyleColor = userDto.StyleColor;
      }
    }
    await this.GetVerifyRegex();
    document.getElementById('colorPicker').addEventListener('input', () => {
      ChangeStyleColor(this.StyleColor);
    });
    let box = document.getElementsByClassName('tab-content')[0],
      config = {
        attributes: true,
        attributeFilter: ['class'],
        subtree: true
      };
    let observer = new MutationObserver(mutations => {
      this.$nextTick(() => {
        AutoExtendTextArea();
      })
    })
    observer.observe(box, config);
    window.onbeforeunload = function () {
      return confirm("您的文章未保存，确定离开吗？");
    }
  }
}
</script>
<style>
.userHeadImg {
  border: 5px solid var(--main_light_color);
  margin: 0 auto;
  height: 120px;
  width: 120px;
}

.backgroundImg {
  margin: 0 auto;
  max-height: 300px;
  width: auto;
  max-width: 100%;
}

.userHeadImg:hover {
  animation: selectHeadImg 2s infinite;
  -webkit-animation: selectHeadImg 5s infinite;
  cursor: cell;
}

::-webkit-input-placeholder {
  color: #476166;
  font-size: 1rem;
}

.textStyle {
  color: #476166;
  font-size: 1rem;
}

@keyframes selectHeadImg {
  0% {
    border-color: var(--main_dark_color);
  }

  66% {
    border-color: var(--main_light_color);
  }

  100% {
    border-color: var(--main_dark_color);
  }
}

.slider-selection {
  background: var(--main_color);
}

.slider-handle {
  background: white;
  box-shadow: 0px 0px 5px 1px #00000052;
}

.tagTitle {
  font-size: 1.25em;
  font-weight: 700;
}

.inputTitle {
  border-left: 5px solid var(--main_color);
  padding-left: 5px;
}

.nav-link-main {
  color: var(--main_color) !important;
}

.nav-link-main:hover,
.nav-link-main:focus {
  color: var(--main_dark_color) !important;
}

.nav-pills-main .nav-link.active,
.nav-pills-main .show>.nav-link {
  color: #fff !important;
  background-color: var(--main_color) !important;
}

.flex__container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}
</style>