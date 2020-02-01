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

##### 通过脚本控制主窗口
{{< figure src="../../images/luna/luna_v0.1.0_folding_demo.gif" >}}
脚本源码如下：
```lua
local coreServs = Server:GetAllServers()
for coreServ in Each(coreServs) do
    local coreState = coreServ:GetCoreStates()
    local folding = coreState:GetFoldingState()
    coreState:SetFoldingState((folding + 1) % 2)
end
```
