---
title: "下载bing壁纸"
date: 2020-02-25T13:40:01+08:00
draft: false
weight: 2
---

需要V2RayGCon `v1.3.4.2`或以后版本。
如果使用`v1.3.4.1`或更早版本需要修改第24行。
```lua
-- 图片下载到以下位置(目录必须可写，注意斜杠方向)
imageFilename = "d:/wallpaper.jpg"

-- 代码
bingUrl = "https://www.bing.com"
bingApiUrl = bingUrl .. "/HPImageArchive.aspx?format=js&idx=0&n=8&mkt=zh-CN"

json = Web:Fetch(bingApiUrl)
jobj = Json:ParseJObject(json)
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

-- v1.3.4.1-
-- Misc:SetWallpaper(imageFilename)

print("完成")
```

