---
title: "Sys库示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 5
---



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

##### `MailBox`用法示例 (`v1.3.5.0+`)
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

##### `modules.hotkey` 自定义热键 (`v1.3.6.0+`)
基本步骤是先用`Reg`把快捷键和某个函数绑定  
然后按需选`Check()`轮询或`Wait()`阻塞等待按键消息  
```lua
local HotKey = require "modules.hotkey"

function Main()
    local kfs = {
        { "D5", function() print("5") end},
        { "D6", function() print("6") end},
        { "D7", function() print("7") end},
    }
    
    local hk = HotKey()
    for idx, kf in ipairs(kfs) do
        if not hk:Reg(kf[1], kf[2], true, true, false) then
            assert(false, "注册热键[" .. kf[1] .. "]失败")
        end
    end
    
    while not Signal:Stop() do
        if not hk:Wait(1500) then
            print("请按Ctrl + Alt + 5, 6 或 7")
        end
    end
    hk:Destroy()
    print("done")
end

Main()
```
其实`modules.hotkey`只是对`Sys`库中的`Sys:RegisterHotKey(...)`等几个函数简单包装了一下。  
如果你想硬核一点可以直接使用`Sys`库中的函数。具体代码看`lua/modules/hotkey.lua`  
