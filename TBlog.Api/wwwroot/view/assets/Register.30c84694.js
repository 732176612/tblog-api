import{V,C as p,R as v,a as _}from"./interface.05da88ba.js";import{_ as g,r as m,o as P,d as b,e as s,w as r,i as d,n as h,t as u,z as f,C,D as M}from"./vendor.5dbb932c.js";import{_ as y}from"./logo.f0566f2c.js";import"./index.1ee5bce1.js";const T={data(){return{VCodeTip:"\u53D1\u9001\u9A8C\u8BC1\u7801",PhoneRegex:"",MailRegex:"",PassWordRegex:"",Phone:"",PassWord:"",Mail:"",VCode:"",IsSumbit:"",MailInVailTip:"",PhoneInVailTip:""}},methods:{async GetVerifyRegex(){let e=(await V({regexName:"Phone,Mail,PassWord"})).Data;for(var i in e)e[i].Key=="Phone"?this.PhoneRegex=e[i].Value:e[i].Key=="Mail"?this.MailRegex=e[i].Value:e[i].Key=="PassWord"&&(this.PassWordRegex=e[i].Value)},async CheckHavePhone(){if(this.Phone.length!=0)if(this.$refs.Phone.checkValidity()){var e=await p({phoneOrMail:this.Phone});e!=null&&e.Status==500?this.PhoneInVailTip="\u8BE5\u624B\u673A\u53F7\u7801\u5DF2\u88AB\u6CE8\u518C":this.PhoneInVailTip=""}else this.PhoneInVailTip="\u624B\u673A\u53F7\u7801\u683C\u5F0F\u4E0D\u6B63\u786E";else this.PhoneInVailTip=""},async CheckHaveMail(){if(this.Mail.length!=0)if(this.$refs.Mail.checkValidity()){var e=await p({phoneOrMail:this.Mail});e!=null&&e.Status==500?this.MailInVailTip="\u8BE5\u90AE\u7BB1\u5DF2\u88AB\u6CE8\u518C":this.MailInVailTip=""}else this.MailInVailTip="\u90AE\u7BB1\u683C\u5F0F\u4E0D\u6B63\u786E";else this.MailInVailTip=""},async SendVCode(){if(this.CheckPhoneOrMail()==!1){this.$toast.warning("\u8BF7\u8F93\u5165\u6B63\u786E\u7684\u90AE\u7BB1\u6216\u624B\u673A\u53F7\u7801");return}if(this.VCodeTip=="\u53D1\u9001\u9A8C\u8BC1\u7801"){let i=this.$refs.Phone.checkValidity()?this.Phone:this.Mail,a=await v({phoneOrMail:i});a!=null&&a.Status==200&&this.Countdown()}else this.$toast.info("\u9A8C\u8BC1\u7801\u5DF2\u53D1\u9001\uFF0C\u8BF7\u8010\u5FC3\u7B49\u5019")},async Countdown(){if(this.VCodeTip=="\u53D1\u9001\u9A8C\u8BC1\u7801")this.VCodeTip=60,this.Countdown();else if(this.VCodeTip>0){let e=this;setTimeout(()=>{e.VCodeTip--,e.Countdown()},1e3)}else this.VCodeTip="\u53D1\u9001\u9A8C\u8BC1\u7801"},async RequestRegisterUser(){if(this.IsSumbit=!0,this.CheckPhoneOrMail()==!1||this.$refs.PassWord.checkValidity()==!1||this.$refs.VCode.checkValidity()==!1)return;let e={email:this.Mail,phone:this.Phone,password:this.PassWord,vCode:this.VCode},i=await _(e);if(i!=null&&i.Status==200){let a=this;this.$toast.info("\u5C06\u57283\u79D2\u540E\u8DF3\u8F6C\u81F3\u767B\u9646\u9875\u9762"),setTimeout(()=>{a.$router.push("/view/login")},3e3)}},CheckPhoneOrMail(){if(this.PhoneInVailTip==""&&this.MailInVailTip==""){let e=this.$refs.Phone.checkValidity(),i=this.$refs.Mail.checkValidity();return!e&&!i?(this.$toast.warning("\u8BF7\u8F93\u5165\u6B63\u786E\u7684\u624B\u673A\u6216\u90AE\u7BB1"),!1):!0}else return!1}},mounted(){this.GetVerifyRegex()}},l=e=>(C("data-v-5bc477fc"),e=e(),M(),e),x={class:"container-fluid h-100"},I={class:"row align-items-center justify-content-center",style:{height:"95vh"}},k={class:"col-xl-4 col-lg-4 col-md-6 col-sm-8 col-10"},w={class:"card"},R=l(()=>s("div",{class:"card-header text-center",style:{"border-top":"2px solid var(--blue)"}},[s("img",{class:"logoImg",src:y})],-1)),S={class:"card-body"},W=l(()=>s("p",{class:"login-box-msg h3 text-center"},"\u6CE8\u518C",-1)),q={class:"needs-validation"},H={class:"input-group mb-3"},O=["pattern"],U={class:"invalid-feedback"},j={class:"input-group mb-3"},A=["pattern"],B={class:"invalid-feedback"},D=["pattern"],K=l(()=>s("div",{class:"invalid-feedback"}," \u8981\u540C\u65F6\u542B\u6709\u6570\u5B57\u548C\u5B57\u6BCD\uFF0C\u4E14\u957F\u5EA6\u8981\u57288-16\u4F4D\u4E4B\u95F4 ",-1)),N={class:"input-group-prepend"},z=l(()=>s("div",{class:"invalid-feedback"}," \u8BF7\u8F93\u5165\u6B63\u786E\u7684\u9A8C\u8BC1\u7801 ",-1)),G={class:"d-grid gap-2 text-center mt-2 mb-3"},E=l(()=>s("p",{class:"mb-0"},[s("a",{href:"/view/login",class:"text-center"},"\u6211\u5DF2\u7ECF\u6709\u4E86\u8D26\u53F7")],-1)),F=l(()=>s("div",{class:"mt-2 text-center",style:{height:"5vh"}},[s("a",{href:"http://beian.miit.gov.cn",target:"_blank",class:"h6 mt-5 text-black-50"},"\xA9TBlog - \u7CA4ICP\u590720006712\u53F7\u3002")],-1));function J(e,i,a,L,t,n){const c=m("loadingbtn");return P(),b("div",x,[s("div",I,[s("div",k,[s("div",w,[R,s("div",S,[W,s("form",q,[s("div",H,[r(s("input",{ref:"Mail",type:"text",class:h(["form-control",t.Mail.length!=0?t.MailInVailTip==""?"is-valid":"is-invalid":""]),placeholder:"\u90AE\u7BB1",pattern:t.MailRegex,"onUpdate:modelValue":i[0]||(i[0]=o=>t.Mail=o),onInput:i[1]||(i[1]=(...o)=>n.CheckHaveMail&&n.CheckHaveMail(...o)),required:""},null,42,O),[[d,t.Mail]]),s("div",U,u(t.MailInVailTip),1)]),s("div",j,[r(s("input",{ref:"Phone",type:"text",class:h(["form-control",t.Phone.length!=0?t.PhoneInVailTip==""?"is-valid":"is-invalid":""]),placeholder:"\u624B\u673A",pattern:t.PhoneRegex,"onUpdate:modelValue":i[2]||(i[2]=o=>t.Phone=o),onInput:i[3]||(i[3]=(...o)=>n.CheckHavePhone&&n.CheckHavePhone(...o)),required:""},null,42,A),[[d,t.Phone]]),s("div",B,u(t.PhoneInVailTip),1)]),s("div",{class:h(["input-group mb-3",t.IsSumbit||t.PassWord.length!=0?"was-validated":""])},[r(s("input",{ref:"PassWord",type:"password",class:"form-control",placeholder:"\u5BC6\u7801",pattern:t.PassWordRegex,"onUpdate:modelValue":i[4]||(i[4]=o=>t.PassWord=o),required:""},null,8,D),[[d,t.PassWord]]),K],2),s("div",{class:h(["input-group mb-3",t.IsSumbit||t.VCode.length!=0?"was-validated":""])},[s("div",N,[f(c,{class:"btn-main",style:{"background-color":"var(--orange)","border-color":"var(--orange)"},awaitAction:n.SendVCode,btnText:t.VCodeTip},null,8,["awaitAction","btnText"])]),r(s("input",{ref:"VCode",type:"text",class:"form-control",pattern:"\\d{4}$","onUpdate:modelValue":i[5]||(i[5]=o=>t.VCode=o),required:""},null,512),[[d,t.VCode]]),z],2)]),s("div",G,[f(c,{class:"btn-block btn-main",awaitAction:n.RequestRegisterUser,btnText:"\u6CE8\u518C"},null,8,["awaitAction"])]),E])]),F])])])}var $=g(T,[["render",J],["__scopeId","data-v-5bc477fc"]]);export{$ as default};