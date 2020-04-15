---
title: "Sys库示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 5
---

需要V2RayGCon `v1.3.4.4`或以后版本

调用外部程序示例
```lua
stdin = "ping 8.8.8.8 -t"
proc = Sys:Run("cmd.exe", nil, stdin)
Sys:WaitForExit(proc)
```


互发信件示例（开2个窗口，同时运行以下脚本）
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
小朋友，你是否有很多问号？  
这个功能大概可以用来写些c/s结构的脚本（们）。  