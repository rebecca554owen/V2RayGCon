---
title: "发布记录"
date: 2020-02-01T22:16:17+08:00
draft: false
weight: 1
---

##### `v1.6.6.0`
提高导入、删除服务器速度  
调整ProxySetter界面  
删除coreConfiger:GetHash()  

##### `v1.6.5.0`
Luna插件添加httpServ模块  
修复v1.6.2起可能无法保存服务器信息的bug  

##### `v1.6.4.0`
优化一点点内存占用  

##### `v1.6.3.0`
移除Luna插件所有Json相关的函数  
优化Luna脚本内存占用  

##### `v1.6.2.1`
一丢丢优化  

##### `v1.6.2.0`
提高导入服务器速度  
支持trojan链接的remark项  
托盘图标服务器菜单改为显示标题  
配置文件部分使用压缩格式  

##### `v1.6.1.0`
修改import合并规则，合并in(out)bounds中tag相同的项  
Luna插件添加htmlparser.lua（可以写爬虫了）  
Scripts包添加“故障转移.lua”  

##### `v1.6.0.0`
主窗口分页菜单显示实际的序号范围  
修复服务器较多(4K+)时搜索卡顿及部分菜单无法弹出的问题  
Luna插件添加Server:Count()  
Luna插件的coreState添加tag1到3三个自定义标签  
Luna插件Server.balancerStrategyRandom首字母b改成大写  
Luna插件Server.balancerStrategyLeastPing首字母b改成大写  
Scripts包添加“查询服务器所在国家.lua”  

##### `v1.5.9.1`
支持使用web-safe base64编码的订阅  
支持导入、分享shadowsocks SIP002标准的链接  
分页菜单添加序号范围  
导入结果窗口添加进度条  
~~修复服务器较多(4k+)时搜索会卡顿的问题~~ 偶尔还会卡顿，下个版本将修复此问题  
修复当前页选择菜单会清除其他页面选中状态的问题  

##### `v1.5.8.0`
修改配置文件存盘方式（测试中，注意多备份）  
scripts包添加一个SSH端口转发示例脚本  
Pacman插件添加ProbeURL及ProbeInterval选项  
Luna插件添加coreCtrl:RunSpeedTestThen()函数  

##### `v1.5.7.0`
修复Win7下有些序号显示不全的问题  
`vless://...`分享链接支持gRPC部分传输模式  

##### `v1.5.6.0`
Pacman插件添加将多个服务器串成一条代理链的功能  

##### `v1.5.5.0`
修复v2ray-core v4.35.1起出现的测速失败问题  
Pacman插件添加leastPing均衡策略（需v2ray-core v4.38.0+)  
Luna插件添加32位dll文件  

##### `v1.5.4.0`
支持vmess分享链接中的`sni`及`gRPC`配置项  
`选项`窗口添加uTLS指纹伪装配置项  
添加`XRAY_BROWSER_DIALER`等几个环境变量  
`简易编辑器`添加`gRPC`传输类型  

##### `v1.5.3.0`
支持从XTLS/Xray-core更新core  
修复服务器面板按钮闪烁问题  
服务器设置界面自动切换分享链接类型  

##### `v1.5.2.0`
支持vless0分享链接提案  
“此服务器日志”在窗口打开后才开始记录日志  
Luna插件添加Misc:GetSpeedtestQueueLength()函数  
默认关闭ProxySetter插件（感谢Microsoft Defender） 

##### `v1.5.1.0`
导入socks协议时，默认关闭mux  
临时支持xray，将xray.exe解压到core目录内即可  
到xray不兼容v2ray-core或者GUI发布时将不再支持  

##### `v1.5.0.0`
修复一处内存泄漏  

##### `v1.4.9.0`
修复偶尔出现的100%~空手接白刃~CPU占用问题（观察中）  
Luna插件添加Web:ExtractBase64String(text, minLen)函数  
支持Trojan协议的flow配置项(v2fly PR #334)  

##### `v1.4.8.0`
调整简易编辑器，添加serverName配置项  
服务器大于1024个时改用动态菜单（上一版是2048个）  
修复订阅的"启用"选项对Luna脚本的Web:UpdateSubscriptions()无效的问题  
服务器包的摘要改为显示balancer的tag  
优化程序退出时的清理步骤  
Scripts包添加"托管（简单版）.lua"  

##### `v1.4.7.0`
服务器大于2048个时，托盘改用动态菜单以减少卡顿  
点击“修改时间”标签时弹出“简单编辑器”  
适配v2fly PR #181添加的trojan协议(需要v2ray-core v4.30.0+)  

##### `v1.4.6.0`
测速过程中遇到错误不再弹出提示窗口  
更新v2ray-core时不重启插件  
优化日志窗口刷新算法减少卡顿  
优化多屏体验  
Luna添加`Misc:SetSubscriptionConfig(cfgStr)`  

##### `v1.4.5.0`
修复v1.4.4出现的界面小概率冻结问题  

##### `v1.4.4.0`
修复v1.4.3出现的删除服务器后界面不刷新问题  
添加一个简单版服务器编辑器  
二维码功能合并进服务器设定窗口  
订阅支持直接给vmess://...链接的网页  
GitHub的release页面添加scripts.zip脚本包   

##### `v1.4.3.0`
调整服务器面板各元素布局  
减少搜索及换页时出现的闪烁现象  
导入分享链接时忽略vmess最后面的等号  
添加lua.modules.coreEvent详见手册-所有函数  

##### `v1.4.2.0`
移除Staticstics插件（请先备份数据）  
流量统计功能整合进主窗口，试验中还有点bug  
添加`解码模板`选项，样板参见[templates/custom/vmessDecodeTemplate.json](https://raw.githubusercontent.com/vrnobody/V2RayGCon/master/V2RayGCon/Resources/Files/templates/custom/vmessDecodeTemplate.json)  
添加检查v2ray-core更新选项  

##### `v1.4.1.0`
添加自定义inbounds选项  
添加可选v2fly更新源  
修复了一个磨人的小(死)妖(锁)精  
修复文件占用引起的v2ray-core更新失败问题  
主窗口搜索时保留当前的选中状态  
删除Luna中的`Web:FindAllHrefs()`函数  

##### `v1.4.0.0`  
Luna界面少量调整  
添加luasocket、luasec模块  
添加Sys:CreateHttpServer()、Sys:Volume*()  
Misc:Choice()支持数字键选取及双击确定  
托盘菜单可以停止脚本  
少量其他bug修复  

##### `v1.3.9.0`
界面微调  

##### `v1.3.8.0`
Luna插件少量更新  

##### `v1.3.7.2`  
Luna添加是否允许加载CLR库选项  
`Sys`库添加热键功能(详见手册Sys库最底)  
原`Json`库移进`Misc`并添加`modules.json`模块  
`Signal`添加`ScreenLocked()`查询屏幕是否处于锁定状态  
`Misc:ShowData()`改为返回`Json`字符串  

##### `v1.3.6.2`
此版本因存在界面冻结bug取消发布  

##### `v1.3.5.0`
快速切换功能的迟延要求改为可选  
添加允许自签证书选项  
Luna支持加载CLR库  
Luna添加ILuaSys接口及其他少量调整  

##### `v1.3.4.0`
托盘图标服务器菜单添加快速切换功能  
服务器面板添加备注标签  
程序退出后保留上次测速结果  
优化unicode支持  
用户界面多处微调  

##### `v1.3.3.0`
Luna插件添加Misc:ShowData等几个函数  
批量测速时改用随机顺序  
订阅界面少量调整  

##### `v1.3.2.0`
优化测速算法  
支持v2ray-core v4.23.1的多文件配置功能  
修复ProxySetter插件的菜单不更新问题  
升级失败时无需重启软件  

##### `v1.3.1.0`
修复导入链接解码失败时，其他链接丢失的问题  
服务器很多时，相关菜单自动切换为多层嵌套模式  
优化批量测速的CPU占用  
优化界面刷新逻辑  

##### `v1.3.0.0`
Luna插件的Input控件可设定初值  
修复一个导致菜单冻住的bug  
修复中文tag乱码问题  

##### `v1.2.9.3`
luna插件添加Input，Choices等几个输入控件  

##### `v1.2.9.0`
界面少量调整  
修改lua/libs/utils.lua  

##### `v1.2.8.7`
`v://...`分享链接支持http协议  

##### `v1.2.8.4`
重新设计服务器面板  

##### `v1.2.8.2`
添加停止测速功能  
修复主窗口的失焦问题  

##### `v1.2.8.0`
更新各Nuget包  
修复lua/templates文件夹为空的问题  
修复订阅选项页有时会崩溃的问题  
修复主窗口聚焦问题  
默认启用ProxySetter插件  

##### `v1.2.7.7`
vee链接支持v2ray/discussion [#513](https://github.com/v2ray/discussion/issues/513) 中提出的socks outbound（测试中）

##### `v1.2.7`  
少量更新  

