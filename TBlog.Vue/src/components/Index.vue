<template>
  <div class="d-flex h-100 mx-auto flex-column">
    <header v-if="UserDto.BlogName!=''">
      <nav class="navbar navbar-expand-sm navbar-dark fixed-top" :class="NavClass" aria-label="Ninth navbar example">
        <div class="container-xl">
          <a class="navbar-brand" href="#">{{UserDto.BlogName}}</a>
          <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarsExample07XL" style="border: 3px solid white;"
            aria-controls="navbarsExample07XL" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
          </button>

          <div class="collapse navbar-collapse" id="navbarsExample07XL">
            <ul class="navbar-nav me-auto">
              <li class="nav-item" v-for="(item,index) in Menus" :key="index">
                <a class="nav-link" href="#" :class="$route.name.indexOf(item.Name)!=-1?'active':''"
                  @click="OnClickMenuBtn(item)">{{item.Name}}</a>
              </li>
              <li v-if="Config.token==''"><a class="nav-link" href="/view/login">登陆</a></li>
              <li v-if="Config.token!=''&&isSelf($route)==false"><a class="nav-link" :href="'/view/index/'+Config.userSelf.BlogName">回到我的博客</a></li>
              <li class="nav-item dropdown" v-if="Config.token!=''">
                <a class="nav-link dropdown-toggle" href="#" id="dropdown07XL" data-bs-toggle="dropdown"
                  aria-expanded="false">个人中心</a>
                <ul class="dropdown-menu" aria-labelledby="dropdown07XL" v-if="true">
                  <li><a class="dropdown-item" :href="'/view/acticleEditor/'+Config.userSelf.BlogName">写文章</a></li>
                  <li><a class="dropdown-item" :href="'/view/userInfo/'+Config.userSelf.BlogName">个人信息</a></li>
                  <li>
                    <hr class="dropdown-divider">
                  </li>
                  <li><a class="dropdown-item" @click="OnClickLogOut">注销</a></li>
                </ul>
              </li>
            </ul>
            <!-- <form role="search">
              <input class="form-control" type="search" placeholder="Search" aria-label="Search">
            </form> -->
          </div>
        </div>
      </nav>
    </header>

    <main role="main" id="main" style="height:auto;">
      <div class="h-100">
        <router-view />
      </div>
    </main>

    <footer class="mastfoot mt-auto border-top">
      <div class="inner text-center py-2">
        <a href="http://beian.miit.gov.cn" target="_blank" class="h6 mt-5 text-black-50">©TBlog -
          粤ICP备20006712号。</a>
      </div>
    </footer>
  </div>
</template>

<script>
  import {
    SerializeJwt,
    GetUserInfo,
    GetMenus,
    LogOut
  } from '../assets/js/interface.js';
  export default {
    name: "Index",
    data() {
      return {
        UserDto: {
          BlogName: "",
          UserName: "",
          IsInit: false,
        },
        Menus: [],
        NavClass: "bg-main"
      }
    },
    methods: {
      async InitMenus() {
        let respone = await GetMenus();
        if (respone != null && respone.Status == 200) {
          this.Menus = respone.Data;
        }
      },
      async InitUserInfo() {
        let respone = await GetUserInfo(this.$route.params.blogname);
        if (respone != null && respone.Status == 200) {
          let model = respone.Data;
          this.UserDto.UserName = model.UserName;
          this.UserDto.BlogName = model.BlogName;
          if (model.StyleColor != undefined && model.StyleColor != '') {
            ChangeStyleColor(model.StyleColor);
          }
        } else {
          this.$cookie.set('AutoLogin', 'false');
          this.$router.push("/view/login");
        }
      },
      OnClickLogOut() {
        LogOut();
        this.Config.token = '';
        this.$cookie.remove('token');
        this.$router.push("/view/login");
      },
      OnClickMenuBtn(item) {
        location.href = item.Url + "/" + this.$route.params.blogname;
      },
      async CheckToken() {
        this.Config.token = getCookie('token');
        if (this.$route.params.blogname == undefined) {
          if (this.Config.token == '') {
            this.$toast.warning('您还未登陆，请登陆');
            await this.$router.push("/view/login");
          } else {
            if (this.Config.userSelf.BlogName != null && this.Config.userSelf.BlogName != '') {
              await this.$router.push("/view/index/" + this.Config.userSelf.BlogName);
            } else {
              await this.$router.push("/view/userinfo");
            }
          }
        } else if (!this.IsInit) {
          await this.InitUserInfo();
          await this.InitMenus();
        }
      }
    },
    beforeRouteEnter(to, from, next) {
      next(async _ => {
        let that = _;
        await that.CheckToken();
        if (that.$route.path.indexOf("index") == -1 || window.scrollY > 400) {
          that.NavClass = 'bg-main';
        } else {
          that.NavClass = '';
        }
      })
    },
    mounted() {
      setInterval(() => {
        if (this.$route.path.indexOf("index") == -1 || window.scrollY > 400) {
          this.NavClass = 'bg-main';
        } else {
          this.NavClass = '';
        }
      }, 100);
    }
  }
</script>

<style scoped>
  .nav-masthead .nav-link {
    padding: .25rem 0;
    font-weight: 700;
    color: rgba(0, 0, 0, .5);
    background-color: transparent;
    border-bottom: .25rem solid transparent;
  }

  .nav-masthead .nav-link:hover,
  .nav-masthead .nav-link:focus {
    border-bottom-color: rgba(0, 0, 0, .25);
  }

  .nav-masthead .nav-link+.nav-link {
    margin-left: 1rem;
  }

  .nav-masthead .active {
    color: #212529;
    border-bottom-color: var(--blue);
  }

  .NavUserImg {
    border: 3px solid var(--blue);
    padding: 3px;
  }

  .navbar {
    -webkit-transition: all 0.5s cubic-bezier(0.685, 0.0473, 0.346, 1);
    transition: all 0.5s cubic-bezier(0.685, 0.0473, 0.346, 1);
  }
</style>