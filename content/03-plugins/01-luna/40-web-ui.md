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
在Luna插件中新建一个脚本，运行以下命令，然后打开浏览器。  
```lua
local url = 'http://localhost:4000/'
loadfile('./lua/webui/server.lua')(url)
``` 
注：v0.0.3的默认密码是`123456`（发布时忘了删密码，抱歉！）  

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