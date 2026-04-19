<template>
    <div class="card">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
            <label class="tagTitle">专业技能</label>
            <button class="btn btn-main" style="font-size:1.25em;font-weight: 700;" @click="OnClickAddSkillInfo">
                <i class="bi bi-plus"></i>
            </button>
        </div>
        <div class="card-body pt-0">
            <div class="needs-validation" v-for="(item, index) in SkillInfos" :key="index">
                <div class="row mt-2">
                    <div class="col-3 my-1">
                        <label class="form-label">技能</label>
                    </div>
                    <div class="col-9 my-1">
                        <label class="form-label">熟练度</label>
                        <button class="btn btn-outline-danger btn-sm mb-2 float-end" style="height:2rem;"
                            @click="OnClickCloseButton(index)"><i class="bi bi-x"></i></button>
                        <button v-show="index != 0" class="btn btn-outline-primary btn-sm mb-2 float-end"
                            style="height:2rem;margin-right:15px" @click="OnClickUpButton(index)"><i
                                class="bi bi-arrow-up"></i></button>
                    </div>
                </div>

                <div class="row">
                    <div class="col-3 my-1">
                        <input type="text" class="form-control"
                            :class="IsSumbit || item.Skill.length != 0 ? 'was-validated' : ''" pattern="^.{1,20}$"
                            v-model="item.Skill" required />
                    </div>
                    <div class="col-9 my-1 pt-2 d-flex">
                        <input :id="'ex' + index" :ref="'ex' + index" :data-slider-id="'ex' + index" type="text"
                            data-slider-min="0" data-slider-max="100" data-slider-step="10"
                            :data-slider-value="item.Progress" style="width:100%;margin-top:7px;" />
                    </div>
                </div>

            </div>

            <div class="d-grid mt-3">
                <loadingbtn class="btn-block btn-main" :awaitAction="OnClickSaveButton" :btnText="'保存'">
                </loadingbtn>
            </div>
        </div>
    </div>
</template>

<script>
import {
    SaveSkillInfo,
    GetSkillInfo
} from '../assets/js/interface.js';
import {
    nextTick
} from 'vue'
import Slider from 'bootstrap-slider'
import 'bootstrap-slider/dist/css/bootstrap-slider.min.css'
export default {
    name: "SkillInfo",
    data() {
        return {
            IsSumbit: false,
            SkillInfos: []
        }
    },
    methods: {
        OnClickAddSkillInfo() {
            this.SkillInfos = [{
                Skill: "",
                SkillCompent: undefined,
                Progress: 0,
                Sort: ""
            }, ...this.SkillInfos];
            this.RefreshSlider();
        },
        async OnClickCloseButton(index) {
            this.SkillInfos = [
                ...this.SkillInfos.slice(0, index),
                ...this.SkillInfos.slice(index + 1)
            ];
            this.RefreshSlider();
        },
        async OnClickUpButton(index) {
            this.SkillInfos = [
                ...this.SkillInfos.slice(0, index - 1),
                this.SkillInfos[index],
                this.SkillInfos[index - 1],
                ...this.SkillInfos.slice(index + 1)
            ];
            this.RefreshSlider();
        },
        async OnClickSaveButton() {
            for (var i = 0; i < this.SkillInfos.length; i++) {
                console.log(this.SkillInfos[i].SkillCompent.getValue());
                this.SkillInfos[i].Progress = this.SkillInfos[i].SkillCompent.getValue();
                this.SkillInfos[i].SkillCompent = '';
                this.SkillInfos[i].Sort = i;
            }
            await SaveSkillInfo(this.SkillInfos);
        },
        async RefreshSlider() {
            let temp1 = [...this.SkillInfos];
            this.SkillInfos = [];
            await nextTick();
            this.SkillInfos = temp1;
            await nextTick();
            for (var i = 0; i < this.SkillInfos.length; i++) {
                let value = 0;
                if (this.SkillInfos[i].SkillCompent != undefined)
                    value = this.SkillInfos[i].SkillCompent.getValue();
                this.SkillInfos[i].SkillCompent = new Slider('#ex' + i, {
                    formatter: function (value) {
                        return value + "%";
                    }
                });
                if (value > 0) {
                    this.SkillInfos[i].SkillCompent.setValue(value);
                }
            }
        }
    },
    async mounted() {
        let respone = await GetSkillInfo(this.$route.params.blogname);
        if (respone.Status == 200) {
            this.SkillInfos = respone.Data;
            if (this.SkillInfos.length == 0) {
                this.SkillInfos = [{
                    Skill: "",
                    Progress: 0,
                    Sort: "",
                    SkillCompent: undefined,
                }];
            }
            this.RefreshSlider();
        }
    }
}
</script>

<style></style>