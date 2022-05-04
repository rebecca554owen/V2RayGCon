---
title: "用网页控制服务器"
date: 2020-02-25T13:40:01+08:00
draft: false
weight: 32
---

这个脚本需要用到V2RayGCon v1.6.5+新增的httpServ模块。  
```lua
-- 如果4000端口已在使用，可修改为其他端口
local url = "http://localhost:4000/"

local haServ = require('lua.modules.httpServ').new()
local json = require('lua.libs.json')
local utils = require('lua.libs.utils')

function Main()
    local html = GenHTML()
    haServ:Create(url, html, handler)
    print("请打开网址: ", url)
    haServ:Run()
end

function ListAllServers()
    local d = {}
    local servs = Server:GetAllServers()
    for coreServ in Each(servs) do
        local coreState = coreServ:GetCoreStates()
        local coreCtrl = coreServ:GetCoreCtrl()
        local t = {}
        t["title"] = coreState:GetTitle()
        t["uid"] = coreState:GetUid()
        t["on"] = coreCtrl:IsCoreRunning()
        table.insert(d, t)
    end
    return json.encode(d)
end 

function RestartServ(uid)
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreCtrl = coreServ:GetCoreCtrl()
        Server:StopAllServers()
        coreCtrl:RestartCore()
        return "ok"
    end
    return "unknow uid: " .. uid
end

function StopServ(uid)
    local coreServ = utils.GetFirstServerWithUid(uid)
    if coreServ ~= nil then
        local coreCtrl = coreServ:GetCoreCtrl()
        coreCtrl:StopCore()
        return "ok"
    end
    return "unknow uid: " .. uid
end

function handler(req)
    local j = json.decode(req)
    local op = j["op"]
    if op == "RefreshServs" then
        return ListAllServers()
    elseif op == "RestartServ" then
        return RestartServ(j["p1"])
    elseif op == "StopServ" then
        return StopServ(j["p1"])
    end
    return "unknow req: " .. req
end

function GenHTML()
    return [[
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Demo</title>
    <script src="https://unpkg.com/vue@3"></script>
    <style>
        li:nth-child(odd) {
            background-color: ghostwhite;
        }
        .round-div {
            background-color: orange;
            display: inline-block;
            color: white;
            padding: 0px 5px;
            border-radius: 3px;
            cursor: pointer;
        }
    </style>
</head>

<body>
    <div id="app">
        <button @click="refreshServs()">Refresh</button>
        <ul style="list-style-type: none;">
            <li v-for="serv in servs" style="padding: 3px;">
                <div v-if="serv.on"
                     @click="stopServ(serv.uid)"
                     class="round-div">
                     ON
                </div>
                {{ serv.title }}
                <button @click="restartServ(serv.uid)">Restart</button>
            </li>
        </ul>
    </div>
</body>

<script>

    function post(callback, content) {
        let httpRequest = new XMLHttpRequest()
        httpRequest.open('POST', '/', true)
        httpRequest.send(content)
        httpRequest.onreadystatechange = function () {
            if (httpRequest.readyState == 4 && httpRequest.status == 200) {
                let resp = httpRequest.responseText
                callback(resp);
            }
        };
    }

    function invoke(callback, op, p1) {
        let cmd = {
            op: op,
            p1: p1,
        }
        post(callback, JSON.stringify(cmd))
    }

    const app = Vue.createApp({
        data() {
            return {
                servs: [],
            }
        },
        methods: {
            stopServ(uid) {
                let that = this
                let done = function (msg) {
                    that.refreshServs()
                    if(msg != "ok"){
                        alert(msg)
                    }
                }
                invoke(done, "StopServ", uid)
            },
            restartServ(uid) {
                let that = this
                let done = function (msg) {
                    that.refreshServs()
                    if(msg != "ok"){
                        alert(msg)
                    }
                }
                invoke(done, "RestartServ", uid)
            },
            refreshServs() {
                let that = this
                // that.servs = []
                let done = function (content) {
                    let data = JSON.parse(content)
                    if (data.length > 0) {
                        that.servs = data
                    } else {
                        alert("Server list is empty!")
                    }
                }
                invoke(done, 'RefreshServs')
            }
        },
        mounted() {
            this.refreshServs()
        }
    })
    app.mount('#app')
</script>

</html>
]]
end

Main()
```