---
title: "所有函数"
date: 2020-02-02T19:13:26+08:00
draft: false
weight: 50
---

前面的示例脚本中像`Signal:Stop()`这样的语句，实际是调用了[ILuaSignal.cs][1]里面的`bool Stop();`在[Interfaces.Lua][2]目录下的所有接口都可以像上面那样调用。例如：`Misc:Sleep(1000)`

控制服务器的脚本，通常需要先调用`Server:GetAllServers()`函数。  
下面是一个选中所有ws.tls服务器的小脚本：  
```lua
local coreServs = Server:GetAllServers()
for coreServ in Each(coreServs) do
    local coreState = coreServ:GetCoreStates()
    local summary = coreState:GetSummary()
    local isWsTls = string.startswith(summary, "vmess.ws.tls@")
    coreState:SetIsSelected(isWsTls ~= false and isWsTls ~= nil)
end
```
 * Each()是预定义函数，源码在[LuaPredefinedFunctions.txt][4]文件中
 * coreServ的类型是[ICoreServCtrl][3]因函数太多，所以拆成4个模块。通过调用`Get***()`方法选用相应模块。每个模块在[CoreCtrlComponents][5]之中声明。

 注：上面代码使用`coreServ`，`coreState`这么奇怪的变量名是因为这两个关键字有代码提示。还有`coreLogger`，`coreConfiger`两个关键字也有代码提示。如果忘记了可以输入`core:`然后在提示列表中慢慢选。

 前面所有示例都是通过脚本控制服务器，其实也可以反过来，让脚本响应服务器事件：
 ```lua
local coreServ = Server:GetAllServers()[0]
local title = coreServ:GetCoreStates():GetTitle()
print(title)

local function OnCoreStart()
    print("core start: ", title)
end

local function OnCoreStop()
    print("core stop: ", title)
end

-- 把函数和事件绑定在一起
local hCoreStart = coreServ.OnCoreStart:Add(OnCoreStart)
local hCoreStop = coreServ.OnCoreStop:Add(OnCoreStop)

-- 启动、关闭第一个服务器时，上面绑定的函数就会执行
print("waiting")
while not Signal:Stop() do
    Misc:Sleep(1000)
end

-- 记得在脚本结束前解绑
coreServ.OnCoreStart:Remove(hCoreStart)
coreServ.OnCoreStop:Remove(hCoreStop)
print("done")
 ```
 如果脚本执行期间出错导致结束前没解绑，那么绑定的函数在脚本停止后还会被执行。  
 所以绑定事件须谨慎！

[1]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/Lua/ILuaSignal.cs "ILuaSignal.cs"
[2]: https://github.com/vrnobody/V2RayGCon/tree/master/VgcApis/Interfaces/Lua "Interfaces.Lua"
[3]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/ICoreServCtrl.cs "ICoreServCtrl.cs"
[4]: https://github.com/vrnobody/V2RayGCon/blob/master/Plugins/Luna/Resources/Files/LuaPredefinedFunctions.txt "LuaPredefinedFunctions.txt"
[5]: https://github.com/vrnobody/V2RayGCon/tree/master/VgcApis/Interfaces/CoreCtrlComponents "CoreCtrlComponents"