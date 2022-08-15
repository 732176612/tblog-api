import{G as p,d as _,S as v,e as C}from"./interface.0cd745a2.js";import{_ as k,r as w,o as t,d as n,e as s,t as f,F as b,E as N,n as u,G as r,z as y,C as S,D as x}from"./vendor.b0a0702d.js";import"./index.e0dd1f6b.js";const I={name:"Index",data(){return{UserDto:{BlogName:"",UserName:"",IsInit:!1},Menus:[],NavClass:"bg-main"}},methods:{async InitMenus(){let e=await p();e!=null&&e.Status==200&&(this.Menus=e.Data)},async InitUserInfo(){let e=await _(this.$route.params.blogname);if(e!=null&&e.Status==200){let a=e.Data;this.UserDto.UserName=a.UserName,this.UserDto.BlogName=a.BlogName,a.StyleColor!=null&&a.StyleColor!=""&&ChangeStyleColor(a.StyleColor)}else this.$cookie.set("AutoLogin","false"),this.$router.push("/view/login")},async RefreshUserSelf(){if(this.Config.token!=""){let e=await v({token:this.Config.token});e.Data.BlogName!=null&&e.Data.BlogName!=""&&(this.Config.userSelf=e.Data)}},OnClickLogOut(){C(),this.Config.token="",this.$cookie.remove("token"),this.$router.push("/view/login")},OnClickMenuBtn(e){location.href=e.Url+"/"+this.$route.params.blogname},async CheckToken(){this.Config.token=getCookie("token"),await this.RefreshUserSelf(),this.$route.params.blogname==null?this.Config.token==""?(this.$toast.warning("\u60A8\u8FD8\u672A\u767B\u9646\uFF0C\u8BF7\u767B\u9646"),await this.$router.push("/view/login")):this.Config.userSelf.BlogName!=null&&this.Config.userSelf.BlogName!=""?await this.$router.push("/view/index/"+this.Config.userSelf.BlogName):await this.$router.push("/view/userinfo"):this.IsInit||(await this.InitUserInfo(),await this.InitMenus())}},beforeRouteEnter(e,a,c){c(async h=>{let o=h;await o.CheckToken(),o.$route.path.indexOf("index")==-1||window.scrollY>400?o.NavClass="bg-main":o.NavClass=""})},mounted(){setInterval(()=>{this.$route.path.indexOf("index")==-1||window.scrollY>400?this.NavClass="bg-main":this.NavClass=""},100)}},l=e=>(S("data-v-7eff22a6"),e=e(),x(),e),B={class:"d-flex h-100 mx-auto flex-column"},U={key:0},$={class:"container-xl"},D={class:"navbar-brand",href:"#"},O=l(()=>s("button",{class:"navbar-toggler",type:"button","data-bs-toggle":"collapse","data-bs-target":"#navbarsExample07XL",style:{border:"3px solid blue"},"aria-controls":"navbarsExample07XL","aria-expanded":"false","aria-label":"Toggle navigation"},[s("span",{class:"navbar-toggler-icon"})],-1)),L={class:"collapse navbar-collapse",id:"navbarsExample07XL"},M={class:"navbar-nav me-auto"},E=["onClick"],X={key:0},G=l(()=>s("a",{class:"nav-link",href:"/view/login"},"\u767B\u9646",-1)),T=[G],j={key:1},z=["href"],R={key:2,class:"nav-item dropdown"},V=l(()=>s("a",{class:"nav-link dropdown-toggle",href:"#",id:"dropdown07XL","data-bs-toggle":"dropdown","aria-expanded":"false"},"\u4E2A\u4EBA\u4E2D\u5FC3",-1)),F={key:0,class:"dropdown-menu","aria-labelledby":"dropdown07XL"},Y=["href"],A=["href"],J=l(()=>s("li",null,[s("hr",{class:"dropdown-divider"})],-1)),P={role:"main",id:"main",style:{height:"auto"}},q={class:"h-100"},H=l(()=>s("footer",{class:"mastfoot mt-auto border-top"},[s("div",{class:"inner text-center py-2"},[s("a",{href:"http://beian.miit.gov.cn",target:"_blank",class:"h6 mt-5 text-black-50"},"\xA9TBlog - \u7CA4ICP\u590720006712\u53F7\u3002")])],-1));function K(e,a,c,h,o,d){const g=w("router-view");return t(),n("div",B,[o.UserDto.BlogName!=""?(t(),n("header",U,[s("nav",{class:u(["navbar navbar-expand-sm navbar-dark fixed-top",o.NavClass]),"aria-label":"Ninth navbar example"},[s("div",$,[s("a",D,f(o.UserDto.BlogName),1),O,s("div",L,[s("ul",M,[(t(!0),n(b,null,N(o.Menus,(i,m)=>(t(),n("li",{class:"nav-item",key:m},[s("a",{class:u(["nav-link",e.$route.name.indexOf(i.Name)!=-1?"active":""]),href:"#",onClick:Q=>d.OnClickMenuBtn(i)},f(i.Name),11,E)]))),128)),e.Config.token==""?(t(),n("li",X,T)):r("",!0),e.Config.token!=""&&e.isSelf(e.$route)==!1?(t(),n("li",j,[s("a",{class:"nav-link",href:"/view/index/"+e.Config.userSelf.BlogName},"\u56DE\u5230\u6211\u7684\u535A\u5BA2",8,z)])):r("",!0),e.Config.token!=""?(t(),n("li",R,[V,(t(),n("ul",F,[s("li",null,[s("a",{class:"dropdown-item",href:"/view/acticleEditor/"+e.Config.userSelf.BlogName},"\u5199\u6587\u7AE0",8,Y)]),s("li",null,[s("a",{class:"dropdown-item",href:"/view/userInfo/"+e.Config.userSelf.BlogName},"\u4E2A\u4EBA\u4FE1\u606F",8,A)]),J,s("li",null,[s("a",{class:"dropdown-item",onClick:a[0]||(a[0]=(...i)=>d.OnClickLogOut&&d.OnClickLogOut(...i))},"\u6CE8\u9500")])]))])):r("",!0)])])])],2)])):r("",!0),s("main",P,[s("div",q,[y(g)])]),H])}var se=k(I,[["render",K],["__scopeId","data-v-7eff22a6"]]);export{se as default};
