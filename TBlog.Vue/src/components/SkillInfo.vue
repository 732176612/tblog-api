<template>
    <div class="card">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
            <label class="tagTitle">专业技能</label>
            <button v-if="DisplayMode === 'progress'" class="btn btn-main" style="font-size:1.25em;font-weight:700;"
                @click="OnClickAddSkillInfo" aria-label="添加技能">
                <i class="bi bi-plus"></i>
            </button>
        </div>
        <div class="card-body pt-0">
            <div class="mt-3" role="group" aria-labelledby="skill-display-mode-label">
                <div id="skill-display-mode-label" class="form-label fw-bold">专业技能展示方式</div>
                <label class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" value="progress" v-model="DisplayMode">
                    <span class="form-check-label">进度条</span>
                </label>
                <label class="form-check form-check-inline">
                    <input class="form-check-input" type="radio" value="text" v-model="DisplayMode">
                    <span class="form-check-label">文字描述</span>
                </label>
            </div>

            <template v-if="DisplayMode === 'progress'">
                <div class="needs-validation" v-for="(item, index) in SkillInfos" :key="index">
                    <div class="row mt-2">
                        <div class="col-3 my-1"><label class="form-label">技能</label></div>
                        <div class="col-9 my-1">
                            <label class="form-label">熟练度</label>
                            <button class="btn btn-outline-danger btn-sm mb-2 float-end" style="height:2rem;"
                                @click="OnClickCloseButton(index)" aria-label="删除技能"><i class="bi bi-x"></i></button>
                            <button v-show="index !== 0" class="btn btn-outline-primary btn-sm mb-2 float-end"
                                style="height:2rem;margin-right:15px" @click="OnClickUpButton(index)" aria-label="上移技能">
                                <i class="bi bi-arrow-up"></i>
                            </button>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-3 my-1">
                            <input type="text" class="form-control" v-model="item.Skill" maxlength="20" required>
                        </div>
                        <div class="col-9 my-1 d-flex align-items-center gap-2">
                            <input type="range" class="form-range flex-grow-1" min="0" max="100" step="10"
                                v-model.number="item.Progress" :aria-label="item.Skill || '熟练度'">
                            <span>{{item.Progress}}%</span>
                        </div>
                    </div>
                </div>
            </template>
            <div v-else class="mt-3">
                <label for="skill-descriptions" class="form-label">技能描述（每行一条）</label>
                <textarea id="skill-descriptions" class="form-control" rows="12" v-model="TextContent"
                    placeholder="每行填写一条技能描述"></textarea>
            </div>

            <div class="d-grid mt-3">
                <loadingbtn class="btn-block btn-main" :awaitAction="OnClickSaveButton" :btnText="'保存'">
                </loadingbtn>
            </div>
        </div>
    </div>
</template>

<script>
import { SaveSkillInfo, GetSkillInfo } from '../assets/js/interface.js';

export default {
    name: 'SkillInfo',
    data() {
        return {
            DisplayMode: 'progress',
            SkillInfos: [],
            TextContent: ''
        }
    },
    methods: {
        OnClickAddSkillInfo() {
            this.SkillInfos = [{ Skill: '', Progress: 0 }, ...this.SkillInfos];
        },
        OnClickCloseButton(index) {
            this.SkillInfos.splice(index, 1);
        },
        OnClickUpButton(index) {
            [this.SkillInfos[index - 1], this.SkillInfos[index]] =
                [this.SkillInfos[index], this.SkillInfos[index - 1]];
        },
        async OnClickSaveButton() {
            const infos = this.DisplayMode === 'text'
                ? this.TextContent.split(/\r?\n/).map(line => line.trim()).filter(Boolean)
                    .map(Skill => ({ Skill, Progress: 0 }))
                : this.SkillInfos.filter(item => (item.Skill || '').trim());
            await SaveSkillInfo(infos.map((item, index) => ({
                Skill: item.Skill,
                Progress: this.DisplayMode === 'progress' ? Number(item.Progress) || 0 : 0,
                Sort: index,
                DisplayMode: this.DisplayMode
            })));
        }
    },
    async mounted() {
        const response = await GetSkillInfo(this.$route.params.blogname);
        if (response.Status === 200) {
            this.SkillInfos = response.Data || [];
            this.DisplayMode = this.SkillInfos[0]?.DisplayMode === 'text' ? 'text' : 'progress';
            this.TextContent = this.SkillInfos.map(item => item.Skill).join('\n');
            if (this.SkillInfos.length === 0) this.OnClickAddSkillInfo();
        }
    }
}
</script>
