---
title: "第三方插件"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 5
---

从`v1.8.3`开始又重新支持第三方插件。把插件发布文件包解压到V2RayGCon目录内，然后在选项窗口的插件面板中启用。第三方插件制作方法详见[IPlugin.cs](https://github.com/vrnobody/V2RayGCon/blob/master/VgcApis/Interfaces/IPlugin.cs)里面的注释。简单来说就是，创建一个.net framework 4.5的dll项目并引用VgcApis项目，然后实现IPlugin接口。如果你想分享自己写的插件，直接发issue即可。  
  
安全提示：  
点选项窗口-插件分页的刷新按钮，会执行3rd/plugins目录中插件dll的代码，有一定风险！  
由于我水平不高加上本项目的特殊性，我不会对第三方插件做任何检查。  
  
第三方插件列表：  
<等你来添加>  

