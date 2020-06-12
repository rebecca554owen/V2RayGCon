---
title: "网络相关"
date: 2020-02-25T13:40:01+08:00
draft: false
weight: 30
---

从Bing下载图片并设置为壁纸  
```lua
-- 图片下载到以下位置(目录必须可写，注意斜杠方向)
imageFilename = "d:/wallpaper.jpg"

-- 代码
bingUrl = "https://www.bing.com"
bingApiUrl = bingUrl .. "/HPImageArchive.aspx?format=js&idx=0&n=8&mkt=zh-CN"

json = Web:Fetch(bingApiUrl)

-- v1.3.6.0+
jobj = Misc:ParseJObject(json)

assert(jobj ~= nil)
keys = {}
values = {}
images = jobj["images"]
for image in Each(images) do
    table.insert(keys, tostring(image["copyright"]))
    table.insert(values, tostring(image["url"]))
end

index = Misc:Choice("请选择壁纸(点取消结束脚本):", keys, true)
assert(index > 0)
Web:Download(bingUrl .. values[index], imageFilename)

-- v1.3.4.2+
Sys:SetWallpaper(imageFilename)

print("完成")
```

查询百度网盘配额 V2RayGCon `v1.4.1.0+`
```lua
local BDUSS = "填入你的BDUSS，网上有教程，很简单的"

-- 如果以下appid不可用，请网上搜可用appid
local appId = "371067"

-- 代码
local https = require('lua.libs.luasec.https')
local ltn12 = require('lua.libs.luasocket.ltn12')
local json = require('lua.libs.json')

local bdApi = "https://pcs.baidu.com/rest/2.0/pcs/"

local function GenReqUrl(cat, method, params)
    local tail = ""
    if type(params) == "table" and table.length(params) > 0 then
        for k, v in pairs(params) do
            tail = tail .. "&" .. tostring(k) .. "=" .. tostring(v)
        end
    end
    return bdApi .. cat
        .. "?app_id=" .. appId 
        .. "&method=" .. method
        .. tail
end

local function Post(cat, method, params)
    local r = {}
    local res, code, h, status = https.request({
        url = GenReqUrl(cat, method, params),
        sink = ltn12.sink.table(r),
        method = "POST",
        headers = { Cookie = "BDUSS=" .. BDUSS},
    })
    if code == 200 then
        return table.concat(r)
    end
    print("debug: ", res, ", ", code, ", ", h, ", ", status)
    return nil
end

local function GetQuota()
    local text = Post("quota", "info")
    local j = json.decode(text)
    local gib = 1024 * 1024 * 1024
    local q = math.floor(j["quota"] / gib)
    local u = math.floor(j["used"] / gib)
    return "空间(GiB)：" .. tostring(u) .. " / " .. tostring(q)
end

Misc:Alert(GetQuota())
```
