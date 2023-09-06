---
title: "全部函数"
date: 2020-02-02T19:13:26+08:00
draft: false
weight: 50
---

前面的示例脚本中像`Signal:Stop()`这样的语句，实际是调用了[ILuaSignal.cs][1]里面的`bool Stop();`在[Interfaces][2]目录下的所有接口都可以像上面那样调用。例如：`Misc:Sleep(1000)`

控制服务器的脚本，通常从调用`Server:GetAllServers()`函数开始。  
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
其中：  
 * Each()是预定义函数，源码在[LuaPredefinedFunctions.txt][4]文件中
 * coreServ的类型是[ICoreServCtrl][3]，分为4个模块，通过调用`Get***()`方法选用相应模块。每个模块在[CoreCtrlComponents][5]之中声明。

上面代码使用`coreServ`，`coreState`这么奇怪的变量名是因为这两个关键字有代码提示。还有`coreLogger`，`coreConfiger`两个关键字也有代码提示。V2RayGCon v1.8+引入了一个新的关键字wserv简化各模块调用，例如：
```lua
-- 旧写法（重启第1个服务器）
local coreServ = Server:GetServerByIndex(1)
local coreState = coreServ:GetCoreStates()
local coreCtrl = coreServ:GetCoreCtrl()
local title = coreState:GetTitle()
print(title)
coreCtrl:RestartCore()

-- 新写法
local wserv = Server:GetServerByIndex(1):Wrap()
local title = wserv:GetTitle()
print(title)
wserv:RestartCore()
```
Wrap()的作用是把一个coreServ包装成一个[IWrappedCoreServCtrl](https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/IWrappedCoreServCtrl.cs)，然后可以省掉coreServ:GetCoreStates()这些步骤，直接调用各模块里面的函数。wserv:Unwrap()可以还原出coreServ。这层包装有性能损耗，一方面性能损耗巨大（多用一倍时间），另一方面性能损耗微乎其微（50万次函数调用才多用1秒，因为每次调用只有2ns），所以想用就用吧，不用太在意性能。  

[1]: https://github.com/vrnobody/V2RayGCon/blob/master/3rd/Luna/Interfaces/ILuaSignal.cs "ILuaSignal.cs"
[2]: https://github.com/vrnobody/V2RayGCon/tree/master/3rd/Luna/Interfaces "Interfaces"
[3]: https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/ICoreServCtrl.cs "ICoreServCtrl.cs"
[4]: https://github.com/vrnobody/V2RayGCon/blob/master/3rd/Luna/Resources/Files/LuaPredefinedFunctions.txt "LuaPredefinedFunctions.txt"
[5]: https://github.com/vrnobody/V2RayGCon/tree/master/VgcApis/Interfaces/CoreCtrlComponents "CoreCtrlComponents"