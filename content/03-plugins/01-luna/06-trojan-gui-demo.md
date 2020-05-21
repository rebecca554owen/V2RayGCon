---
title: "Trojan示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 6
---

需要V2RayGCon `v1.3.8.4+`

这个脚本主要演示怎么调用CLR里的Winform库，运行效果大概这样：  
{{< figure src="../../../images/plugins/trojan_gui.png" >}}  
注意要钩上`加载CLR库`选项，运行之后托盘区会出现一个的小图标。  
    
全部代码如下：  

```lua
-- 设置
local trExe = "trojan/trojan.exe"
local trConfigJson = "trojan/config.json"

-- 代码

import('System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089')
import('System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a')
import('System.Windows.Forms')
import('System.Drawing')
import('System.Drawing.Drawing2D')

local Utils = require "libs.utils"
local libJson = require "libs.json"

local trKey = "trojan-settings"

local chkSelfSignedCert
local tboxServAddr, tboxServPort, tboxServPassword, tboxLocalAddr, tboxLocalPort
local cboxName
local btnSave, btnStart, btnStop, btnExit, btnRemove

local servSettings = nil;

local form = nil
local trojan = nil
local isExit = false
local lastIsRunning = true

function Main()
    
    servSettings = LoadSettings()
    CreateNotifyIcon()
    ShowForm()
    
    local tick = 100
    while not Signal:Stop() and not isExit do
        Misc:Sleep(tick)
        Application.DoEvents()
        UpdateIcon()
    end
    
    Cleanup()
end

function GetCurServ()
    local n = servSettings["curServ"]
    if n ~= nil then 
        return n
    end
    return FirstKey(servSettings["servers"])
end

function OnBtnRemoveClick()
    local name = cboxName.Text
    if name == nil or string.isempty(name) then
        Misc:Alert("错误:没设置名字")
        return
    end
    if not Misc:Confirm("确定要删除[" .. name .. "]?") then
        return
    end
    print("Remove server [", name, "]")
    servSettings["servers"][name] = nil
    SaveSettings()
end

function FirstKey(t)
    if t == nil or type(t) ~= "table" then
        return nil
    end
    for k, v in pairs(t) do
      return k
    end
    return nil
end

function GetServSetting(name)
    if name == nil or string.isempty(name) 
    then
        return nil
    end
    
    local v = servSettings["servers"][name]
    return Clone(v)
end

function AddServSetting(name, setting)
    if name == nil or setting == nil or string.isempty(name) then
        Misc:Alert("错误:没有设置名字!")
        return
    end
    print("Save server [", name, "] settings.")
    servSettings["servers"][name] = Clone(setting)
    SaveSettings()
    Misc:Alert("完成")
end

function Clone(json)
    if json == nil then
        return nil
    end
    local str = libJson.encode(json)
    return libJson.decode(str)
end

function SaveSettings()
    if servSettings == nil then
        print("Error: server setting object is empty")
        return
    end
    local s = libJson.encode(servSettings)
    Misc:WriteLocalStorage(trKey, s)
end

function OnFormClosed()
    form = nil
end

function ShowForm()
    
    if form ~= nil then
        form:Activate()
        return
    end
    
    -- print("create form")
    CreateWinForm()
    -- print("Show record: ", GetCurServ()) 
    ShowRecord(GetCurServ())
    -- print("bind events")
    BindEvents()
    form:Show()
end

function ShowRecord(name)
    local s = GetServSetting(name)
    if s ~= nil then
        servSettings["curServ"] = name
        SaveSettings()
        SetControlsValue(name, s)
    else
        MessageBox.Show("找不到相关服务器信息")
    end
end

function OnMouseClick()
    ShowForm()
end

function DrawAppIcon(isOn)
    
    local s = 96
    local brBack = isOn and Brushes.DarkOrange or Brushes.ForestGreen
    local brFont = Brushes.White
    
    local icon = Bitmap(s, s)
    local g = Graphics.FromImage(icon)
    -- Circle
    g:FillEllipse(brBack, 0,  0, s, s)
    
    -- T
    g:FillRectangle(brFont, s*0.05, s*0.16, s*0.6,s*0.15)
    g:FillRectangle(brFont, s*0.29, s*0.2, s*0.15, s*0.7)
    
    -- r
    g:FillRectangle(brFont, s*0.6, s*0.47, s*0.16, s* 0.4);
    local rect = Rectangle(s*0.73, s*0.55, s*0.5, s*0.5)
    local pen = Pen(brFont, s*0.1)
    g:DrawArc(pen, rect, 210, 53)
    g:Dispose()
    return icon
end

function UpdateIcon()
    if lastIsRunning == IsRunning() then
        return
    end
    
    lastIsRunning = not lastIsRunning
    if ni.Icon ~= nil then
        ni.Icon:Dispose()
    end
    
    local icon = DrawAppIcon(lastIsRunning)
    local text = lastIsRunning and "ON" or "OFF"
    ni.Icon =Icon.FromHandle(icon:GetHicon())
    ni.Text = "Trojan " .. text
end

function CreateNotifyIcon()
    ni = NotifyIcon()
    UpdateIcon()
    ni.Visible = true
    ni.MouseClick:Add(OnMouseClick)
end

function Cleanup()
    if form ~= nil then
        form:Close()
    end
    ni.Visible = false
    ni:Dispose()
    StopTrojan()
end

function OnBtnExitClick()
    isExit = true
end

function OnCboxNameSelectedValueChanged()
    local name = cboxName.Text
    ShowRecord(name)    
end

function BindEvents()
    btnSave.Click:Add(OnBtnSaveClick)
    btnStart.Click:Add(OnBtnStartClick)
    btnStop.Click:Add(OnBtnStopClick)
    btnExit.Click:Add(OnBtnExitClick)
    btnRemove.Click:Add(OnBtnRemoveClick)
    form.FormClosed:Add(OnFormClosed)
    cboxName.DropDown:Add(OnCboxNameDropDown)
    cboxName.SelectedValueChanged:Add(OnCboxNameSelectedValueChanged)
end

function StartTrojan()
    if IsRunning() then
        StopTrojan()
    end
    local args = "-c " .. trConfigJson
    trojan = Sys:Run(trExe, args, nil, nil, false, true)
end

function StopTrojan()
    if not IsRunning() then
        return
    end
    Sys:SendStopSignal(trojan)
end

function IsRunning()
    return not Sys:HasExited(trojan)
end

function WriteToFile(filename, content)
	local file = io.open(filename, "w+")
    file:write(content)
    file:close()
end

function OnBtnStopClick()
    StopTrojan()
end

function OnBtnStartClick()
    local n, s = GetControlsValue()
    local cfg = GenConfigString(s)
    WriteToFile(trConfigJson, cfg)
    StartTrojan()
end

function OnBtnSaveClick()
    local n, s = GetControlsValue()
    AddServSetting(n, s)
end

function GetControlsValue()
    local name = cboxName.Text
    local tpl = LoadTemplate("setting")
    local s = libJson.decode(tpl)
    s["verify"] = not chkSelfSignedCert.Checked
    s["remote_addr"] = tboxServAddr.Text
    s["remote_port"] = Utils.ToNumber(tboxServPort.Text)
    s["password"] = tostring(tboxServPassword.Text)
    s["local_addr"] = tboxLocalAddr.Text
    s["local_port"] = Utils.ToNumber(tboxLocalPort.Text)
    return name, s
end

function SetControlsValue(name, s)
    cboxName.Text = name
    chkSelfSignedCert.Checked = not s["verify"]
    tboxServAddr.Text = s["remote_addr"]
    tboxServPort.Text = tostring(s["remote_port"])
    tboxServPassword.Text = tostring(s["password"])
    tboxLocalAddr.Text = s["local_addr"]
    tboxLocalPort.Text = tostring(s["local_port"])
end

function CreateDefSettings()
    local tpl = LoadTemplate("defSettings")
    return libJson.decode(tpl)
end

function LoadSettings()
    
    local rawData = Misc:ReadLocalStorage(trKey)
    if rawData == nil or string.isempty(rawData) then
        print("create default setting")
        return CreateDefSettings()
    end
    
    local json = libJson.decode(rawData)
    if json == nil 
       or json["servers"] == nil
    then
        return CreateDefSettings()
    end
    return json
end

function OnCboxNameDropDown()
    cboxName.Items:Clear()
    local t = servSettings["servers"]
    if t == nil then 
        print("DB is empty!")
        return
    end
    for k, v in pairs(t) do
        cboxName.Items:Add(k)
    end
end    

function GenConfigString(s)
    
    local tpl = LoadTemplate("config")
    local config = libJson.decode(tpl)
    
    -- 切记Lua从1开始编号!!!
    config["password"][1] = tostring(s["password"])
    
    config["ssl"]["verify"] = s["verify"]
    config["remote_addr"] = s["remote_addr"]
    config["remote_port"] = s["remote_port"]
    config["local_addr"] = s["local_addr"]
    config["local_port"] = s["local_port"]
    
    local encoded = libJson.encode(config)
    return encoded
end 

function CreateWinForm()
    
	form = Form()
    
    cboxName = ComboBox()
    btnRemove = Button()
	tboxServAddr =  TextBox()
    tboxServPort =  TextBox()
    tboxServPassword =  TextBox()
    chkSelfSignedCert =  CheckBox()
    tboxLocalPort =  TextBox()
	tboxLocalAddr =  TextBox()
    btnSave =  Button()
    btnStart =  Button()
	btnStop =  Button()
    btnExit =  Button()
    
    cboxName.TabIndex = 0
    btnRemove.TabIndex = 1
    tboxServAddr.TabIndex = 2
    tboxServPort.TabIndex = 3
    tboxServPassword.TabIndex = 4
    chkSelfSignedCert.TabIndex = 5
    tboxLocalPort.TabIndex = 6
    tboxLocalAddr.TabIndex = 7
    
    btnSave.TabIndex = 8
    btnStart.TabIndex = 9
    btnStop.TabIndex = 10
    btnExit.TabIndex = 11
    
	local label1 =  Label()
	local groupBox1 =  GroupBox()
	local label3 =  Label()
	local groupBox2 =  GroupBox()
	local label4 =  Label()
    local label5 = Label()
	
	groupBox1:SuspendLayout()
	groupBox2:SuspendLayout()
	form:SuspendLayout()

	tboxServAddr.Location = Point(39, 20)
	tboxServAddr.Name = "tboxServAddr"
	tboxServAddr.Size = Size(172, 21)
	tboxServAddr.TabIndex = 2
	tboxServAddr.Text = "http://www.test.com"

	label1.AutoSize = true
	label1.Location = Point(6, 24)
	label1.Name = "label1"
	label1.Size = Size(29, 12)
	label1.TabIndex = 1
	label1.Text = "地址"

	tboxServPort.Location = Point(217, 20)
	tboxServPort.Name = "tboxServPort"
	tboxServPort.Size = Size(72, 21)
	tboxServPort.TabIndex = 2
	tboxServPort.Text = "65535"

	btnSave.Location = Point(59, 189)
	btnSave.Name = "btnSave"
	btnSave.Size = Size(50, 23)
	btnSave.TabIndex = 3
	btnSave.Text = "保存"
	btnSave.UseVisualStyleBackColor = true

	chkSelfSignedCert.AutoSize = true
	chkSelfSignedCert.Location = Point(217, 52)
	chkSelfSignedCert.Name = "chkSelfSignedCert"
	chkSelfSignedCert.Size = Size(72, 16)
	chkSelfSignedCert.TabIndex = 4
	chkSelfSignedCert.Text = "自签证书"
	chkSelfSignedCert.UseVisualStyleBackColor = true

    groupBox1.Controls:Add(label1)
	groupBox1.Controls:Add(label3)
	groupBox1.Controls:Add(tboxServAddr)
    groupBox1.Controls:Add(tboxServPort)
    groupBox1.Controls:Add(tboxServPassword)
	groupBox1.Controls:Add(chkSelfSignedCert)
	
	groupBox1.Location = Point(14, 38)
	groupBox1.Name = "groupBox1"
	groupBox1.Size = Size(301, 84)
	groupBox1.TabIndex = 6
	groupBox1.TabStop = false
	groupBox1.Text = "服务器"

	label3.AutoSize = true
	label3.Location = Point(6, 54)
	label3.Name = "label3"
	label3.Size = Size(29, 12)
	label3.TabIndex = 1
	label3.Text = "密码"

	tboxServPassword.Location = Point(39, 50)
	tboxServPassword.Name = "tboxServPassword"
	tboxServPassword.Size = Size(172, 21)
	tboxServPassword.TabIndex = 2
	tboxServPassword.Text = "000000"

    groupBox2.Controls:Add(label4)
    groupBox2.Controls:Add(tboxLocalAddr)
	groupBox2.Controls:Add(tboxLocalPort)
		
	groupBox2.Location = Point(14, 128)
	groupBox2.Name = "groupBox2"
	groupBox2.Size = Size(301, 55)
	groupBox2.TabIndex = 6
	groupBox2.TabStop = false
	groupBox2.Text = "本地"

	tboxLocalPort.Location = Point(217, 20)
	tboxLocalPort.Name = "tboxLocalPort"
	tboxLocalPort.Size = Size(72, 21)
	tboxLocalPort.TabIndex = 2
	tboxLocalPort.Text = "0"

	tboxLocalAddr.Location = Point(39, 20)
	tboxLocalAddr.Name = "tboxLocalAddr"
	tboxLocalAddr.Size = Size(172, 21)
	tboxLocalAddr.TabIndex = 2
	tboxLocalAddr.Text = "192.168.0.1"

	label4.AutoSize = true
	label4.Location = Point(6, 23)
	label4.Name = "label4"
	label4.Size = Size(29, 12)
	label4.TabIndex = 1
	label4.Text = "监听"

	btnStart.Location = Point(115, 189)
	btnStart.Name = "btnStart"
	btnStart.Size = Size(50, 23)
	btnStart.TabIndex = 3
	btnStart.Text = "启动"
	btnStart.UseVisualStyleBackColor = true

	btnStop.Location = Point(171, 189)
	btnStop.Name = "btnStop"
	btnStop.Size = Size(50, 23)
	btnStop.TabIndex = 3
	btnStop.Text = "停止"
	btnStop.UseVisualStyleBackColor = true
    
    btnExit.Location = Point(226, 189)
    btnExit.Name = "btnExit"
    btnExit.Size = Size(50, 23)
    btnExit.TabIndex = 3
    btnExit.Text = "退出"
    btnExit.UseVisualStyleBackColor = true

    -- top
    cboxName.FormattingEnabled = true
    cboxName.Location = Point(53, 12)
    cboxName.Name = "cboxName"
    cboxName.Size = Size(172, 20)
    cboxName.TabIndex = 0

    btnRemove.Location = Point(231, 9)
    btnRemove.Name = "btnRemove"
    btnRemove.Size = Size(72, 23)
    btnRemove.TabIndex = 1
    btnRemove.Text = "删除"
    btnRemove.UseVisualStyleBackColor = true

    label5.AutoSize = true;
    label5.Location = Point(20, 15);
    label5.Name = "label1";
    label5.Size = Size(29, 12);
    label5.TabIndex = 2;
    label5.Text = "名称";

	form.AutoScaleDimensions = SizeF(6.0, 12.0)
	form.AutoScaleMode = AutoScaleMode.Font
	form.ClientSize = Size(334, 219)
    form.Controls:Add(label5)
    form.Controls:Add(cboxName)
    form.Controls:Add(btnRemove)
	form.Controls:Add(groupBox1)
	form.Controls:Add(groupBox2)
    form.Controls:Add(btnSave)
	form.Controls:Add(btnStop)
	form.Controls:Add(btnStart)
    form.Controls:Add(btnExit)
	form.FormBorderStyle = FormBorderStyle.FixedDialog
	form.MaximizeBox = false
	form.MinimizeBox = false
	form.Name = "Form1"
	form.StartPosition = FormStartPosition.CenterScreen
	form.Text = "🎠Trojan"

	groupBox1:ResumeLayout(false)
	groupBox1:PerformLayout()
	groupBox2:ResumeLayout(false)
	groupBox2:PerformLayout()
	form:ResumeLayout(false)
end

 
function LoadTemplate(key)
    local defSettings = [[
{
    "curServ": "",
    "servers": [],
}]]
        
    local setting = [[
{
    "remote_addr": "www.baidu.com",
    "remote_port": 80,
    "password": "123457",
    "verify": true,
    "local_addr": "127.0.0.1",
    "local_port": 1080
}]]

    local defConfig = [[
{
    "run_type": "client",
    "local_addr": "0.0.0.0",
    "local_port": 8080,
    "remote_addr": "www.bing.com",
    "remote_port": 443,
    "password": ["none"],
    "log_level": 1,
    "ssl": {
        "verify": false,
        "verify_hostname": true,
        "cert": "",
        "cipher": "ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-AES256-SHA:ECDHE-ECDSA-AES128-SHA:ECDHE-RSA-AES128-SHA:ECDHE-RSA-AES256-SHA:DHE-RSA-AES128-SHA:DHE-RSA-AES256-SHA:AES128-SHA:AES256-SHA:DES-CBC3-SHA",
        "cipher_tls13": "TLS_AES_128_GCM_SHA256:TLS_CHACHA20_POLY1305_SHA256:TLS_AES_256_GCM_SHA384",
        "sni": "",
        "alpn": [
            "h2",
            "http/1.1"
        ],
        "reuse_session": true,
        "session_ticket": false,
        "curves": ""
    },
    "tcp": {
        "no_delay": true,
        "keep_alive": true,
        "reuse_port": false,
        "fast_open": false,
        "fast_open_qlen": 20
    }
}]]
       
    if key == "setting" then
        return setting
    elseif key == "config" then
        return defConfig
    elseif key == "defSettings" then
        return defSettings
    end
    
    return nil
end


Main()
```
