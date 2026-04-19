<template>
    <div class="justify-content-center px-xl-5 px-lg-4 px-md-3 px-sm-2 px-2 pb-3 w-100" style="padding-top:80px">
        <div class="w-100">
            <form class="mb-4 needs-validation">
                <div class="card">
                    <div class="card-header">
                        <CheckInput :PlaceholderVal="'文章标题'" :CheckAction="CheckRepeatTitle2" :RequiredVal=true
                            PatternVal="^.{1,30}$" :VerifyTipVal="'文章标题长度必须为1-30个字符'" ref="ActicleTitleInputDom">
                        </CheckInput>
                    </div>
                    <div class="card-body">
                        <div style="border: 1px solid #ccc">
                            <Toolbar style="border-bottom: 1px solid #ccc" :editor="EditorRef"
                                :defaultConfig="ToolbarConfig" :mode="Mode" />
                            <Editor style="height: 500px; overflow-y: hidden;" v-model="Content"
                                :defaultConfig="EditorConfig" :mode="Mode" @onCreated="HandleCreated" />
                        </div>
                    </div>
                    <div class="card-footer text-muted">
                        <div class="rounded-1 px-2 pb-2 row" style="background-color:white">
                            <div class="d-flex align-items-center py-3 my-2 border-bottom col-xl-6 col-lg-12">
                                <label style="font-weight:400" class="w-25 text-end"> 封面图片:</label>
                                <div class="d-flex justify-content-center w-75">
                                    <div class="UpLoadPoster d-flex align-items-center justify-content-center"
                                        @click="PosterImgDom.click()">
                                        <img ref="ViewPosterImg" class="ViewPosterImg img-thumbnail" :src="PosterImg"
                                            alt="选择头像" />
                                        <!-- <i class="bi bi-plus-circle-dotted" style="font-size:2rem"></i> -->
                                    </div>
                                </div>
                            </div>
                            <div class="d-flex align-items-center py-3 my-2 border-bottom col-xl-6 col-lg-12">
                                <label style="font-weight:400" class="w-25 text-end">文章标签:</label>
                                <div
                                    class="d-flex justify-content-center align-content-around flex-wrap w-75 pl-2 text-center">
                                    <button v-show="ActicleTags.length < 3" type="button"
                                        class="btn btn-outline-danger btn-sm mx-2 mb-2" data-bs-toggle="modal"
                                        data-bs-target="#addTagModal" @click="RequestGetTags">
                                        <i class="bi bi-plus"></i>
                                        添加标签
                                    </button>
                                    <button type="button" class="btn btn-outline-danger btn-sm mx-2 mb-2"
                                        v-for="(item, index) in ActicleTags" :key="index"
                                        @click="OnClickActicleTags(index)">
                                        {{ item }}
                                        <i class="bi bi-x"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="d-flex align-items-center py-3 my-2 border-bottom col-xl-6 col-lg-12">
                                <label style="font-weight:400" class="w-25 text-end">文章类型:</label>
                                <div class="d-flex justify-content-center w-75 pl-2">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="ActicleTypeRadio"
                                            id="Original" value="1" v-model="ActicleType">
                                        <label class="form-check-label" for="Original">
                                            原创
                                        </label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="ActicleTypeRadio"
                                            id="Reprint" value="2" v-model="ActicleType">
                                        <label class="form-check-label" for="Reprint">
                                            转载
                                        </label>
                                    </div>
                                </div>
                            </div>
                            <div class="d-flex align-items-center py-3 my-2 border-bottom col-xl-6 col-lg-12">
                                <label style="font-weight:400" class="w-25 text-end">发布形式:</label>
                                <div class="d-flex justify-content-center w-75 pl-2">
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="AccessRadio" id="Public"
                                            value="1" v-model="ActicleReleaseFormRadio">
                                        <label class="form-check-label" for="Public">
                                            公共
                                        </label>
                                    </div>
                                    <div class="form-check form-check-inline">
                                        <input class="form-check-input" type="radio" name="AccessRadio" id="Private"
                                            value="2" v-model="ActicleReleaseFormRadio">
                                        <label class="form-check-label" for="Private">
                                            私密
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <input type="file" ref="PosterImgDom" hidden @change="OnPosterImgChange" />
            </form>
        </div>
        <div class="BottomNav">
            <div class="d-flex align-items-center justify-content-evenly h-100 py-1">
                <loadingbtn class="btn btn-success rounded-pill" :awaitAction="OnClickSaveDraft" :btnText="'保存草稿'">
                </loadingbtn>
                <loadingbtn class="btn btn-main rounded-pill" :awaitAction="OnClickSaveActicle" :btnText="'发布博客'">
                </loadingbtn>
            </div>
        </div>

        <!-- 添加标签弹框 -->
        <div class="modal fade" id="addTagModal" tabindex="-1" aria-labelledby="TagModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="TagModalLabel">添加标签</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" data-bs-target="#addTagModal"
                            aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <input ref="InputArtcleTagDom" type="text" class="form-control text-center"
                            placeholder="标签名(最长不超过10个字符)" pattern="^.{1,10}$" required>
                        <div class="nav-scroller mt-3">
                            <nav class="nav d-flex">
                                <div v-for="(item, index) in OldActicleTags" :key="index"
                                    class="tag rounded-pill mx-2 py-1 px-3 mb-2 text-center"
                                    :style="'background-color:var(--blue)'" @click="OnClickAddActicleTag(item)">
                                    {{ item }}</div>
                            </nav>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-main" @click="OnClickAddActicleTag">添加</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- 预览弹框 -->
        <div class="modal fade p-0" id="ViewModal" tabindex="-1" aria-labelledby="ViewModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-fullscreen modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="ViewModalLabel">预览</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" data-bs-target="#ViewModal"
                            aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div id="ViewContent">

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 加载草稿确认弹窗 -->
        <div class="modal fade" id="loadDraftModal" tabindex="-1" aria-labelledby="loadDraftModalLabel" aria-hidden="true">
          <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="loadDraftModalLabel">提示</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="关闭"></button>
              </div>
              <div class="modal-body">
                检测到您上次有未保存的文章，是否加载？
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-secondary" @click="onLoadDraftCancel">取消</button>
                <button type="button" class="btn btn-primary" @click="onLoadDraftConfirm">加载</button>
              </div>
            </div>
          </div>
        </div>
    </div>
</template>

<script>
import { useRoute, useRouter } from 'vue-router'
import {
    CheckRepeatTitle,
    SaveActicle,
    UpLoadImgByFile,
    GetActicle,
    GetTags
} from '../assets/js/interface.js'
import {
    Modal
} from 'bootstrap/dist/js/bootstrap.bundle'
import PosterImgUrl from "../assets/svg/plus-circle-dotted.svg"
import '@wangeditor/editor/dist/css/style.css'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import { onBeforeUnmount, ref, shallowRef, onMounted, getCurrentInstance } from 'vue'
import { watch } from 'vue';

export default {
    components: { Editor, Toolbar },
    name: "ArtcleEditor",
    setup() {
        const { proxy } = getCurrentInstance();
        const route = useRoute();
        const router = useRouter();
        const Title = ref('');
        const Content = ref('');
        const PosterImg = ref(PosterImgUrl);
        const ActicleTitleInputDom = ref();
        const InputArtcleTagDom = ref();
        const PosterImgDom = ref();
        const ActicleReleaseFormRadio = ref('1');
        const ActicleType = ref('1');
        const ActicleTags = ref([]);
        const OldActicleTags = ref([]);
        const EditorRef = shallowRef();
        const ToolbarConfig = {};
        const EditorConfig = {
            placeholder: '请输入内容...',
            MENU_CONF: {
                'uploadImage': {
                    server: '/api/Media/UpLoadImgByFile?path=ActicleImg',
                    async customUpload(file, insertFn) {
                        let respone = await UpLoadImgByFile('ActicleImg', file);
                        if (respone.Status == 200) {
                            if (PosterImg.value.indexOf('/svg/plus-circle-dotted.svg') != -1 && PosterImgDom.value.files.length == 0) {
                                PosterImg.value = respone.Data;
                            }
                        }
                        insertFn(respone.Data, '', '')
                    }
                }
            }
        };
        const Mode = 'default';
        const getCacheKey = () => {
            const id = route.query.id;
            return `article_draft_${id ? id : 'draft'}`;
        };

        watch([Content, ActicleTags, ActicleType, ActicleReleaseFormRadio], () => {
            const cacheKey = getCacheKey();
            const cacheData = {
                title: ActicleTitleInputDom.value?.InputValue || '',
                content: Content.value,
                tags: ActicleTags.value,
                acticleType: ActicleType.value,
                releaseForm: ActicleReleaseFormRadio.value,
                posterImg: PosterImg.value
            };
            localStorage.setItem(cacheKey, JSON.stringify(cacheData));
        });

        // --------- 新增：加载草稿弹窗逻辑 ---------
        let loadDraftData = null;
        let loadDraftModalInstance = null;
        function onLoadDraftConfirm() {
            if (loadDraftData) {
                const data = loadDraftData;
                if (data.title) ActicleTitleInputDom.value.InputValue = data.title;
                if (data.content) Content.value = data.content;
                if (data.tags) ActicleTags.value = data.tags;
                if (data.acticleType) ActicleType.value = data.acticleType;
                if (data.releaseForm) ActicleReleaseFormRadio.value = data.releaseForm;
                if (data.posterImg) PosterImg.value = data.posterImg;
            }
            loadDraftModalInstance.hide();
        }
        function onLoadDraftCancel() {
            loadDraftModalInstance.hide();
        }
        // --------- 新增结束 ---------

        onMounted(() => {
            document.getElementById('addTagModal').addEventListener('hide.bs.modal', function (event) {
                InputArtcleTagDom.value = '';
            })
            window.onbeforeunload = function () {
                return confirm("您的文章未保存，确定离开吗？");
            }
        })

        onBeforeUnmount(() => {
            const editor = EditorRef.value
            if (editor == null) return
            editor.destroy()
        })

        const HandleCreated = async (editor) => {
            EditorRef.value = editor;
            await InitActicle();
        }

        const OnPosterImgChange = () => {
            if (PosterImgDom.value.files.length >= 1) {
                let file = PosterImgDom.value.files[0];
                if (file.size > 1 * 1024 * 1024) {
                    proxy.$toast.warning("图片大小不能超过2MB");
                    PosterImgDom.value = '';
                    return;
                }
                let imgUrl = getObjectURL(file);
                PosterImg.value = imgUrl;
            }
        }

        function OnClickActicleTags(index) {
            ActicleTags.value = [
                ...ActicleTags.value.slice(0, index),
                ...ActicleTags.value.slice(index + 1)
            ];
        }

        function OnClickAddActicleTag(title) {
            let tag = title ? title : InputArtcleTagDom.value;

            if (ActicleTags.value.length >= 3) {
                proxy.$toast.error("标签最多添加3个!");
                return;
            }

            if (tag == '' || tag == undefined) {
                proxy.$toast.warning('标签不能为空!');
                return;
            }

            ActicleTags.value = [...ActicleTags.value, tag];
            Modal.getOrCreateInstance(document.getElementById('addTagModal')).hide();
        }

        async function OnClickSaveActicle() {
            if (ActicleReleaseFormRadio.value == '3') {
                ActicleReleaseFormRadio.value = '1';
            }
            await RequestSaveActicle(false);
        }

        async function OnClickSaveDraft() {
            await RequestSaveActicle(true);
        }

        async function RequestSaveActicle(isDraft) { //isDraft:是否为草稿
            if (!ActicleTitleInputDom.value.CheckValidity) {
                proxy.$toast.error("文章标题有误");
                return;
            }

            if (PosterImgDom.value.files.length != 0) {
                let respone = await UpLoadImgByFile('acticleposter', PosterImgDom.value.files[0]);
                if (respone == null || respone.Status == 500) {
                    proxy.$toast.error("文章封面上传失败");
                    return;
                } else {
                    PosterImg.value = respone.Data;
                }
            }
            let posterImg = PosterImg.value.indexOf('plus-circle-dotted') == -1 ? PosterImg.value : '';
            let respone = await SaveActicle({
                "id": route.query.id,
                "title": ActicleTitleInputDom.value.InputValue,
                "content": Content.value,
                "posterUrl": posterImg,
                "tags": ActicleTags.value,
                "acticleType": ActicleType.value,
                "releaseForm": isDraft ? '3' : ActicleReleaseFormRadio.value
            });
            if (respone.Status == 200) {
                window.onbeforeunload = null;
                localStorage.removeItem(getCacheKey());
                router.push("/view/acticleView/" + route.params.blogname + "?id=" + respone.Data);
            }
        }

        let oldActicleTitle = '';
        function CheckRepeatTitle2(title) {
            if (route.query.id != undefined && oldActicleTitle == title) return;
            return CheckRepeatTitle(title);
        }

        async function RequestGetTags() {
            let respone = await GetTags(route.params.blogname, 1);
            OldActicleTags.value = respone.Data;
        }

        async function InitActicle() {
            if (route.query.id != undefined) {
                let respone = await GetActicle(route.query.id);
                if (respone.Status == 200) {
                    oldActicleTitle = respone.Data.Title;
                    ActicleTitleInputDom.value.InputValue = respone.Data.Title;
                    ActicleTags.value = [...respone.Data.Tags];
                    if (respone.Data.PosterUrl)
                        PosterImg.value = respone.Data.PosterUrl;
                    ActicleReleaseFormRadio.value = respone.Data.ReleaseForm;
                    ActicleType.value = respone.Data.ActicleType;
                    Content.value = respone.Data.Content;
                }
                return;
            }
            readActicle();
        }

        function readActicle(){
            const cacheKey = getCacheKey();
            const cache = localStorage.getItem(cacheKey);
            if (cache) {
                try {
                    loadDraftData = JSON.parse(cache);
                    if (!loadDraftModalInstance) {
                        loadDraftModalInstance = Modal.getOrCreateInstance(document.getElementById('loadDraftModal'));
                    }
                    loadDraftModalInstance.show();
                } catch (e) {}
            }
        }

        return {
            Title,
            Content,
            PosterImg,
            ActicleReleaseFormRadio,
            ActicleType,
            ActicleTags,
            EditorRef,
            ToolbarConfig,
            EditorConfig,
            Mode,
            HandleCreated,
            OnPosterImgChange,
            OnClickActicleTags,
            OnClickAddActicleTag,
            OnClickSaveActicle,
            OnClickSaveDraft,
            RequestSaveActicle,
            InitActicle,
            CheckRepeatTitle2,
            PosterImgDom,
            ActicleTitleInputDom,
            RequestGetTags,
            OldActicleTags,
            onLoadDraftConfirm,
            onLoadDraftCancel
        }
    }
}
</script>

<style scoped>
.BottomNav {
    position: fixed;
    bottom: 0;
    width: 100vw;
    height: 4rem;
    right: 0;
    background-color: rgb(255, 255, 255);
    z-index: 1051;
    border-top: darkgray solid 1px;
}

.UpLoadPoster {
    border: 1px dashed #bfbfbf;
    border-radius: 4px;
    width: 90px;
    height: 90px;
}

.UpLoadPoster:hover {
    cursor: pointer;
    background-color: rgb(221, 221, 221);
}

.ViewPosterImg {
    max-height: 90px;
    width: auto;
}

.tag {
    background-color: #949494;
    color: white;
    font-size: .85rem;
}

.tag:hover {
    background-color: var(--main_color);
    cursor: pointer;
}
</style>