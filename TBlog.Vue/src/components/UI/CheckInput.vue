<!--
 * @Author: your name
 * @Date: 2022-02-26 12:19:17
 * @LastEditTime: 2022-02-28 15:51:17
 * @LastEditors: Please set LastEditors
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\UI\CheckInput.vue
-->
<template>
    <div class="input-group">
        <input id="test" type="text" class="form-control text-input"
            :class="(IsVerify == true ? '' : 'is-invalid') + ' ' + ClassVal" :placeholder="PlaceholderVal" :pattern="PatternVal"
            @change="Onchange" :readonly="ReadonlyVal ? 'readonly' : false" :required="RequiredVal ? 'required' : false"
            :autocomplete="AutocompleteVal ? 'autocomplete' : false" ref="CheckInput" v-model="InputValue" />
        <div class="invalid-feedback">
            {{ RequestErrorTip == '' ? VerifyTipVal : RequestErrorTip }}
        </div>
    </div>
</template>

<script>
import {
    ref,
    computed,
    watchEffect
} from 'vue'
export default {
    props: ['ClassVal', 'PatternVal', 'PlaceholderVal', 'ReadonlyVal', 'RequiredVal', 'AutocompleteVal',
        'VerifyTipVal', 'IsVerify', 'InputValue', 'CheckValidity',
        'CheckAction', 'CheckInput', 'RequestErrorTip'
    ],
    setup(props) {
        const ClassVal = ref(props.ClassVal == undefined ? '' : props.ClassVal);
        const RequestErrorTip = ref('');
        const PatternVal = computed(() => {
            return props.PatternVal;
        });
        const PlaceholderVal = computed(() => {
            return props.PlaceholderVal;
        });

        const ReadonlyVal = computed(() => {
            return props.ReadonlyVal;
        });

        const RequiredVal = computed(() => {
            return props.RequiredVal;
        });

        const AutocompleteVal = computed(() => {
            return props.AutocompleteVal;
        });

        const IsVerify = ref(true);
        const VerifyTipVal = ref(props.VerifyTipVal);

        let IsFirst = true;
        const CheckAction = computed(() => {
            return props.CheckAction;
        });
        const CheckInput = ref(null);
        const InputValue = ref((props.InputValue == undefined ? '' : props.InputValue));
        const asyncPrint = val => {
            return setTimeout(async () => {
                if (IsFirst) {
                    IsFirst = false;
                    return;
                }

                IsVerify.value = CheckInput.value.checkValidity();
                if (!IsVerify.value) {
                    RequestErrorTip.value = '';
                    return;
                }
                if (props.CheckAction != undefined) {
                    let respone = await props.CheckAction(val);
                    if (respone) {
                        if (respone.Status != 200) {
                            RequestErrorTip.value = respone.Msg;
                            IsVerify.value = false;
                        } else {
                            RequestErrorTip.value = '';
                            IsVerify.value = true;
                        }
                    }
                }
            }, 500)
        }

        const CheckValidity = computed(() => {
            if (CheckInput == null) return false;
            let val = CheckInput.value.checkValidity();
            if (val == false) {
                IsVerify.value = false;
            }
            return val;
        })

        watchEffect(
            onInvalidate => {
                const timer = asyncPrint(InputValue.value)
                onInvalidate(() => clearTimeout(timer))
            }, {
            flush: 'post'
        }
        )

        return {
            ClassVal,
            PatternVal,
            PlaceholderVal,
            ReadonlyVal,
            RequiredVal,
            AutocompleteVal,
            VerifyTipVal,
            CheckAction,
            InputValue,
            CheckInput,
            IsVerify,
            InputValue,
            CheckValidity,
            RequestErrorTip
        }
    }
}
</script>