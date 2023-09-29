---
title: "导入链接选项"
date: 2020-02-01T17:39:27+08:00
draft: false
weight: 15
---

选项窗口的`默认值`分页可以修改导入链接的默认配置。
{{< figure src="../../images/forms/form_option_defaults.png" >}}
`默认模式`对应`Inbounds`分页的配置名字，软件自带了config, http, socks三种配置，可以自行修改、添加更多配置  
`默认core`对应`多内核`分页的配置名字  
`解码模板`用于修改（仅限）vmess协议的outbounds配置。具体参考[vmess outbound模板](https://raw.githubusercontent.com/vrnobody/V2RayGCon/master/V2RayGCon/Resources/Files/templates/custom/vmessDecodeTemplate.json)  

##### Inbounds(分页)
{{< figure src="../../images/forms/form_custom_inbound_editor.png" >}}
上面是http配置模板示例。模板中的`%host%`和`%port%`将会替换成`默认值`分页中的`默认地址`。模板不一定是json，也可以是yaml或者任意text。不过v1.8.5对yaml的支持比较弱鸡，只会简单的替换第一层key。而text类型的模板则直接插入到config的顶部。这样看文字比较抽象，建议自己添加几个不同类型的Inbounds，然后看服务器最终配置。注意模板的类型要和服务器config的类型对应，如果把一个yaml类型的模板应用到一个json类型的config会失效。  

##### 最终配置
Text编辑器里面看到的config是解码分享链接后生成的原始配置。在传给core前需要做点处理。比如，选项窗口中钩选了uTLS，那么就要在config里面添加fingerprint配置项。还有流量统计、自签名证书以及inbound这些设置也要翻译成相应的配置添加到config当中。这样处理之后得到的就是“最终配置”，即传给core的config。  

##### 多内核(分页)
{{< figure src="../../images/forms/form_option_custom_core.png" >}}
这个软件默认支持xray和v2ray v4.x内核。其他内核可以在`多内核`分页里面添加。上面是一个v2ray v5.x的配置例子。这里配置完成后可以回`默认值`分页选择`默认core`。使用自定义core时，流量统计功能失效。如果需要生成config文件，注意文件名(v5.json)要和命令行参数一致。生成的文件位于软件目录内。为了减少（不知道怎么避免）并发读写同一配置文件时发生冲突，连续写盘间隔设定为3秒。  

“环境变量” 可以用逗号分隔多个变量，例如：A=123, B=abc  
“测速Inb.模板” 对应Inbounds分页里面的配置名字。%host%将替换为127.0.0.1, %port%替换为随机端口。测速时合并到最终配置然后启动core。如果选“无”将生成一个http协议json格式的inbound合并到最终配置。  

##### 导入以后修改设置
上面的设置仅对新导入的服务器生效，已经导入的服务器可以在`设定及二维码`面板中修改相应的设置。  
{{< figure src="../../images/forms/form_settings_and_qrcode.png" >}}
提示：点击青色的 (http) 标签可以快速调出`设定及二维码`窗口。

同时修改多个服务器时，在钩选多个服务器以后点击“主窗口”-“服务器”-“批量修改”即可。 
{{< figure src="../../images/forms/form_batch_modify.png" >}}

