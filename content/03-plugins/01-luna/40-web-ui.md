---
title: "Web UI"
date: 2020-02-25T13:40:01+08:00
draft: false
weight: 40
---

这是本软件的一个Web界面，需要V2RayGCon v1.7.0+  
项目地址：[https://github.com/vrnobody/WebUI](https://github.com/vrnobody/WebUI)  
我没什么设计天赋，欢迎PR。  

#### 使用方法
V2RayGCon v1.8+之后WebUI迁移到NeoLuna插件中，在NeoLuna插件中运行以下脚本：  
```lua
loadfile('3rd/neolua/webui/server.lua')()
```

V2RayGCon v1.7.1及以前版本，在Luna插件中运行以下脚本：  
```lua
local url = 'http://localhost:4000/'
loadfile('./lua/webui/server.lua')(url)
``` 
然后在浏览器中访问[http://localhost:4000/](http://localhost:4000/)  

#### light主题：  
{{< figure src="../../../images/luna/web_ui_light_v0.0.2.0.png" >}}

#### dark主题：  
{{< figure src="../../../images/luna/web_ui_dark_v0.0.2.0.png" >}}

#### 运行lua脚本：  
{{< figure src="../../../images/luna/web_ui_luna_print_w.png" >}}

小技巧：  
WinForm界面的选项窗口-设置-托盘单击中输入'http://localhost:4000'可以实现点下图标就打开WebUI。  
上面这个设置项也可以填快捷方式(.lnk)的路径，这个玩法就更多了。  
更多用法请看[WebUI项目](https://github.com/vrnobody/WebUI)的readme，那里更新会比这快一点。  