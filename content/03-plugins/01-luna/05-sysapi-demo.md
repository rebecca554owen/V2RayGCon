---
title: "Sys库示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 5
---

需要V2RayGCon `v1.3.4.3`或以后版本

调用外部程序示例
```lua
stdin = "ping 8.8.8.8 -t"
proc = Sys:Run("cmd.exe", nil, stdin)
Sys:WaitForExit(proc)
```

互发信件示例（开2个窗口，同时运行以下脚本）
```lua
local name = "Alex"
local address = "Bob"

local Utils = require "libs.utils"

local mailbox = Sys:CreateMailBox(name)
if mailbox == nil then
    name, address = address, name
    mailbox = Sys:CreateMailBox(name)
end

function CheckMail()
    if mailbox:Count() < 1 then
        mailbox:Send(address, "1")
        print("Say hello to ", address)
    else
        local mail = mailbox:Check()
        if mail ~= nil then
            local c = mail:GetHeader()
            print(mail:GetAddress() , " said ", c) 
            local r = Utils.ToNumber(c) + 1
            mailbox:Reply(mail, tostring(r))
        end
    end
end

while not Signal:Stop() do
    CheckMail()
    Misc:Sleep(1000)
end
```


