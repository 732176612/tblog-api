<!--
 * @Author: your name
 * @Date: 2022-01-23 15:57:56
 * @LastEditTime: 2022-09-20 20:42:59
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\UserInfo.vue
-->
<template>
    <div class="row justify-content-center" style="padding-bottom:4rem;padding-top:100px;width:100%;margin-left:0">
        <div class="col-11 col-xl-10 col-lg-10 col-md-8 col-sm-10 mb-2" v-show="ActicleList.length!=0">
            <div class="nav-scroller">
                <nav class="nav d-flex">
                    <div v-for="(item,index) in ActicleTags" :key="index"
                        class="tag rounded-pill mx-2 py-1 px-3 mb-2 text-center"
                        :style="(SelectActicleTags.indexOf(item)!= '-1'? 'background-color:var(--blue)':'')"
                        @click="OnClickActicleTag(item)">
                        {{item}}</div>
                </nav>
            </div>
        </div>

        <div class="col-xl-10 col-lg-10 col-md-11 col-sm-11 col-11" style="padding:0">
            <div class="card">
                <div class="card-header bg-white">
                    <div class="container-fluid pl-0" style="padding-left:0;padding-right:0;">
                        <div
                            class="d-flex flex-wrap align-items-center justify-content-center justify-content-lg-start">

                            <ul class="nav col-12 col-lg-auto me-lg-auto mb-2 justify-content-center mb-md-0">
                                <li v-for="(item,index) in SortTags" :key="index"
                                    class="nav-item px-2 sortTag border-right"
                                    :style="(SelectSortTag==item.Key?'Color:var(--main_color)':'')"
                                    @click="OnClickSortTag(item.Key)">
                                    {{item.Value}}</li>
                                <li v-show="isSelf($route)" class="nav-item px-2 sortTag border-right"
                                    :style="(ReleaseForm=='2'?'Color:var(--orange)':'')"
                                    @click="OnClickReleaseFormTag(2)">
                                    私密</li>
                                <li v-show="isSelf($route)" class="nav-item px-2 sortTag"
                                    :style="(ReleaseForm=='3'?'Color:var(--orange)':'')"
                                    @click="OnClickReleaseFormTag(3)">
                                    草稿</li>
                            </ul>

                            <form class="col-12 col-lg-auto mb-3 mb-lg-0 me-lg-3 mt-2" role="search"
                                @submit.prevent="InitActicleList">
                                <div class="input-group w-100">
                                    <input type="text" v-model="SearchVal" class="form-control" placeholder="搜索文章">
                                    <button type="button" class="btn btn-outline-secondary" @click="InitActicleList">
                                        <i class="bi bi-search"></i>
                                        <span class="visually-hidden">Button</span>
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
                <div class="card-body bg-white pt-0 px-0">
                    <div class="list">
                        <Mescroll ref="Mescroll" :upCallback="ScrollUpCallback" :pageSize=10 :pageIndex=0>
                            <div class="item pt-2  px-4" v-for="(acticle,index) in ActicleList" :key="index"
                                @click="OnClickActicleItem(acticle)">
                                <div class="item-header py-2">
                                    <ul class="nav">
                                        <li class="nav-item border-right" style="padding-right:0.5rem;color:#626262">
                                            {{acticle.CBlogName}}</li>
                                        <li class="nav-item border-right px-2">{{acticle.CDate}}</li>
                                        <li class="nav-item px-2">{{acticle.Tags.join('.')}}</li>
                                    </ul>
                                </div>
                                <div class="item-content pb-2 d-flex justify-content-between">
                                    <div class="content-main d-flex align-content-between flex-column">
                                        <div class="content-title" v-html="acticle.Title"></div>
                                        <div class="content my-2" v-html="acticle.Content"></div>
                                        <div class="content-end">
                                            <ul class="nav text-center">
                                                <li class="nav-item" style="padding-right:0.25rem">
                                                    <i class="bi bi-eye mr-1" style="padding-right:0.25rem"></i>
                                                    {{acticle.LookNum}}</li>
                                                <li class="nav-item px-2">
                                                    <i class="bi bi-hand-thumbs-up" style="padding-right:0.25rem"></i>
                                                    {{acticle.LikeNum}}</li>
                                            </ul>
                                        </div>
                                    </div>
                                    <img v-if="acticle.PosterUrl!=''" :src="acticle.PosterUrl" class="content-img" />
                                </div>
                            </div>
                        </Mescroll>
                    </div>
                    <div v-if="ActicleList.length==0&&EnterSearchVal==''&&ReleaseForm!=2&&ReleaseForm!=3"
                        class="ListEmptyTip">
                        <a v-show="isSelf($route)"
                            :href="'/view/acticleEditor/'+$route.params.blogname">--快来写你的第一篇文章吧！--</a>
                        <span v-show="isSelf($route)==false">该博主太懒了，一篇博客都没写呢~</span>
                    </div>

                    <div v-if="ActicleList.length==0&&EnterSearchVal!=''" class="ListEmptyTip">
                        <span>搜索结果为空</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import {
        GetTags,
        GetEnums,
        GetActicleList
    } from '../assets/js/interface.js';

    export default {
        name: "ActicleList",
        data() {
            return {
                ActicleTags: [],
                SelectActicleTags: [],
                SortTags: [],
                SelectSortTag: '-1',
                ActicleList: [],
                TotalPageIndex: 0,
                CurPageIndex: 0,
                ReleaseForm: '1',
                SearchVal: '',
                EnterSearchVal: ''
            }
        },
        beforeRouteEnter(to, from, next) {
            next(vm => {
                vm.$refs.Mescroll && vm.$refs.Mescroll.beforeRouteEnter()
            })
        },
        beforeRouteLeave(to, from, next) {
            this.$refs.Mescroll && this.$refs.Mescroll.beforeRouteLeave()
            next()
        },
        methods: {
            ScrollInit() {
                this.$refs.Mescroll.init();
                let that = this;
                window.addEventListener('scroll', function (e) {
                    let innerHeight = document.getElementById('main').clientHeight
                    let outerHeight = document.documentElement.clientHeight
                    let scrollTop = document.documentElement.scrollTop
                    // scrollTop在页面为滚动时为0，开始滚动后，慢慢增加，滚动到页面底部时，出现innerHeight <= (outerHeight + scrollTop)的情况，严格来讲，是接近底部。
                    if (outerHeight + scrollTop >= innerHeight) {
                        if (that.TotalPageIndex > that.CurPageIndex) {
                            that.$refs.Mescroll.mescroll.triggerUpScroll();
                        }
                    }
                });
            },
            async ScrollUpCallback(page, mescroll) {
                let arr = await this.RequestGetActicleList(page.size, page.num);
                if (page.num === 1) this.ActicleList = []
                this.ActicleList = this.ActicleList.concat(arr)
                this.$nextTick(() => {
                    mescroll.endSuccess(arr.length)
                })
            },
            RequestGetTags: async function () {
                let respone = await GetTags(this.$route.params.blogname, this.ReleaseForm);
                this.ActicleTags = respone.Data;
                this.SelectActicleTags = [];
            },
            RequestGetEnums: async function () {
                let respone = await GetEnums('EnumActicleSortTag');
                if (respone.Data.length == 1) {
                    this.SortTags = respone.Data[0].EnumKeyValues;
                    if (this.SortTags.length >= 1) {
                        this.SelectSortTag = this.SortTags[0].Key;
                    }
                }
            },
            RequestGetActicleList: async function (pageSize, pageIndex) {
                this.EnterSearchVal = this.SearchVal;
                let respone = await GetActicleList({
                    pageSize: pageSize,
                    pageIndex: pageIndex,
                    blogName: this.$route.params.blogname,
                    releaseForm: this.ReleaseForm,
                    acticleSortTag: this.SelectSortTag,
                    tags: this.SelectActicleTags.join(','),
                    searchVal: this.SearchVal
                });
                this.TotalPageIndex = respone.Data.PageCount;
                this.CurPageIndex = respone.Data.PageIndex;
                let acticleList = respone.Data.Data;

                for (let i = 0; i < acticleList.length; i++) {
                    acticleList[i].CDate = this.$dayjs(acticleList[i].CDate).fromNow();
                    if (acticleList[i].PosterUrl != '')
                        acticleList[i].PosterUrl += '?imageMogr2/crop/120x120/gravity/center';
                }
                return acticleList;
            },
            OnClickActicleTag: function (tag) {
                console.log(tag)
                console.log(this.SelectActicleTags)
                let index = this.SelectActicleTags.indexOf(tag);
                console.log(index);
                if (index != '-1') {
                    this.SelectActicleTags = [
                        ...this.SelectActicleTags.slice(0, index),
                        ...this.SelectActicleTags.slice(index + 1)
                    ];
                } else {
                    this.SelectActicleTags = [
                        ...this.SelectActicleTags,
                        tag
                    ];
                    console.log(this.SelectActicleTags)
                }
                this.InitActicleList();
            },
            OnClickSortTag: async function (tag) {
                this.SelectSortTag = tag;
                this.ReleaseForm = '1';
                await this.InitActicleList();
            },
            OnClickReleaseFormTag: async function (tag) {
                this.SelectSortTag = '2';
                this.ReleaseForm = tag;
                await this.InitActicleList();
            },
            InitActicleList: async function (e) {
                this.ActicleList = [];
                this.$refs.Mescroll.mescroll.setPageNum(1);
                this.$refs.Mescroll.mescroll.triggerUpScroll();
            },
            OnClickActicleItem: function (item) {
                window.open("/view/acticleView/" + this.$route.params.blogname + "?id=" + item.Id, '_blank')
            }
        },
        async mounted() {
            this.RequestGetTags();
            await this.RequestGetEnums();
            this.ScrollInit();
        }
    }
</script>

<style scoped>
    .tag {
        background-color: #949494;
        color: white;
        font-size: .85rem;
    }

    .tag:hover {
        background-color: var(--main_color);
        cursor: pointer;
    }

    .sortTag {
        color: black;
    }

    .border-right {
        border-right: 1px solid hsla(0, 0%, 59.2%, .2);
    }

    .border-none {
        border: none;
    }

    .sortTag:hover {
        color: var(--main_color);
    }

    .content-main {
        width: 0;
        flex: 1 1 auto;
    }

    .item {}

    .item:hover {
        background-color: #ebebeb;
        cursor: pointer;
    }

    .item-header {
        color: #949494;
        font-size: 0.9rem;
    }

    .item-content {
        display: flex;
        border-bottom: 1px solid #949494;
    }

    .content {
        color: #949494;
        font-size: 1rem;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-box-orient: vertical;
        -webkit-line-clamp: 3;
    }

    .content-title {
        font-size: 1.25rem;
        font-weight: 700;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-box-orient: vertical;
        -webkit-line-clamp: 1;
    }

    .content-end {
        padding-top: 0.25rem;
        font-size: 0.85rem;
    }

    .content-img {
        margin-left: 24px;
        height: 120px;
        width: 120px;
    }

    .ListEmptyTip {
        width: 100%;
        text-align: center;
        height: 100px;
        line-height: 100px;
    }
</style>