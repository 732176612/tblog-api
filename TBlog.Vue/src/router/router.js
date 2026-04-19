import {
  createRouter
} from 'vue-router'
const routes = [{
  name: "登录",
  path: '/view/login',
  component: () => import('../components/Login.vue')
}, {
  name: "注册",
  path: '/view/register',
  component: () => import('../components/Register.vue')
}, {
  name: "找回密码",
  path: '/view/recoverpwd',
  component: () => import('../components/RecoverPwd.vue')
}, {
  name: "首页",
  path: '/view/index/:blogname',
  component: () => import('../components/Index.vue'),
  children: [{
    name: "首页 ",
    path: '',
    component: () => import('../components/Index-Main.vue')
  }]
}, {
  path: '/view/index',
  component: () => import('../components/Index.vue'),
  children: [{
    name: "个人信息 ",
    path: '/view/userinfo',
    component: () => import('../components/UserInfo.vue')
  },{
      name: "个人信息  ",
      path: '/view/userinfo/:blogname',
      component: () => import('../components/UserInfo.vue')
    },
    {
      name: "写文章",
      path: '/view/acticleEditor/:blogname',
      component: () => import('../components/ActicleEditor.vue')
    },
    {
      name: "文章",
      path: '/view/acticleList/:blogname',
      component: () => import('../components/ActicleList.vue')
    },
    {
      name: "文章视图",
      path: '/view/acticleView/:blogname',
      component: () => import('../components/ActicleView.vue')
    }
  ]
},
// 404页面路由
{
  path: '/:pathMatch(.*)*',
  name: 'NotFound',
  component: () => import('../components/NotFound.vue')
}
]

export default function (history) {
  return createRouter({
    history,
    routes
  })
}