---
title: "ProxySetter"
date: 2020-02-01T17:58:23+08:00
draft: false
weight: 3
---

简单来说这个插件就是用来修改windows系统“Internet选项”里的代理设定的。  

{{< figure src="../../images/plugins/plugin_proxysetter.png" >}}
最右侧的“在记事本中...”和“在浏览器中...”对调试问题会有点帮助。  

基于以下几点原因，本项目不使用GFWList作为默认PAC源：
 1. “正版”GFWList已经很久没更新，很多网站已经不适用
 2. 其他人维护的GFWList多如牛毛，选哪个都会有人跳出来说不好
 3. GFWList使用正则来匹配域名的算法，已在其他（非v2ray相关）项目中显示出弊端

不过。。。这个插件有个“自定义PAC”功能，想用什么就用什么吧。  
 