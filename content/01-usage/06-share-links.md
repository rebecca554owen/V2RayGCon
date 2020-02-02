---
title: "各种分享链接"
date: 2020-02-01T22:27:56+08:00
draft: false
weight: 6
---

这个软件支持v2rayN的vmess(ver2)链接和标准的ss链接。然后还自创了`v2cfg://...`和`v://...`两种链接。其中`v2cfg://...`是简单的把整个config.json进行base64编码得出，主要用于备份/还原数据。`v://...`又名vee链接，主要特点是短，但编码方式很复杂只是写来玩的。  

vee链接编码原理是把vmess/ss中的数据转成二进制串，然后对其进行base64编码。为了缩短链接长度，不同数据编码方式有所不同。比如server地址如果是域名就当字符串处理，如果是IP则要看是存字符串省空间还是存byte串省空间。对一些常用的字符串比如ss的几种加密方式，vmess的几种streamSettings则通过查表只存下标号。最后还会加点版本信息和校验信息。具体可以看[VeeDecoder.cs][1]不过代码是用Component的方式写的，分得比较散，比较难明白。  

编码思想出自v2ray-core [issue 1392][2]  

[1]: https://github.com/vrnobody/V2RayGCon/blob/master/V2RayGCon/Services/ShareLinkComponents/VeeDecoder.cs "VeeDecoder.cs"
[2]: https://github.com/v2ray/v2ray-core/issues/1392 "v2ray-core #1392"

