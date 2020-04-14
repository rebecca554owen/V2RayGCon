---
title: "调用外部程序示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 5
---

需要V2RayGCon `v1.3.4.2`或以后版本
```lua
cmd = "ping 8.8.8.8 -t"
proc = Sys:Run("cmd.exe", nil, true, cmd, nil)
Sys:WaitForExit(proc)
```


