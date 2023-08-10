---
title: "NeoLuna"
date: 2020-02-01T12:31:46+08:00
draft: false
weight: 20
---
V2RayGCon v1.8+

NeoLuna是采用NeoLua作为后端的Lua脚本管理器。多插个字母n主要是避免c#命名空间重复问题。NeoLua是直接用c#写的lua5.3。原来的Luna插件后端是NLua，它是通过P/Invoke调用c写的lua53.dll。  

使用NeoLuna插件的时候注意这些坑：  
 1. 正则是通过简单的查找、替换把lua的pattern转成c#的regex，所以像string.gsub(), string.match()之类的函数，执行结果会和NLua不完全一致。要特别注意参数带table或者function的情况。  
 2. select()是从0开始算，而NLua是从1开始算。  
 3. 原版NeoLua的string, table不支持添加函数。NeoLuna插件给他们加了点魔法，让他们支持扩展。副作用是部分函数行为会和NLua不一致。  
 4. require()里面的路径用'/'分隔，所以NeoLua不能重用NLua写的模块。  
 5. NeoLua不支持string.dump()，不支持loadfile后面传参数。  
 6. NeoLua的os.time()是带时区的秒数，但os.date()输出结果又和NLua相同。  

 既然NeoLua有这么多坑，为什么还要加入这个功能重复的插件呢？因为NLua有个比较严重的问题。当调用像Server:GetAllServers() coreServ:GetCoreStates()这样的函数的时候，NLua会保留一个引用。就算后来删掉服务器、停止脚本、Dispose掉Lua()虚拟机这个引用依然存在。这导致CLR没法回收内存。服务器比较少（一千来个）的时候，问题并不严重。但是服务器比较多（上万个）并且经常添加、删除服务器的时候问题就会暴露出来，有时内存占用会上G。还有个有意思的现象，因为这些对象只保留了一个引用并不会被访问到，所以一段时间（几小时）过后，操作系统会把这些内存转到pagefile（也就是Linux的swap）里面。这时看任务管理器的内存数值会降下来，给人一种内存被回收了的错觉。 而NeoLua的对象完全由CLR的GC管理，没有这个问题。  

正则的坑我也没想到什么好的解决办法。目前一个比较绕的办法是在NeoLua中通过mailbox把要处理的文本发给NLua，处理完成后再发送回来。多说一句NeoLua和NLua的LocalStorage, SnapCache还有mailbox都是相通的。但是他们的table实现方式有所不同，所以如果不序列化直接把table当object发送，对方收到的将会是userdata。同一个插件的不同脚本之间发送table就没这个问题。  
