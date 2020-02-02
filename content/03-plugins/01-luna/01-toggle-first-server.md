---
title: "开关第一个服务器"
date: 2020-02-02T22:13:01+08:00
draft: false
weight: 1
---

```lua
local coreServs = Server:GetAllServers()
for coreServ in Each(coreServs) do
    local coreState = coreServ:GetCoreStates()
    if coreState:GetIndex() == 1 then
        local title = coreState:GetTitle()
        local coreCtrl = coreServ:GetCoreCtrl()
        local isRunning = coreCtrl:IsCoreRunning()
        if isRunning then
            print("Stop ", title)
            coreCtrl:StopCore()
        else
            print("Start ", title)
            coreCtrl:RestartCore()
        end
    end
end
```

