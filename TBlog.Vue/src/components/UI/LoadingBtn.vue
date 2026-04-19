<!--
 * @Author: qyz
 * @Date: 2021-12-26 15:57:51
 * @LastEditTime: 2022-01-17 17:00:35
 * @LastEditors: Please set LastEditors
 * @Description: 加载中按钮组件 {btnText：按钮文本; awaitAction：按钮方法;)
 * @FilePath: \tblog\src\components\UI\Loading-Button.vue
-->
<template>
    <button type="button" class="btn" @click="ClickAction">
        <span class="spinner-border spinner-border-sm" v-show="showSpinner"></span>
        {{btnText}}
    </button>
</template>

<script>
import {computed} from 'vue'
    export default {
        data() {
            return {
                showSpinner: false
            }
        },
        props: ['btnText', 'awaitAction'],
        methods: {
            async ClickAction() {
                if(this.showSpinner){
                    this.$toast.warning('请勿重复点击');
                    return;
                }
                if (this.awaitAction != "") {
                    this.showSpinner = true;
                    try{
                        await this.awaitAction();
                    }
                    catch(e){
                     console.log(e)   
                    }
                    this.showSpinner = false;
                }
            }
        },
        setup(props) {
            const btnText = computed(() => {
                return props.btnText;
            })
            return {
                btnText
            }
        }
    }
</script>