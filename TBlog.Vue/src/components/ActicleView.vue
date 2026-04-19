<!--
 * @Author: your name
 * @Date: 2022-01-23 15:57:56
 * @LastEditTime: 2022-06-12 15:09:47
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\UserInfo.vue
-->
<template>
    <div class="row justify-content-center" style="padding-top:80px;margin: 0;">
        <div class="col-xl-10 col-lg-10 col-md-11 col-sm-12" style="padding: 5px;">
            <div class="main rounded bg-white mx-xl-5 mx-3 px-xl-5 px-md-3 px-2 py-4">
                <div class="title">{{ Title }}</div>
                <div class="header d-flex justify-content-between flex-wrap my-4">
                    <div class="header-right d-flex">
                        <img class="header-img rounded-circle mr-2" :src="UserHeadImg" />
                        <div class="header-content align-self-center">
                            <div class="header-name">{{ UserName }}</div>
                            <div class="header-info">{{ ActicleCDate }}</div>
                            <div class="header-info">阅读 {{ LookNums }} · 点赞 {{ LikeNums }}</div>
                        </div>
                    </div>
                    <!-- <button type="button" class="btn btn-outline-primary btn-sm align-self-center" style="height:40px">
                        <i class="bi bi-plus"></i>
                        关注
                    </button> -->
                </div>
                <div class="content" v-html="Content"></div>
                <div class="mescroll-upwarp mescroll-hardware" style="visibility: visible; display: block;">
                    <span class="upwarp-nodata">-- END --</span>
                </div>
                
                <!-- 评论区域 -->
                <CommentSection :acticle-id="$route.query.id" />
                <div class="footer d-flex justify-content-center">
                    <button type="button" class="btn btn-outline-primary position-relative mx-3"
                        @click="OnClickLikeButton" :disabled="isSelf($route) ? 'disabled' : false">
                        <i class="bi bi-hand-thumbs-up"></i>
                        <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-primary">
                            {{ LikeNums }}+
                            <span class="visually-hidden">unread messages</span>
                        </span>
                    </button>

                    <button type="button" class="btn btn-outline-success mx-3" v-show="isSelf($route)"
                        @click="OnClickEditorButton">
                        <i class="bi bi-pencil-square"></i>
                    </button>

                    <button type="button" class="btn btn-outline-danger mx-3" v-show="isSelf($route)"
                        data-bs-toggle="modal" data-bs-target="#deleteModal">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </div>

            <!-- 添加标签弹框 -->
            <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="TagModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="TagModalLabel">删除文章</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"
                                data-bs-target="#deleteModal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <h4>确定要删除吗?</h4>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger" @click="OnClickDeleteButton">删除</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import {
    GetActicle,
    GetUserInfo,
    LikeArticle,
    LookArticle,
    DeleteArticle
} from '../assets/js/interface.js';
import {
    Modal
} from 'bootstrap/dist/js/bootstrap.bundle';
import CommentSection from './CommentSection.vue';
export default {
    name: "ArtcleEditor",
    components: {
        CommentSection
    },
    data() {
        return {
            Title: "",
            Content: "",
            PosterImg: "",
            ActicleReleaseForm: "1",
            ActicleType: "1",
            ActicleTags: [],
            ActicleCDate: "",
            UserHeadImg: "",
            UserName: "",
            LikeNums: 0,
            LookNums: 0,
            CBlogName: ""
        }
    },
    methods: {
        async InitActicle() {
            if (this.$route.query.id != undefined) {
                let respone = await GetActicle(this.$route.query.id);
                if (respone.Status == 200) {
                    this.Title = respone.Data.Title;
                    this.ActicleTags = [...respone.Data.Tags];
                    this.PosterImg = respone.Data.PosterUrl;
                    this.ActicleReleaseForm = respone.Data.ReleaseForm;
                    this.ActicleType = respone.Data.ActicleType;
                    this.Content = respone.Data.Content;
                    this.ActicleCDate = respone.Data.CDate;
                    this.LikeNums = respone.Data.LikeNum;
                    this.LookNums = respone.Data.LookNum;
                    this.CBlogName = respone.Data.CBlogName;
                    document.title = this.Title;
                    this.$nextTick(() => {
                        const images = document.querySelectorAll('.content img');
                        images.forEach(img => {
                            img.style.width = '100%';
                            img.style.height = 'auto';
                        });
                    });
                }
            }
        },
        async InitUserInfo() {
            let respone = await GetUserInfo(this.$route.params.blogname);
            if (respone != null && respone.Status == 200) {
                this.UserName = respone.Data.UserName;
                this.UserHeadImg = respone.Data.HeadImgUrl;
            }
        },
        async OnClickLikeButton() {
            let respone = await LikeArticle(this.$route.query.id);
            if (respone.Status == 200) {
                this.LikeNums++;
            }
        },
        OnClickEditorButton() {
            this.$router.push("/view/acticleEditor/" + this.$route.params.blogname + "?id=" + this.$route
                .query.id);
        },
        async OnClickDeleteButton() {
            Modal.getOrCreateInstance(document.getElementById('deleteModal')).hide();
            let respone = await DeleteArticle(this.$route.query.id);
            if (respone.Status == 200) {
                this.$toast.success("删除成功!");
                this.$router.push("/view/acticleList/" + this.$route.params.blogname);
            }
        }
    },
    async mounted() {
        if (this.$route.query.id == undefined) {
            this.$router.push("/view/index");
            return;
        }
        this.InitUserInfo();
        this.InitActicle();
        await LookArticle(this.$route.query.id);
    }
}
</script>

<style scoped>
.main {
    overflow-x: hidden;
}

.title {
    color: #1d1f25;
    font-size: 2.1rem;
    font-weight: 700;
}

.header {
    padding-top: 20px;
    padding-bottom: 20px;
    border-bottom: 1px solid #949494;
    border-top: 1px solid #949494;
}

.header-img {
    height: 60px;
    width: 60px;
    margin-right: 15px;
}

.header-name {
    color: #515767;
    font-size: 1.1rem;
}

.header-info {
    color: #777f97;
    font-size: 0.9rem;
}

.content {
    width: 100%;

    img {
        width: 100% !important;
        height: auto !important;
        max-width: 100% !important;
    }
}

.content img {
    width: 100% !important;
    height: auto !important;
    max-width: 100% !important;
}
</style>