---
title: "多个配置文件"
date: 2020-02-02T22:45:44+08:00
draft: false
weight: 20
---

V2RayGCon `v1.3.1.3+`开始支持v2ray-core `v4.23.1`新增的多配置文件功能。  

用法和[全局import](https://vrnobody.github.io/V2RayGCon/01-usage/17-global-import/)相近，在`选项`窗口的`多配置文件`分页中设定常用的配置文件路径（必须是完整的绝对路径）。然后就可以在`配置编辑器`里面把这些设定插入到`config.json`的`v2raygcon.configs`分节。  

其中`stdin:`表示当前整个`config.json`  
CONFDIR功能可以通过插入`V2RAY_LOCATION_CONFDIR`环境变量来启用  

提示：各配置文件/文件夹的合并顺序/合并规则是由v2ray-core决定的，注意看日志信息。  