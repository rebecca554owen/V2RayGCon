---
title: "Sys库示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 5
---

需要V2RayGCon `v1.3.4.4`或以后版本

##### `Sys:Run()`用法示例
假设`trojan.exe`位于`V2RayGCon/trojan/`文件夹中。
```lua
-- 设定文件位置
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

##### `MailBox`用法示例
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

mailbox:Send(to, 1)
repeat
    local mail = mailbox:Wait()
    if mail ~= nil then
        local code = mail:GetCode()
        print(mail:GetAddress() , " said ", code)
        Misc:Sleep(800)
        mailbox:Reply(mail, code + 1)
    end
until mail == nil
```
小朋友，你是否有很多问号？这个`MailBox`有什么用呢？   