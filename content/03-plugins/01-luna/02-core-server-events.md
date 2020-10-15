---
title: "监听服务器事件"
date: 2020-02-02T19:13:26+08:00
draft: false
weight: 02
---

V2RayGCon `v1.4.3+`

服务器启动、停止时会产生相应的事件。你可以通过加载`lua.modules.coreEvent`模块来监听这些事件。  

 ```lua
local cev = require('lua.modules.coreEvent').new()

local coreServ = Server:GetAllServers()[0]
local title = coreServ:GetCoreStates():GetTitle()

local function OnCoreStart() print("core start: ", title) end
local function OnCoreStop() print("core stop: ", title) end
local function OnCorePropertyChanged() print("core property changed: ", title) end

cev:RegEvStart(coreServ, OnCoreStart)
cev:RegEvStop(coreServ, OnCoreStop)
cev:RegEvPropertyChanged(coreServ, OnCorePropertyChanged)

print("server: ", title)
while not Signal:Stop() do
    cev:Wait(1000)
end
```

如果你想硬核一点，也可以不用模块，手动绑定事件：
```lua
local coreServ = Server:GetAllServers()[0]
local title = coreServ:GetCoreStates():GetTitle()

function OnCoreStart() print("core start: ", title) end
local handle = coreServ.OnCoreStart:Add(OnCoreStart)
-- OnCoreStop 同理

print("server: ", title)
while not Signal:Stop() do
   Misc:Sleep(1000)
end

-- 记得在脚本结束前解除绑定，不然绑定的函数还会触发
coreServ.OnCoreStart:Remove(handle)
```

[1]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/Lua/ILuaSignal.cs "ILuaSignal.cs"
[2]: https://github.com/vrnobody/V2RayGCon/tree/master/VgcApis/Interfaces/Lua "Interfaces.Lua"
[3]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/ICoreServCtrl.cs "ICoreServCtrl.cs"
[4]: https://github.com/vrnobody/V2RayGCon/blob/master/Plugins/Luna/Resources/Files/LuaPredefinedFunctions.txt "LuaPredefinedFunctions.txt"
[5]: https://github.com/vrnobody/V2RayGCon/tree/master/VgcApis/Interfaces/CoreCtrlComponents "CoreCtrlComponents"