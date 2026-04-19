<template>
    <div class="page-content">
        <div class="profile-page">
            <div class="wrapper">
                <div class="page-header page-header-small" filter-color="green">
                    <div class="page-header-image" :style="'background-image: url('+(UserDto.BackgroundUrl)+')'"
                        data-parallax="true">
                    </div>
                    <div class="container">
                        <div class="content-center">
                            <div class="cc-profile-image" v-if="UserDto.HeadImgUrl!=''&&UserDto.HeadImgUrl!=undefined">
                                <a href="#"><img :class="IsLoadImg?'':'placeholder'" :onload="UserHeadOnLoad(this)"
                                        :src="UserDto.HeadImgUrl" alt="头像" /></a>
                            </div>
                            <div class="h2 title">{{UserDto.UserName}}</div>
                            <p class="category text-white">{{UserDto.Sign}}</p>
                            <a class="btn btn-main" v-if="UserDto.ResumeUrl!=''" :href="UserDto.ResumeUrl"
                                target="_blank">下载简历</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="section" id="about">
            <div class="container">
                <div class="card aos-init " data-aos="fade-up" data-aos-offset="10">
                    <div class="row">
                        <div class="col-lg-6 col-md-12">
                            <div class="card-body">
                                <div class="h4 mt-0 title">自我介绍</div>
                                <p>{{UserDto.Introduction}}</p>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-12">
                            <div class="card-body">
                                <div class="h4 mt-0 title">基本信息</div>
                                <div class="row mt-1" v-show="UserDto.Age!='0'">
                                    <div class="col-sm-4"><strong class="text-uppercase">年龄:</strong></div>
                                    <div class="col-sm-8">{{UserDto.Age}}</div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-sm-4"><strong class="text-uppercase">邮箱:</strong></div>
                                    <div class="col-sm-8">{{UserDto.Email}}</div>
                                </div>
                                <div class="row mt-2" v-show="UserDto.Phone!=''">
                                    <div class="col-sm-4"><strong class="text-uppercase">手机:</strong></div>
                                    <div class="col-sm-8">{{UserDto.Phone}}</div>
                                </div>
                                <div class="row mt-2" v-if="IsApartEdu">
                                    <div class="col-sm-4"><strong class="text-uppercase">毕业学校:</strong></div>
                                    <div class="col-sm-8">{{EduInfos[0].School}}</div>
                                </div>
                                <div class="row mt-2" v-if="IsApartEdu">
                                    <div class="col-sm-4"><strong class="text-uppercase">所读专业:</strong></div>
                                    <div class="col-sm-8">{{EduInfos[0].Major}}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="section" id="skill" v-show="SkillInfos.length!=0">
            <div class="container">
                <div class="h4 text-center mb-4 title">专业技能</div>
                <div class="card aos-init " data-aos="fade-up" data-aos-anchor-placement="top-bottom">
                    <div class="card-body">
                        <div class="row" v-for="i in Math.ceil(SkillInfos.length/2)" :key="i">
                            <div class="col-md-6" v-for="j in 2" :key="j">
                                <div v-if="(i-1)*2+(j-1)<SkillInfos.length" class="progress-container progress-primary">
                                    <span class="progress-badge">{{SkillInfos[(i-1)*2+(j-1)].Skill}}</span>
                                    <div class="progress bg-main-light">
                                        <div class="progress-bar bg-main aos-init" data-aos="progress-full"
                                            data-aos-offset="1" data-aos-duration="2000" role="progressbar"
                                            aria-valuenow="60" aria-valuemin="0" aria-valuemax="100"
                                            :style="'width: '+SkillInfos[(i-1)*2+(j-1)].Progress+'%;'"></div><span
                                            class="progress-value">{{SkillInfos[(i-1)*2+(j-1)].Progress}}%</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="section" id="experience" v-show="CompanyInfos.length!=0">
            <div class="container cc-experience">
                <div class="h4 text-center mb-4 title">工作经历</div>
                <div class="card" v-for="(item,index) in CompanyInfos" :key="index">
                    <div class="row">
                        <div class="col-md-3 bg-main aos-init" data-aos="fade-right" data-aos-offset="50"
                            data-aos-duration="500">
                            <div class="card-body cc-experience-header">
                                <p>{{item.Company}}</p>
                                <div class="h5">{{item.StartDate}} - {{item.EndDate}}</div>
                            </div>
                        </div>
                        <div class="col-md-9 aos-init" data-aos="fade-left" data-aos-offset="50"
                            data-aos-duration="500">
                            <div class="card-body">
                                <div class="h5">{{item.Position}}-<span class="category"
                                        style="font-size:1rem">{{item.Department}}</span>
                                </div>
                                <p>{{item.Introduction}}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="section" v-show="ProjectInfos.length!=0">
            <div class="container cc-experience">
                <div class="h4 text-center mb-4 title">项目经历</div>
                <div class="card" v-for="(item,index) in ProjectInfos" :key="index">
                    <div class="row">
                        <div class="col-md-3 bg-main aos-init" data-aos="fade-right" data-aos-offset="50"
                            data-aos-duration="500">
                            <div class="card-body cc-experience-header">
                                <div class="h5">{{item.Project}}</div>
                                <div style="font-size:1.25rem;padding-bottom:5px;">{{item.StartDate}} - {{item.EndDate}}
                                </div>
                                <div class="h5">
                                    {{item.Role}} - {{item.City}}
                                </div>
                            </div>
                        </div>
                        <div class="col-md-9 aos-init" data-aos="fade-left" data-aos-offset="50"
                            data-aos-duration="500">
                            <div class="card-body">
                                <p>{{item.Introduction}}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="section" v-show="EduInfos.length!=0&&!IsApartEdu">
            <div class="container cc-experience">
                <div class="h4 text-center mb-4 title">教育经历</div>
                <div class="card" v-for="(item,index) in EduInfos" :key="index">
                    <div class="row">
                        <div class="col-md-3 bg-main aos-init" data-aos="fade-right" data-aos-offset="50"
                            data-aos-duration="500">
                            <div class="card-body cc-experience-header">
                                <div class="h5">{{item.School}}</div>
                                <div style="font-size:1.25rem;padding-bottom:5px;">{{item.StartDate}} - {{item.EndDate}}
                                </div>
                                <div class="h5">
                                    {{item.Major}}
                                </div>
                            </div>
                        </div>
                        <div class="col-md-9 aos-init" data-aos="fade-left" data-aos-offset="50"
                            data-aos-duration="500">
                            <div class="card-body">
                                <p>{{item.Introduction}}</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="section" v-if="$route.params.blogname=='FalseEndLess'||$route.params.blogname=='TBlog'">
            <div class="container cc-experience">
                <div class="h4 text-center mb-4 title">关于TBlog</div>
                <div class="card">
                    <div class="row">
                        <div class="col-md-12 aos-init" data-aos="fade-left" data-aos-offset="50"
                            data-aos-duration="500">
                            <div class="card-body">
                                <p style="text-align:center">
                                    <b>谢谢你光临本博客</b>
                                    <br>
                                    如果你对本博客系统源代码感兴趣可以点下方链接进行查阅哦。
                                    <br>
                                    前端源代码:
                                    <a
                                        href=" https://github.com/732176612/tblog-vue">https://github.com/732176612/tblog-vue</a>
                                    <br>
                                    前端架构:Vue3.2.16+Vite2.6.4+Bootstrap5.1
                                    <br>
                                    后端源代码:
                                    <a
                                        href="https://github.com/732176612/tblog-api">https://github.com/732176612/tblog-api</a>
                                    <br>
                                    后端架构:.Net6.0+MongoDB5.0.3+Redis6.2.6
                                    <br>
                                    有任何疑问都可以加我QQ732176612进行交流。
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import {
        ref,
        onMounted
    } from 'vue'
    import {
        GetUserInfo,
        GetSkillInfo,
        GetCompanyInfo,
        GetProjectInfo,
        GetEduInfo
    } from '../assets/js/interface.js';
    import {
        useRoute,
        useRouter
    } from 'vue-router'
    import BackgroundUrl from '../assets/img/defaultbg.jpg'
    export default {
        props: ['UserDto', 'UserHeadOnLoad', 'IsLoadImg', 'SkillInfos', 'CompanyInfos', 'ProjectInfos', 'EduInfos',
            'IsApartEdu'
        ],
        setup(props) {
            const UserDto = ref({});
            const $route = useRoute()
            const $router = useRouter()
            const SkillInfos = ref([]);
            const CompanyInfos = ref([]);
            const ProjectInfos = ref([]);
            const IsLoadImg = ref(false);
            const EduInfos = ref([]);
            const IsApartEdu = ref(false);
            onMounted(async () => {
                let respone = await GetUserInfo($route.params.blogname);
                if (respone != null && respone.Status == 200) {
                    UserDto.value = respone.Data;
                    if (UserDto.value.HeadImgUrl == '')
                        IsLoadImg.value = false;
                    if (UserDto.value.BackgroundUrl == '') {
                        UserDto.value.BackgroundUrl = BackgroundUrl;
                    }
                } else {
                    this.$cookie.set('AutoLogin', 'false');
                    $router.push("/view/login");
                }

                respone = await GetSkillInfo($route.params.blogname);
                if (respone.Status == 200) {
                    SkillInfos.value = respone.Data;
                }

                respone = await GetCompanyInfo($route.params.blogname);
                if (respone.Status == 200) {
                    CompanyInfos.value = respone.Data;
                }

                respone = await GetProjectInfo($route.params.blogname);
                if (respone.Status == 200) {
                    ProjectInfos.value = respone.Data;
                }

                respone = await GetEduInfo($route.params.blogname);
                if (respone.Status == 200) {
                    EduInfos.value = respone.Data;
                    if (EduInfos.value.length == 1 && EduInfos.value[0].Introduction == '') {
                        IsApartEdu.value = true;
                    }
                }
            });

            const UserHeadOnLoad = function (img) {
                if (UserDto.value.HeadImgUrl != '') {
                    IsLoadImg.value = true;
                }
            }

            return {
                UserDto,
                UserHeadOnLoad,
                IsLoadImg,
                SkillInfos,
                CompanyInfos,
                ProjectInfos,
                EduInfos,
                IsApartEdu
            }
        }
    }
</script>

<style scoped>
    .progress-container {
        position: relative;
    }

    .progress-container+.progress-container,
    .progress-container~.progress-container {
        margin-top: 15px;
    }

    .progress-container .progress-badge {
        color: #000;
        font-size: 0.8571em;
    }

    .progress-container .progress {
        height: 1px;
        border-radius: 0;
        -webkit-box-shadow: none;
        box-shadow: none;
        background: rgba(222, 222, 222, 0.8);
        margin-top: 14px;
    }

    .progress-container .progress .progress-bar {
        -webkit-box-shadow: none;
        box-shadow: none;
        background-color: #000;
    }

    .progress-container .progress .progress-value {
        position: absolute;
        top: 2px;
        right: 0;
        color: #000;
        font-size: 0.8571em;
    }

    .progress-container {
        margin-bottom: 20px;
        font-size: 18px;
    }

    .progress-container .progress-bar {
        height: 5px;
        -webkit-transform: scaleX(0);
        transform: scaleX(0);
        -webkit-transition: -webkit-transform 2s ease-in-out;
        transition: -webkit-transform 2s ease-in-out;
        transition: transform 2s ease-in-out;
        transition: transform 2s ease-in-out, -webkit-transform 2s ease-in-out;
        -webkit-transform-origin: 0% 0%;
        transform-origin: 0% 0%;
    }

    .progress-container .progress {
        height: 5px;
        font-size: 18px;
    }

    .progress-container .aos-animate {
        -webkit-transform: scaleX(1);
        transform: scaleX(1);
    }

    .title {
        font-weight: 700;
    }

    .category {
        text-transform: uppercase;
        font-weight: 700;
        color: #9A9A9A;
    }

    .card {
        border: 0;
        border-radius: 0.1875rem;
        display: inline-block;
        position: relative;
        overflow: hidden;
        width: 100%;
        margin-bottom: 20px;
        -webkit-box-shadow: 0px 5px 25px 0px rgba(0, 0, 0, 0.2);
        box-shadow: 0px 5px 25px 0px rgba(0, 0, 0, 0.2);
    }

    .card .card-body {
        min-height: 170px;
    }

    .section {
        padding: 30px 0;
        position: relative;
        background: #FFFFFF;
    }

    .section .row+.category {
        margin-top: 15px;
    }

    .page-header {
        height: 100vh;
        max-height: 1050px;
        padding: 0;
        color: #FFFFFF;
        position: relative;
        background-position: center center;
        background-size: cover;
    }

    .page-header .page-header-image {
        position: absolute;
        background-size: cover;
        background-position: center center;
        width: 100%;
        height: 100%;
        z-index: -1;
        transform: translate3d(0px, 0px, 0px);
    }

    .page-header .container {
        height: 100%;
        z-index: 1;
        text-align: center;
        position: relative;
    }

    .page-header .container>.content-center {
        position: absolute;
        top: 50%;
        left: 50%;
        -webkit-transform: translate(-50%, -50%);
        transform: translate(-50%, -50%);
        text-align: center;
        padding: 0 15px;
        color: #FFFFFF;
        width: 100%;
        max-width: 880px;
    }

    .page-header .category,
    .page-header .description {
        color: rgba(255, 255, 255, 0.5);
    }

    .page-header.page-header-small {
        height: 60vh;
        max-height: 440px;
    }

    .page-header:after,
    .page-header:before {
        position: absolute;
        z-index: 0;
        width: 100%;
        height: 100%;
        display: block;
        left: 0;
        top: 0;
        content: "";
    }

    .page-header:before {
        background-color: rgba(0, 0, 0, 0.5);
    }

    .page-header .container {
        z-index: 2;
    }

    .page-header {
        background: rgba(44, 44, 44, 0.2);
        background: -webkit-gradient(linear, left bottom, left top, from(rgba(44, 44, 44, 0.2)), to(var(--main_dark_color)));
        background: linear-gradient(0deg, rgba(44, 44, 44, 0.2), var(--main_dark_color));
    }

    .page-header .btn {
        width: 140px;
    }

    /* Profile Image Style */
    @-webkit-keyframes pulsate {
        0% {
            -webkit-transform: scale(0.6, 0.6);
            transform: scale(0.6, 0.6);
            opacity: 0.0;
        }

        50% {
            opacity: 1.0;
        }

        100% {
            -webkit-transform: scale(1, 1);
            transform: scale(1, 1);
            opacity: 0.0;
        }
    }

    @keyframes pulsate {
        0% {
            -webkit-transform: scale(0.6, 0.6);
            transform: scale(0.6, 0.6);
            opacity: 0.0;
        }

        50% {
            opacity: 1.0;
        }

        100% {
            -webkit-transform: scale(1, 1);
            transform: scale(1, 1);
            opacity: 0.0;
        }
    }

    .cc-profile-image a {
        position: relative;
    }

    .cc-profile-image a:before {
        content: "";
        border: 15px solid var(--main_dark_color);
        border-radius: 50%;
        height: 180px;
        width: 180px;
        position: absolute;
        left: 0;
        -webkit-animation: pulsate 1.6s ease-out;
        animation: pulsate 1.6s ease-out;
        -webkit-animation-iteration-count: infinite;
        animation-iteration-count: infinite;
        opacity: 0.0;
        z-index: 99;
    }

    .cc-profile-image img {
        position: relative;
        border-radius: 50%;
        height: 180px;
        width: 180px;
        padding: 0;
        margin: 0;
        border: 15px solid transparent;
        z-index: 9999;
        -webkit-transition: all .3s ease-out;
        transition: all .3s ease-out;
    }

    .cc-profile-image a:hover img {
        -webkit-transform: scale(1.06, 1.06);
        transform: scale(1.06, 1.06);
    }

    .cc-profile-image a:hover:before {
        -webkit-animation: none;
        animation: none;
    }

    .cc-experience .cc-experience-header {
        padding-top: 40px;
        padding-left: 0.75rem;
        margin-right: 0;
        text-align: center;
        color: #fff;
        text-transform: uppercase;
    }

    p {
        white-space: pre-wrap;
    }
</style>