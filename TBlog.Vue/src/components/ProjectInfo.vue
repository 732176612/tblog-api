<!--
 * @Author: your name
 * @Date: 2022-03-21 16:31:24
 * @LastEditTime: 2022-06-24 20:19:39
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\ProjectInfo.vue
-->
<template>
    <div class="card">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
            <label class="tagTitle">项目经历</label>
            <button class="btn btn-main" style="font-size:1.25em;font-weight: 700;" @click="OnClickAddProjectInfo"><i
                    class="bi bi-plus"></i></button>
        </div>
        <div class="card-body pt-0">
            <div class="needs-validation p-2 my-4" v-for="(item,index) in ProjectInfos" :key="index">
                <div class="row">
                    <div class="col  my-1">
                        <div class="d-flex justify-content-between align-items-center">
                            <label class="form-label inputTitle">项目名称</label>
                            <div>
                                <button class="btn btn-outline-danger btn-sm mb-2 float-end" style="height:2rem;"
                                    @click="OnClickCloseButton(index)"><i class="bi bi-x"></i></button>
                                <button v-show="index!=0" class="btn btn-outline-primary btn-sm mb-2 float-end"
                                    style="height:2rem;margin-right:15px" @click="OnClickUpButton(index)"><i
                                        class="bi bi-arrow-up"></i></button>
                            </div>
                        </div>

                        <input type="text" class="form-control"
                            :class="IsSumbit||item.Project.length!=0?'was-validated':''" pattern="^.{1,20}$"
                            v-model="item.Project" required>
                        <div class="invalid-feedback">
                            长度需为1-20个字符
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col my-1">
                        <label class="form-label">担任角色</label>
                        <input type="text" class="form-control"
                            :class="IsSumbit||item.Role.length!=0?'was-validated':''" pattern="^.{1,20}$"
                            v-model="item.Role" required>
                        <div class="invalid-feedback">
                            长度需为1-20个字符
                        </div>
                    </div>
                    <div class="col  my-1">
                        <label class="form-label">所在公司</label>
                        <input type="text" class="form-control" v-model="item.City"
                            :class="IsSumbit||item.City.length!=0?'was-validated':''" pattern="^.{1,20}$" required>
                        <div class="invalid-feedback">
                            不能为空,且长度不能超过20个字符
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col my-1 dateTimePickerLayout">
                        <label class="form-label">开始时间</label>
                        <input type="text" class="form-control" v-model="item.StartDate">
                        <input type="date" class="form-control dateTimePicker" v-model="item.StartDate">
                    </div>

                    <div class="col my-1 dateTimePickerLayout">
                        <label class="form-label">结束时间</label>
                        <input type="text" class="form-control" v-model="item.EndDate">
                        <input type="date" class="form-control dateTimePicker" v-model="item.EndDate">
                    </div>
                </div>

                <div class="row my-1">
                    <div class="col">
                        <label class="form-label">经历描述</label>
                        <textarea ref="Introduction" type="text" class="form-control "
                            :class="IsSumbit||item.Introduction.length!=0?'was-validated':''" pattern="^.{0,140}$"
                            v-model="item.Introduction"></textarea>
                        <div class="invalid-feedback">
                            长度不能超过140个字符
                        </div>
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
        SaveProjectInfo,
        GetProjectInfo
    } from '../assets/js/interface.js';
    export default {
        name: "ProjectInfo",
        data() {
            return {
                IsSumbit: false,
                ProjectInfos: []
            }
        },
        methods: {
            OnClickAddProjectInfo() {
                this.ProjectInfos = [{
                    Project: "",
                    Role: "",
                    City: "",
                    StartDate: this.$dayjs().format('YYYY-MM-DD'),
                    EndDate: this.$dayjs().format('YYYY-MM-DD'),
                    Introduction: ""
                }, ...this.ProjectInfos];
            },
            OnClickCloseButton(index) {
                this.ProjectInfos = [
                    ...this.ProjectInfos.slice(0, index),
                    ...this.ProjectInfos.slice(index + 1)
                ];
            },
            async OnClickUpButton(index) {
                this.ProjectInfos = [
                    ...this.ProjectInfos.slice(0, index-1),
                    this.ProjectInfos[index],
                    this.ProjectInfos[index-1],
                    ...this.ProjectInfos.slice(index + 1)
                ];
                this.RefreshSlider();
            },
            async OnClickSaveButton() {
                await SaveProjectInfo(this.ProjectInfos);
            }
        },
        async mounted() {
            let respone = await GetProjectInfo(this.$route.params.blogname);
            if (respone.Status == 200) {
                this.ProjectInfos = respone.Data;
                if (this.ProjectInfos.length == 0) {
                    this.ProjectInfos = [{
                        Project: "",
                        Role: "",
                        City: "",
                        StartDate: this.$dayjs().format('YYYY-MM-DD'),
                        EndDate: this.$dayjs().format('YYYY-MM-DD'),
                        Introduction: ""
                    }];
                }
            }
        }
    }
</script>

<style>
.dateTimePickerLayout {
    position: relative;
}

.dateTimePicker {
    position: absolute;
    top: 32px;
    width: 45px;
    right: 12px;
}
</style>