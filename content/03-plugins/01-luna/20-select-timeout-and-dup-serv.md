---
title: "选中重复服务器"
date: 2020-02-15T22:47:09+08:00
draft: false
weight: 20
---

需要V2RayGCon `v1.3.8.4`  

```lua
local Set = require('lua.modules.set')

-- 超时（毫秒）
local SPEED_TEST_TIMEOUT = 99999

-- 忽略带以下标记的服务器
local ignoredMarks = Set.new({
    "PackageV4",
    "莫挨老子",
})

-- 重名时优先保留带以下标记的服务器
local specialMarks = Set.new({
    "VIP",
})

-- 代码
local caches = {}

function Main()
    local servs = Server:GetAllServers()
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        coreState:SetIsSelected(false)
        ProcServ(coreState)
    end
end

function ProcServ(coreState)
    local title = coreState:GetTitle()
    
    if ignoredMarks:Contains(coreState:GetMark()) then
        print("Ignore: ", title)
        return
    end
    
    if IsTimeoutServ(coreState) then
        print("Select timeout server: ", title)
        coreState:SetIsSelected(true)
        return
    end
    
    MarkDupServ(coreState, title)
end

function MarkDupServ(coreState, title)
    local summary = coreState:GetSummary()
    local cache = caches[summary]
    local ci = GetterCoreInfo(coreState)
     
    if cache == nil then
        print("Add to cache: ", title)
        caches[summary] = ci
        return
    end
    
    if IsSpecialMark(cache.mark) and not IsSpecialMark(ci.mark) then
        print("Select duplicated server: ", title)
        coreState:SetIsSelected(true)
        return
    end
    
    if not IsSpecialMark(cache.mark) and IsSpecialMark(ci.mark) then
        SwapCache(summary, ci)
        return
    end
    
    -- both (or neither) are special mark
    if ci.date > cache.date then
        print("Select duplicated server: ", title)
        coreState:SetIsSelected(true)
    else
        SwapCache(summary, ci)
    end  
end

function SwapCache(summary, newCoreInfo)
    print("Swap cached server: ", summary)
    SelectServerByUid(caches[summary].uid)
    caches[summary] = newCoreInfo
end

function SelectServerByUid(uid)
    local servs = Server:GetAllServers()
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        if coreState:GetUid() == uid then
            coreState:SetIsSelected(true)
        end
    end
end

function GetterCoreInfo(coreState)
    return {
        uid = coreState:GetUid(),
        mark = coreState:GetMark(),
        date = coreState:GetLastModifiedUtcTicks()
    }
end

function IsTimeoutServ(coreState)
    return coreState:GetSpeedTestResult() > SPEED_TEST_TIMEOUT
end

function IsSpecialMark(mark)
    return specialMarks:Contains(mark)
end

Main()
```