---
title: "Sys库示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 5
---

##### `Sys:Start()`用法示例(`v1.6.9.0+`)
```lua
Sys:Start("https://www.baidu.com")
```

##### `Sys:Run()`用法示例 (`v1.3.5.0+`)
```lua
-- 假设trojan的位置是V2RayGCon/trojan/trojan.exe

local trojan = "trojan/trojan.exe"
local args = "-c trojan/config.json"

-- 代码
local proc = Sys:Run(trojan, args, nil, nil, false, true)
while not Signal:Stop() and not Sys:HasExited(proc) do
    Misc:Sleep(1000)
end
if not Sys:HasExited(proc) then
    Sys:SendStopSignal(proc)
    Sys:WaitForExit(proc)
end
```

##### `MailBox`用法示例 (`v1.3.8.4+`)
开2个窗口，同时运行以下脚本
```lua
local from = "Alex"
local to = "Bob"

local mailbox = Sys:CreateMailBox(from)
if mailbox == nil then
    from, to = to, from
    mailbox = Sys:CreateMailBox(from)
end
assert(mailbox ~= nil)

mailbox:SendCode(to, 1)
repeat
    local mail = mailbox:Wait()
    if mail ~= nil then
        local code = mail:GetCode()
        print(mail:GetAddress() , " said ", code)
        Misc:Sleep(800)
        mailbox:ReplyCode(mail, code + 1)
    end
until mail == nil
```
小朋友，你是否有很多问号？这个`MailBox`有什么用呢？   

##### `modules.hotkey` 自定义热键 (`v1.3.8.4+`)
基本步骤是先用`Reg`把快捷键和某个函数绑定  
然后按需选 `Check()`轮询 或者 `Wait()`阻塞 等待按键消息  
```lua
local hkMgr = require('lua.modules.hotkey').new()
    
local hkCfgs = {
    {"D5", function() Sys:Run("notepad.exe") end},
    {"D6", function() Sys:Run("cmd.exe") end},
    {"D7", function() Sys:Run("mspaint.exe") end},
}

for index, hkCfg in ipairs(hkCfgs) do
    if not hkMgr:Reg(hkCfg[1], hkCfg[2], true, true, false) then
        local msg = "注册热键[" .. hkCfg[1] .. "]失败"
        assert(false, msg)
    end
end

while not Signal:Stop() do
    if not hkMgr:Wait(1500) then
        print("请按 Ctrl + Alt + 5 或 6 或 7")
    end
end
hkMgr:Destroy()
```
其实`modules.hotkey`只是对`Sys`库中的`Sys:RegisterHotKey(...)`等几个函数简单包装了一下。  
如果你想硬核一点可以直接使用`Sys`库中的函数。具体代码参考`lua/modules/hotkey.lua`。  
  
小技巧：不知道键码（比如上面的 "D5", "D6", "D7"）的时候，可以打开ProxySetter插件来查。  
