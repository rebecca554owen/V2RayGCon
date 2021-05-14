---
title: "各种分享链接"
date: 2020-02-01T22:27:56+08:00
draft: false
weight: 16
---

##### ss://...
仅支持`ss://(base64)#name`形式的分享链接  

##### trojan://...
仅支持[trojan-url](https://github.com/trojan-gfw/trojan-url)定义的分享链接标准（它不包含名字字段，所以导入的时候名字都是空白）  

##### v://...
这是本软件自创的一种分享链接。`简易编辑器`里的各种配置组合都可以用这种链接导入、导出。  
它又叫做vee链接，主要特点是短，编码思想出自v2ray-core [issue 1392][2]。具体实现可以看[VeeDecoder.cs][1]，不过代码是用Component的方式写的，比较散比较乱。  

##### v2cfg://...
这也是本软件自创的一种分享链接。它直接把整个config.json进行base64编码得出，主要用于备份/还原数据。因为v2ray功能过于强大，有可能被有心人利用，通过revers把本地端口暴露到公网，所以这种链接除了`主窗口`-`文件`-`从剪切板导入`外，其他地方都不能导入。  

##### vless://...
从`v1.5.2`起支持Xray-core [issues 91](https://github.com/XTLS/Xray-core/issues/91)提出的vless分享链接标准  
`v1.5.4`支持到`3月7日`的修订，即暂不支持`gRPC`传输类型  

##### vmess://...
仅支持v2rayN的vmess(ver2)分享链接，不支持其他vmess分享链接  


[1]: https://github.com/vrnobody/V2RayGCon/blob/master/V2RayGCon/Services/ShareLinkComponents/VeeDecoder.cs "VeeDecoder.cs"
[2]: https://github.com/v2ray/v2ray-core/issues/1392 "v2ray-core #1392"

