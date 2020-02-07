---
title: "Luna"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 1
---

这个插件内置很多管理服务器的函数，通过lua脚本调用这些函数可以灵活的管理服务器，更新订阅等。

##### 简单脚本演示
{{< figure src="../../images/luna/luna_v0.1.0_print_v.png" >}}

##### 使用Signal响应停止按钮
{{< figure src="../../images/luna/luna_v0.1.0_signal_demo.gif" >}}

##### 修改服务器标记
{{< figure src="../../images/luna/demo_setmark_v1.2.8.4.gif" >}}
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
