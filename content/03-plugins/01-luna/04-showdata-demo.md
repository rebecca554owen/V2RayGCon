---
title: "数据视图控件演示"
date: 2020-04-03T21:05:22+08:00
draft: false
weight: 4
---

需要V2RayGCon `v1.3.3.0`或以后版本
```lua
-- Misc:ShowData() 示例

local Utils = require "libs.utils"

function Main()
    local selected = ShowAllServers()
    if selected ~= nil then
        ShowResult(selected)
    end
end

function ShowResult(rows)
    print("选中:")
    for row in Each(rows) do
        print(row[0] .. "." .. row[1])
    end
end

function ShowAllServers()
    local rows = {}
    for coreServ in Each(Server:GetAllServers()) do
        local coreState = coreServ:GetCoreStates()
        local row = {
            coreState:GetIndex(),
            coreState:GetLongName(),
            coreState:GetSummary(),
            coreState:GetMark(),
            Utils.ToLuaDate(coreState:GetLastModifiedUtcTicks()),
            coreState:GetStatus(),
        }
        table.insert(rows, row)
    end
    local columns = {"序号", "名称", "摘要", "标记", "修改日期", "测速"}
    return Misc:ShowData("服务器列表:", columns, rows, 3)
end

Main()
```