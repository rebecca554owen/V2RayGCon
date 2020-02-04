---
title: "每天更新订阅"
date: 2020-02-02T17:17:33+08:00
draft: false
weight: 10
---

```lua
-- 设定更新间隔（单位:天）
local updateInterval = 1 

-- 代码
local TimestampKey = "TimestampOfLastSubscriptionUpdate"
local SecPerDay = 24 * 60 * 60
local TimeZone8 = 8 * 60 * 60

function Main()

    local timestamp = Str2Num(Misc:ReadLocalStorage(TimestampKey))
    local now = Str2Num(os.time())
    if DayDiff(now, timestamp) < updateInterval then
        print("Already updated today! Abort!")
        return
    end
    
    print("Update subscriptions")
    local proxyPort = Web:GetProxyPort()
    local count = Web:UpdateSubscriptions(proxyPort)
    print("Got", count, "new servers.")
    Server:UpdateAllSummary()
    Misc:WriteLocalStorage(TimestampKey,tostring(os.time()))
    print("Done!")
end

function DayDiff(tickLeft, tickRight)
    local dayL = CalcTotalDays(tickLeft)
    local dayR = CalcTotalDays(tickRight)   
    return math.abs(dayL - dayR)
end

function CalcTotalDays(ticks)
    local ticks8 = ticks + TimeZone8
    local days = ticks8 / SecPerDay
    return tostring(math.floor(days))
end

function Str2Num(str)
    if str == nil or str == '' then
        return 0
    end
    return tostring(str)
end

Main()
```
记得在“概况”页面钩上“自启动”哦！