---
title: "Luna"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 10
---

这是一个NLua脚本执行器插件。内置很多管理服务器的函数，可以用lua脚本来自动化订阅、测速等服务器管理工作。  
自`v1.8.3`起Luna从内置插件中移出，变成外置第三方插件。预计`v1.8.5`将停止发布Luna插件。  
现在建议使用NeoLuna插件，用法和Luna相近，所以这里的文档还保留。  

##### 简单脚本演示
{{< figure src="../../images/luna/luna_v0.4.8_print_v.png" >}}

##### 使用Signal响应停止按钮
{{< figure src="../../images/luna/luna_v0.4.8_signal_demo.gif" >}}

##### 修改服务器标记
{{< figure src="../../images/luna/demo_setmark_v1.5.3.4.gif" >}}
脚本源码如下：
```lua
local coreServ = Server:GetAllServers()[0]
local coreState = coreServ:GetCoreStates()
local counter = 0
while not Signal:Stop() do
    counter = counter + 1
    coreState:SetMark(tostring(counter))
    Misc:Sleep(1000)
end
```

##### 小技巧
Luna插件只能在64位系统中使用，如果想在32位系统中使用，需要把`libs/x86/lua53.dll`复制出来，替换掉`libs/lua53.dll`。
