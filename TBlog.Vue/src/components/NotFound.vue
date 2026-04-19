<template>
    <div class="notfound-container">
        <div class="notfound-content">
            <h1>404</h1>
            <p>抱歉，您访问的页面不存在。</p>
            <router-link v-if="BlogName == ''" to="/view/login" class="back-home">前往登录</router-link>
            <router-link v-if="BlogName != ''" :to="'/view/index/' + BlogName"  class="back-home">回到我的博客</router-link>
        </div>
    </div>
</template>

<script>
import {
    SerializeJwt
} from '../assets/js/interface.js';
export default {
    data() {
        return {
            BlogName: ''
        }
    },
    async mounted() {
        await this.CheckToken();
    },
    methods: {
        async CheckToken() {
            this.Config.token = getCookie('token');
            await this.RefreshUserSelf();
        },
        async RefreshUserSelf() {
            if (this.Config.token != '') {
                let respone = await SerializeJwt({
                    "token": this.Config.token
                });
                if (respone.Data.BlogName != null && respone.Data.BlogName != '') {
                    this.BlogName = respone.Data.BlogName;
                    console.log(this.BlogName)
                }
            }
        }
    }
}
</script>

<style scoped>
.notfound-container {
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #e0e7ff 0%, #f8fafc 100%);
}

.notfound-content {
    text-align: center;
    background: #fff;
    padding: 3rem 2.5rem;
    border-radius: 1.5rem;
    box-shadow: 0 8px 32px rgba(79, 140, 255, 0.10);
}

.notfound-content h1 {
    font-size: 6rem;
    font-weight: bold;
    color: #4f8cff;
    margin-bottom: 1rem;
}

.notfound-content p {
    font-size: 1.5rem;
    color: #555;
    margin-bottom: 2rem;
}

.back-home {
    display: inline-block;
    padding: 0.75rem 2.5rem;
    background: linear-gradient(90deg, #4f8cff 60%, #6ed0ff 100%);
    color: #fff;
    border-radius: 2rem;
    font-size: 1.1rem;
    text-decoration: none;
    transition: background 0.2s, box-shadow 0.2s;
    box-shadow: 0 2px 8px rgba(79, 140, 255, 0.10);
}

.back-home:hover {
    background: linear-gradient(90deg, #3578e5 60%, #4f8cff 100%);
}
</style>