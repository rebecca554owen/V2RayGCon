---
title: "全局Import"
date: 2020-02-02T22:45:44+08:00
draft: false
weight: 17
---

这个功能用于合并多个配置文件，在“选项”窗口中设置。“测速”，“开启”，“打包”是触发条件。  

举个栗子：  
假设“开启”钩上，那么每次启动服务器时，相应的模板都会注入到该服务器配置里面。  
注意！只对“设定”里钩了“全局import”的服务器生效。  

全局import的url可以是本地路径也可以是网络地址：  
{{< figure src="../../images/forms/form-option-global-import.png" >}}

设定好“全局import”后，就可以在“配置编辑器”的“杂项”面板中选择插入当前配置（也可以手动填写）。
{{< figure src="../../images/forms/form-editor-insert-global-import.png" >}}

图中`policy.json`内容如下
{{< figure src="../../images/forms/form-editor-policy-json.png" >}}

切换到“展开”面板点下方“展开”按钮可以查看最终配置
{{< figure src="../../images/forms/form-editor-expand.png" >}}
import的模板有缓存机制，修改模板后需要点下“清理缓存”。  

全局Import支持嵌套，即被Import的模板还可以Import其他模板，最多可以嵌套5层。  
合并时按模板填写顺序，后项合并进前项，配置文件本身视为最后一个合并项。  
注意以下几个key前置，即后项内容会放到前项的顶部：
```json
"inbounds",
"outbounds",
"inboundDetour",
"outboundDetour",
"routing.rules",
"routing.balancers",
"routing.settings.rules",
```
“打包”时`routing`后置（因为打包原模板中的routing内容不太重要）。


