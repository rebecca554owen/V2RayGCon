---
title: "Trojan示例"
date: 2020-04-14T17:35:52+08:00
draft: false
weight: 6
---

需要V2RayGCon `v1.3.5.0`或以后版本  

这个脚本主要演示怎么调用CLR里的Winform库，运行效果大概这样：  
（托盘区还会有个橙色的小图标哦！）  
{{< figure src="../../../images/plugins/trojan_gui.png" >}}  
  
    
全部代码如下：  

```lua
-- 假设trojan的位置是V2RayGCon/trojan/trojan.exe

local trExe = "trojan/trojan.exe"
local trConfigJson = "trojan/config.json"

-- 代码

import('System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089')
import('System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a')
import('System.Windows.Forms')
import('System.Drawing')
import('System.Drawing.Drawing2D')

local Utils = require "libs.utils"

local trKey = "trojan-settings"

local chkSelfSignedCert
local tboxServAddr, tboxServPort, tboxServPassword, tboxLocalAddr, tboxLocalPort
local btnSave, btnStart, btnStop, btnExit

local form = nil
local trojan = nil
local isExit = false

function Main()
    
    CreateNotifyIcon()
    
    ShowForm()
    while not Signal:Stop() and not isExit do
        Misc:Sleep(100)
        Application.DoEvents()
    end
    Cleanup()
end

function OnFormClosed()
    form = nil
end

function ShowForm()
    
    if form ~= nil then
        form:Activate()
        return
    end
    
    CreateWinForm()
    local settings = LoadSettings()
    SetControlsValue(settings)
    BindEvents()
    form:Show()
end

function OnMouseClick()
    ShowForm()
end

function DrawAppIcon()
    local s = 96
    local icon = Bitmap(s, s);
    local g = Graphics.FromImage(icon)
    g.InterpolationMode = InterpolationMode.High;
    g.CompositingQuality = CompositingQuality.HighQuality;
    g:FillEllipse(Brushes.Orange, 0,  0, s, s)
    g:FillRectangle(Brushes.White, s * 0.2, s * 0.23, s*0.6, s*0.16);
    g:FillRectangle(Brushes.White, s * 0.41, s * 0.3, s*0.18, s*0.55);
    g:Dispose()
    return icon
end

function CreateNotifyIcon()
    ni = NotifyIcon()
    local icon = DrawAppIcon()
    ni.Icon = Icon.FromHandle(icon:GetHicon())
    ni.Text = "Trojan"
    ni.Visible = true
    ni.MouseClick:Add(OnMouseClick)
end

function Cleanup()
    ni.Visible = false
    ni:Dispose()
    StopTrojan()
end

function OnBtnExitClick()
    isExit = true
end

function BindEvents()
    btnSave.Click:Add(OnBtnSaveClick)
    btnStart.Click:Add(OnBtnStartClick)
    btnStop.Click:Add(OnBtnStopClick)
    btnExit.Click:Add(OnBtnExitClick)
    form.FormClosed:Add(OnFormClosed)
end

function StartTrojan()
    if IsRunning() then
        StopTrojan()
    end
    local args = "-c " .. trConfigJson
    trojan = Sys:Run(trExe, args, stdin, nil, false, true)
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
    local s = GetControlsValue()
    local cfg = GenConfigString(s)
    -- print(cfg)
    WriteToFile(trConfigJson, cfg)
    StartTrojan()
end

function OnBtnSaveClick()
    local s = GetControlsValue()
    print("save settings.")
    -- print(s)
    Misc:WriteLocalStorage(trKey, s:ToString())
end

function GetControlsValue()
    local tpl = LoadTemplate("settings")
    local s = Json:ParseJObject(tpl)
    Json:TrySetBoolValue(s, "ssl.verify",not chkSelfSignedCert.Checked)
    Json:TrySetStringValue(s, "remote_addr", tboxServAddr.Text)
    local remote_port = Utils.ToNumber(tboxServPort.Text)
    Json:TrySetIntValue(s, "remote_port", remote_port)
    Json:TrySetStringValue(s, "password.0", tboxServPassword.Text)
    Json:TrySetStringValue(s, "local_addr", tboxLocalAddr.Text)
    local local_port = Utils.ToNumber(tboxLocalPort.Text)
    Json:TrySetIntValue(s, "local_port", local_port)
    return s
end

function SetControlsValue(settings)
    chkSelfSignedCert.Checked =not Json:GetBool(settings, "ssl.verify")
    tboxServAddr.Text = Json:GetString(settings, "remote_addr")
    tboxServPort.Text = Json:GetString(settings, "remote_port")
    tboxServPassword.Text = Json:GetString(settings, "password.0")
    tboxLocalAddr.Text = Json:GetString(settings, "local_addr")
    tboxLocalPort.Text = Json:GetString(settings, "local_port")    
end

function LoadSettings()
    local rawData = Misc:ReadLocalStorage(trKey)
    local json = Json:ParseJObject(rawData)
    if json == nil then
        rawData = LoadTemplate("settings")
        return Json:ParseJObject(rawData)
    end
    return json
end

function GenConfigString(settings)
    local tpl = LoadTemplate("config")
    local config = Json:ParseJObject(tpl)
    Json:Merge(config, settings)
    return config:ToString()
end 

function CreateWinForm()
	form = Form()
    chkSelfSignedCert =  CheckBox()

	tboxServAddr =  TextBox()
    tboxServPort =  TextBox()
    tboxServPassword =  TextBox()
    
    tboxLocalPort =  TextBox()
	tboxLocalAddr =  TextBox()
    
    btnSave =  Button()
    btnStart =  Button()
	btnStop =  Button()
    btnExit =  Button()
    
	local label1 =  Label()
	local groupBox1 =  GroupBox()
	local label3 =  Label()
	local groupBox2 =  GroupBox()
	local label4 =  Label()
	
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

	btnSave.Location = Point(59, 163)
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

	groupBox1.Controls:Add(label3)
	groupBox1.Controls:Add(label1)
	groupBox1.Controls:Add(tboxServPassword)
	groupBox1.Controls:Add(tboxServAddr)
	groupBox1.Controls:Add(chkSelfSignedCert)
	groupBox1.Controls:Add(tboxServPort)
	groupBox1.Location = Point(14, 12)
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

	groupBox2.Controls:Add(tboxLocalPort)
	groupBox2.Controls:Add(tboxLocalAddr)
	groupBox2.Controls:Add(label4)
	groupBox2.Location = Point(14, 102)
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

	btnStart.Location = Point(115, 163)
	btnStart.Name = "btnStart"
	btnStart.Size = Size(50, 23)
	btnStart.TabIndex = 3
	btnStart.Text = "启动"
	btnStart.UseVisualStyleBackColor = true

	btnStop.Location = Point(171, 163)
	btnStop.Name = "btnStop"
	btnStop.Size = Size(50, 23)
	btnStop.TabIndex = 3
	btnStop.Text = "停止"
	btnStop.UseVisualStyleBackColor = true
    
    btnExit.Location = Point(226, 163)
    btnExit.Name = "btnExit"
    btnExit.Size = Size(50, 23)
    btnExit.TabIndex = 3
    btnExit.Text = "退出"
    btnExit.UseVisualStyleBackColor = true

	form.AutoScaleDimensions = SizeF(6.0, 12.0)
	form.AutoScaleMode = AutoScaleMode.Font
	form.ClientSize = Size(334, 193)
	form.Controls:Add(groupBox2)
	form.Controls:Add(groupBox1)
	form.Controls:Add(btnStop)
	form.Controls:Add(btnStart)
	form.Controls:Add(btnSave)
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
    "remote_addr": "www.baidu.com",
    "remote_port": 80,
    "password": [ "123457" ],
    "ssl": {
        "verify": true
    },
    "local_addr": "127.0.0.1",
    "local_port": 1080
}
]]

    local defConfig = [[
{
    "run_type": "client",
    "local_addr": "0.0.0.0",
    "local_port": 8080,
    "remote_addr": "www.bing.com",
    "remote_port": 443,
    "password": [],
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
}
    ]]
       
    if key == "settings" then
        return defSettings
    elseif key == "config" then
        return defConfig
    else
        return nil
    end
end

Main()
```
