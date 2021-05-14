---
title: "选项"
date: 2020-02-06T11:05:55+08:00
draft: false
weight: 18
---

选项窗口：
{{< figure src="../../images/forms/form_option_autorun.png" >}}

大部分选项都是所见即所得。除了。。。以下几个：  

###### 自动追踪
别慌，这个功能不会跟踪你的上网行为。它只是记住当前哪几个服务器在运行，重启之后帮你开启这些服务器。  

###### 流量统计(`v1.4.1.7+`)
无论你用的是Xray还V2Ray内核，都需要保留v2ctl.exe统计功能才生效。  
因v2ray-core的神奇设计，统计功能的配置很容易和服务器其他配置冲突，开启这个选项之后尽量不要改动服务器的默认配置。  
Luna插件提供了`coreState:SetDownlinkTotal(sizeInBytes)`等几个函数，可以用来实现每月清空统计数据之类的功能。  
测试发现直到v2ray-core v4.27.0为止，开启流量统计功能都会严重影响网速，如非必要不建议开启这个功能。  

###### 使用代理
这个选项和“订阅”以及“下载v2ray-core”里的“使用代理”选项是共用的，这里钩了之后那俩也会被钩上。  

###### 分页大小
当服务器数量多过分页大小时，主窗口底下会出现几个选页用的按钮。默认8个服务器每页的设置可以轻松管理300来个服务器。如果你有更多服务器。。。就分享几个出来吧。  

###### 解码模板
解码`vmess://...`链接时的outbound模板，样例参见[templates/custom/vmessDecodeTemplate.json](https://raw.githubusercontent.com/vrnobody/V2RayGCon/master/V2RayGCon/Resources/Files/templates/custom/vmessDecodeTemplate.json)  

###### 自定义inbound(默认值分页)
用于改写选中了"自定义"选项的服务器的inbounds设置。可以用来实现同时开放http/socks协议，设置http用户名、密码之类的功能。 

###### 多文件配置(分页)
V2RayGCon `v1.3.1.3+`开始支持v2ray-core `v4.23.1`新增的多文件配置功能。  

用法和[全局import](https://vrnobody.github.io/V2RayGCon/01-usage/17-global-import/)相近，在`选项`窗口的`多配置文件`分页中设定常用的配置文件路径（必须是完整的绝对路径）。然后就可以在`配置编辑器`里面把这些设定插入到`config.json`的`v2raygcon.configs`分节。其中`stdin:`表示当前整个`config.json`。CONFDIR功能可以通过插入`V2RAY_LOCATION_CONFDIR`环境变量来启用。  

提示：各配置文件/文件夹的合并顺序/合并规则是由v2ray-core决定的，注意看日志信息。  

###### 其他
`优先使用v4格式`是v2ray-core 3.x时代的遗产，现在钩上就是了。  
测速分页的`大小(KiB)`可以设置为零，此时下载的数据大于0就算作测速成功。  
`最大并发`数值越大越容易引发端口冲突，从而导致测速失败（超时），所以不要调太大。  