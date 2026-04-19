<!--
 * @Author: your name
 * @Date: 2022-03-21 16:31:24
 * @LastEditTime: 2022-06-24 20:18:22
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\CompanyInfo.vue
-->
<template>
    <div class="card">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
            <label class="tagTitle">工作经历</label>
            <button class="btn btn-main" style="font-size:1.25em;font-weight: 700;" @click="OnClickAddCompanyInfo"><i
                    class="bi bi-plus"></i></button>
        </div>
        <div class="card-body pt-0">
            <div class="needs-validation p-2 my-4" v-for="(item,index) in CompanyInfos" :key="index">
                <div class="row">
                    <div class="col  my-1">
                        <div class="d-flex justify-content-between align-items-center">
                            <label class="form-label inputTitle">公司名称</label>
                            <div>
                                <button class="btn btn-outline-danger btn-sm mb-2 float-end" style="height:2rem;"
                                    @click="OnClickCloseButton(index)"><i class="bi bi-x"></i></button>
                                <button v-show="index!=0" class="btn btn-outline-primary btn-sm mb-2 float-end"
                                    style="height:2rem;margin-right:15px" @click="OnClickUpButton(index)"><i
                                        class="bi bi-arrow-up"></i></button>
                            </div>
                        </div>

                        <input type="text" class="form-control"
                            :class="IsSumbit||item.Company.length!=0?'was-validated':''" pattern="^.{1,20}$"
                            v-model="item.Company" required>
                        <div class="invalid-feedback">
                            长度需为1-20个字符
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col  my-1">
                        <div class="d-flex justify-content-between align-items-center">
                            <label class="form-label">职位</label>
                        </div>

                        <input type="text" class="form-control"
                            :class="IsSumbit||item.Position.length!=0?'was-validated':''" pattern="^.{1,20}$"
                            v-model="item.Position" required>
                        <div class="invalid-feedback">
                            长度需为1-20个字符
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col my-1">
                        <label class="form-label">部门</label>
                        <input type="text" class="form-control"
                            :class="IsSumbit||item.Department.length!=0?'was-validated':''" pattern="^.{1,20}$"
                            v-model="item.Department" required>
                        <div class="invalid-feedback">
                            长度需为1-20个字符
                        </div>
                    </div>
                    <div class="col  my-1">
                        <label class="form-label">所在城市</label>
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
                        <textarea type="text" class="form-control "
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
        SaveCompanyInfo,
        GetCompanyInfo
    } from '../assets/js/interface.js';
    export default {
        name: "CompanyInfo",
        data() {
            return {
                IsSumbit: false,
                CompanyInfos: []
            }
        },
        methods: {
            OnClickAddCompanyInfo() {
                this.CompanyInfos = [{
                    Company: "",
                    Position: "",
                    Department: "",
                    City: "",
                    StartDate: this.$dayjs().format('YYYY-MM-DD'),
                    EndDate: this.$dayjs().format('YYYY-MM-DD'),
                    Introduction: ""
                }, ...this.CompanyInfos];
            },
            OnClickCloseButton(index) {
                this.CompanyInfos = [
                    ...this.CompanyInfos.slice(0, index),
                    ...this.CompanyInfos.slice(index + 1)
                ];
            },
            async OnClickUpButton(index) {
                this.CompanyInfos = [
                    ...this.CompanyInfos.slice(0, index - 1),
                    this.CompanyInfos[index],
                    this.CompanyInfos[index - 1],
                    ...this.CompanyInfos.slice(index + 1)
                ];
                this.RefreshSlider();
            },
            async OnClickSaveButton() {
                await SaveCompanyInfo(this.CompanyInfos);
            }
        },
        async mounted() {
            let respone = await GetCompanyInfo(this.$route.params.blogname);
            if (respone.Status == 200) {
                this.CompanyInfos = respone.Data;
                if (this.CompanyInfos.length == 0) {
                    this.CompanyInfos = [{
                        Company: "",
                        Position: "",
                        Department: "",
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