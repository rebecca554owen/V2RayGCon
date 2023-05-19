---
title: "Web UI"
date: 2020-02-25T13:40:01+08:00
draft: false
weight: 40
---

这是本软件的一个Web界面，需要V2RayGCon v1.6.9.3+  
项目地址：[https://github.com/vrnobody/WebUI](https://github.com/vrnobody/WebUI)  
我没什么设计天赋，欢迎PR。  

light主题：  
{{< figure src="../../../images/luna/web_ui_light_v0.0.1.1.png" >}}

dark主题：  
{{< figure src="../../../images/luna/web_ui_dark_v0.0.1.1.png" >}}

在Luna插件中新建一个脚本，运行以下命令即可：  
```lua
local serv = './lua/webui/server.lua'
loadfile(serv)()
-- 也可以传入参数 loadfile(serv)("http://localhost:1234/") 
``` 

Web UI中也可以运行lua脚本：  
{{< figure src="../../../images/luna/web_ui_luna_print_w.png" >}}