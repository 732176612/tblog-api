<!--
 * @Author: FalseEndLess 732176612@qq.com
 * @Date: 2021-10-16 15:31:30
 * @LastEditors: FalseEndLess 732176612@qq.com
 * @LastEditTime: 2022-05-31 20:03:08
 * @FilePath: \tblog\src\App.vue
 * @Description: 这是默认设置,请设置`customMade`, 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
-->
<template>
  <router-view />
  <button
    v-show="showBackTop"
    class="back-to-top"
    @click="scrollToTop"
    aria-label="回到顶部"
  >
    <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="18 15 12 9 6 15"></polyline></svg>
  </button>
</template>

<script>
import { ref, onMounted, onUnmounted } from 'vue';
export default {
  setup() {
    const showBackTop = ref(false);
    const handleScroll = () => {
      showBackTop.value = window.scrollY > 200;
    };
    const scrollToTop = () => {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    };
    onMounted(() => {
      window.addEventListener('scroll', handleScroll);
    });
    onUnmounted(() => {
      window.removeEventListener('scroll', handleScroll);
    });
    return { showBackTop, scrollToTop };
  }
};
</script>

<style>
.back-to-top {
  position: fixed;
  right: 2.5rem;
  bottom: 2.5rem;
  z-index: 9999;
  background: linear-gradient(135deg, #4f8cff 60%, #6ed0ff 100%);
  color: #fff;
  border: none;
  border-radius: 50%;
  width: 48px;
  height: 48px;
  box-shadow: 0 4px 16px rgba(79,140,255,0.15);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: background 0.2s, box-shadow 0.2s, opacity 0.2s;
  opacity: 0.85;
}
.back-to-top:hover {
  background: linear-gradient(135deg, #3578e5 60%, #4f8cff 100%);
  box-shadow: 0 8px 24px rgba(79,140,255,0.25);
  opacity: 1;
}
.back-to-top svg {
  display: block;
  margin: 0 auto;
}
</style>
