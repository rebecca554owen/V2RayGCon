---
title: "同时开启http和socks"
date: 2020-02-02T22:45:44+08:00
draft: false
weight: 19
---

利用[全局import](https://vrnobody.github.io/V2RayGCon/01-usage/17-global-import/)我们可以实现同时开启http和socks入口  
假设当前服务器的模式为HTTP，我们先创建一个像下面这样的的json文件：
```json
{
  "inbounds": [
    {
      "tag": "socksin",
      "protocol": "socks",
      "listen": "127.0.0.1",
      "port": 1080,
      "settings": {
        "auth": "noauth",
        "accounts": [],
        "udp": true,
        "ip": "127.0.0.1",
        "timeout": 0,
        "userLevel": 0
      }
    }
  ]
}
```
把上面的文件另存为`d:/socks1080.json`  

在"选项"-"全局import"中添加：  
别名：`socks1080`  
Url：`d:/socks1080.json`  
钩上底下的"开启"选项  

现有服务器：  
在主窗口中全选所有服务器，接着点"服务器"-"调整选中"-"修改设置"  
钩上"import"并将后面的"False"改为"True"，最后点"修改"  
此时服务器面板左侧将出现` I `标记  

以后导入的服务器：  
点开"选项"窗口-"默认值"-"导入链接"，钩上"全局import"  


注意"全局import"有缓存机制，所以修改`d:/socks1080.json`之后需要在配置编辑器中清理下缓存  