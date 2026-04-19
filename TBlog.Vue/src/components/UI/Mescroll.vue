<!--
 * @Author: your name
 * @Date: 2022-03-10 15:30:53
 * @LastEditTime: 2022-03-13 18:41:46
 * @LastEditors: Please set LastEditors
 * @Description: 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 * @FilePath: \tblog\src\components\UI\Mescroll.vue
-->
<template>
  <div ref="mescrollDom" class="mescroll">
    <slot></slot>
  </div>
</template>

<script>
  import {
    ref
  } from 'vue'
  export default {
    name: 'Mescroll',
    props: ['mescrollDom', 'mescroll', 'lastScrollTop', 'lastBounce', 'init', 'upCallback', 'pageSize', 'pageIndex'],
    setup(props) {
      let mescrollDom = ref(null);
      let mescroll = ref(null);
      let init = function () {
        mescroll.value = new MeScroll(mescrollDom.value, {
          up: {
            callback: props.upCallback,
            page: {
              num: props.pageIndex,
              size: props.pageSize
            },
            htmlNodata: '<span class="upwarp-nodata">-- END --</span>',
            noMoreSize: 1,
            onScroll: function (mescroll, y, isUp) {
              console.log(y + ":" + isUp);
            }
          },
          down: {
            use: false
          },
        })
      }

      let beforeRouteEnter = function () {
        if (props.mescroll) {
          // 滚动到之前列表的位置
          if (props.lastScrollTop) {
            props.mescroll.setScrollTop(props.lastScrollTop)
            setTimeout(() => { // 需延时,因为setScrollTop内部会触发onScroll,可能会渐显回到顶部按钮
              props.mescroll.setTopBtnFadeDuration(0) // 设置回到顶部按钮显示时无渐显动画
            }, 16)
          }
          // 恢复到之前设置的isBounce状态
          if (props.lastBounce != null) props.mescroll.setBounce(props.lastBounce)
        }
      }

      let beforeRouteLeave = function () {
        if (props.mescroll) {
          props.lastScrollTop = props.mescroll.getScrollTop() // 记录当前滚动条的位置
          props.mescroll.hideTopBtn(0) // 隐藏回到顶部按钮,无渐隐动画
          props.lastBounce = props.mescroll.optUp.isBounce // 记录当前是否禁止ios回弹
          props.mescroll.setBounce(true) // 允许bounce
        }
      }

      return {
        mescrollDom,
        mescroll,
        init,
        beforeRouteLeave,
        beforeRouteEnter
      };
    }
  }
</script>

<style>

</style>