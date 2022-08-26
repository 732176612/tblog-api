import{V as u,L as g,S as p}from"./interface.803c4557.js";import{_,r as f,o as m,d as v,e,n as c,w as l,i as d,y as x,z as y,B as P,F as b,C as k,D as w}from"./vendor.5dbb932c.js";import{_ as L}from"./logo.f0566f2c.js";import"./index.7cb485cd.js";const O={name:"Login",data(){return{PhoneOrMail:"",PassWord:"",PassWordRegex:"",AutoLogin:!0}},methods:{async GetVerifyRegex(){let t=(await u({regexName:"PassWord"})).Data;for(var s in t)t[s].Key=="PassWord"&&(this.PassWordRegex=t[s].Value)},async OnClickLoginUser(){if(this.PhoneOrMail==""){this.$toast.warning("\u8D26\u53F7\u4E0D\u80FD\u4E3A\u7A7A");return}if(!this.$refs.PassWord.checkValidity()){this.$toast.warning("\u5BC6\u7801\u683C\u5F0F\u6709\u8BEF\uFF0C\u8BF7\u68C0\u67E5");return}let t=await g({PassWord:this.PassWord,PhoneOrMail:this.PhoneOrMail});t!=null&&t.Status==200&&(this.$cookie.set("PhoneOrMail",this.PhoneOrMail),this.$cookie.set("AutoLogin",this.AutoLogin),this.Config.token=this.$cookie.get("token"),t.Data==""?this.$router.push("/view/userinfo"):this.$router.push("/view/index/"+t.Data))},async CheckAutoLogin(){let t=this.$cookie.get("AutoLogin");if(t!=null&&(this.AutoLogin=t,t==!0)){console.log("\u81EA\u52A8\u767B\u5F55\uFF01");let s=this.$cookie.get("token");if(s!=null){let a=await p({token:s});a.Data.BlogName!=null&&a.Data.BlogName!=""?this.$router.push("/view/index/"+a.Data.BlogName):this.$router.push("/view/userinfo")}}}},async mounted(){await this.CheckAutoLogin();let t=this.$cookie.get("PhoneOrMail");t!=null&&(this.PhoneOrMail=t),this.GetVerifyRegex()}},n=t=>(k("data-v-7db47a22"),t=t(),w(),t),M={class:"d-flex align-items-center justify-content-center",style:{height:"90%"}},A={class:"col-xl-3 col-lg-4 col-md-6 col-sm-10 col-10",style:{"max-width":"400px"}},W={class:"card",style:{"border-top":"2px solid var(--blue)"}},C=n(()=>e("div",{class:"card-header text-center"},[e("img",{class:"logoImg",src:L})],-1)),V={class:"card-body"},B=n(()=>e("p",{class:"h3 text-center"},"\u767B\u5F55",-1)),D=["pattern"],U=n(()=>e("div",{class:"invalid-feedback"}," \u8981\u540C\u65F6\u542B\u6709\u6570\u5B57\u548C\u5B57\u6BCD\uFF0C\u4E14\u957F\u5EA6\u8981\u57288-16\u4F4D\u4E4B\u95F4 ",-1)),R={class:"d-grid gap-2 mt-2 mb-2"},I={class:"row"},N={class:"col-4 offset-8 text-end"},S=n(()=>e("label",{for:"AutoLogin"},"\u81EA\u52A8\u767B\u5F55",-1)),j=n(()=>e("p",{class:"mb-1",style:{"font-size":"1rem"}},[e("a",{href:"/view/RecoverPwd"},"\u5FD8\u8BB0\u5BC6\u7801")],-1)),z=n(()=>e("p",{class:"mb-0",style:{"font-size":"1rem"}},[e("a",{href:"/view/register",class:"text-center"},"\u6CE8\u518C\u65B0\u8D26\u53F7")],-1)),$=n(()=>e("div",{class:"text-center",style:{height:"10%"}},[e("a",{href:"http://beian.miit.gov.cn",target:"_blank",class:"h6 mt-5 text-black-50"},"\xA9TBlog - \u7CA4ICP\u590720006712\u53F7\u3002")],-1));function K(t,s,a,T,o,r){const h=f("loadingbtn");return m(),v(b,null,[e("div",M,[e("div",A,[e("div",W,[C,e("div",V,[B,e("form",null,[e("div",{class:c(["input-group mb-3",o.PhoneOrMail.length!=0?"was-validated":""])},[l(e("input",{ref:"PhoneOrMail",type:"text",class:"form-control text-input",placeholder:"\u8D26\u53F7",autocomplete:"",required:"","onUpdate:modelValue":s[0]||(s[0]=i=>o.PhoneOrMail=i)},null,512),[[d,o.PhoneOrMail]])],2),e("div",{class:c(["input-group mb-3",o.PassWord.length!=0?"was-validated":""])},[l(e("input",{ref:"PassWord",type:"password",onKeyup:s[1]||(s[1]=x((...i)=>r.OnClickLoginUser&&r.OnClickLoginUser(...i),["enter"])),class:"form-control text-input",placeholder:"\u5BC6\u7801",pattern:o.PassWordRegex,"onUpdate:modelValue":s[2]||(s[2]=i=>o.PassWord=i),autocomplete:"",required:""},null,40,D),[[d,o.PassWord]]),U],2),e("div",R,[y(h,{class:"btn-block btn-primary",awaitAction:r.OnClickLoginUser,btnText:"\u767B\u5F55",style:{height:"3rem"}},null,8,["awaitAction"])]),e("div",I,[e("div",N,[l(e("input",{type:"checkbox",id:"AutoLogin","onUpdate:modelValue":s[3]||(s[3]=i=>o.AutoLogin=i)},null,512),[[P,o.AutoLogin]]),S])])]),j,z])])])]),$],64)}var J=_(O,[["render",K],["__scopeId","data-v-7db47a22"]]);export{J as default};
