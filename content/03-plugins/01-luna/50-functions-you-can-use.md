---
title: "所有函数"
date: 2020-02-02T19:13:26+08:00
draft: false
weight: 50
---

前面的示例脚本中像`Signal:Stop()`这样的语句，实际是调用了[ILuaSignal.cs][1]里面的`bool Stop();`[Interfaces.Lua][2]目录里面的所有接口都可以像上面那样调用。例如：`Misc:Sleep(1000)`

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

 注：上面代码使用`coreServ`，`coreState`这么奇怪的变量名是因为这两个关键字有代码提示。还`coreLogger`，`coreConfiger`两个关键字也有代码提示。如果忘记了可以输入`core:`然后在提示列表中慢慢选。

[1]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/Lua/ILuaSignal.cs "ILuaSignal.cs"
[2]: https://github.com/vrnobody/V2RayGCon/tree/master/VgcApis/Interfaces/Lua "Interfaces.Lua"
[3]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/ICoreServCtrl.cs "ICoreServCtrl.cs"
[4]: https://github.com/vrnobody/V2RayGCon/blob/master/Plugins/Luna/Resources/Files/LuaPredefinedFunctions.txt "LuaPredefinedFunctions.txt"
[5]: https://github.com/vrnobody/V2RayGCon/tree/master/VgcApis/Interfaces/CoreCtrlComponents "CoreCtrlComponents"