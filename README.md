<div align="center"><h1 align="center">ThingsGateway</a></h1></div>
<div align="center"><h3 align="center">工业数采网关</h3></div>

<div align="center">

[![GitHub license](https://img.shields.io/badge/license-Apache2-yellow)](https://gitee.com/dotnetchina/Furion/blob/master/LICENSE)

</div>

### 🎁 介绍

后端.NET6/7(Furion/SqlSugar)，
前端Vue3/Element-plus，
前后端分离。

</li>&#x000A;<li>
核心模块包括：<br>
1、采集/上传设备管理、变量管理；支持导入导出；支持驱动插件自定义开发<br>
2、报警服务：实时报警管理、历史报警存储。<br>
3、时序数据存储。<br>

### 😎 开发计划
- 编写采集插件使用文档，添加Gitee Pages作为使用文档
- 编写OPCUAClient驱动


### 😎 源码集成
- AspNetCore框架集成于Admin.Net，并精简部分内容
- 👉[Admin.NET](https://gitee.com/zuohuaijun/Admin.NET)
- 采集驱动插件的Socket框架集成于TouchSocket，添加自定义数据插件与BitConvter等
- 👉[TouchSocket](https://gitee.com/dotnetchina/TouchSocket)

### 🍿 在线体验
- 开发账号：用户名：superadmin，密码：123456          
- 使用账号：用户名：admin，密码：123456          

- 地址：[http://120.24.62.140/](http://120.24.62.140/) 1核2G配置
服务器带有ModbusSlave (MODBUSTCP 站号1 功能码01-04) , 无关系库，时序库

### 😎 驱动贡献
- 查看Modbus插件源码，以驱动插件abstract实现规则，添加对应驱动插件代码

### 🍎 效果图

<table>
    <tr>
        <td><img src="https://gitee.com/diego2098/ThingsGateway/raw/master/Image/1.png"/></td>
        <td><img src="https://gitee.com/diego2098/ThingsGateway/raw/master/Image/2.png"/></td>
        <td><img src="https://gitee.com/diego2098/ThingsGateway/raw/master/Image/3.png"/></td>
    </tr>
    <tr>
        <td><img src="https://gitee.com/diego2098/ThingsGateway/raw/master/Image/4.png"/></td>
        <td><img src="https://gitee.com/diego2098/ThingsGateway/raw/master/Image/5.png"/></td>
    </tr>
</table>

### 🍄 快速启动

* 后端：需要安装visual studio 2022开发环境

	打开ThingsGateway/ThingsGateway.sln解决方案，直接运行（F5）即可启动（数据库默认SQLite）

* 前端：需要安装nodejs、cnpm、vscode等开发环境

	打开Web文件夹，1、安装依赖cnpm install .2、运行cnpm run dev.3、打包cnpm run build

* 插件集成：源码包含Drivers和Uploads文件夹，复制到运行目录即可（路径可配置）

* 编译包：(https://gitee.com/diego2098/ThingsGateway/releases)

    直接运行后端，前端可部署iis快速启动

* 浏览器访问：`http://localhost:8888` （默认前端端口为：8888，后台端口为：5004）



### 📖 帮助文档

👉使用文档：（正在编写完善;）
* [ThingsGateway使用文档](https://diego2098.gitee.io/thingsgateway/)

👉后端文档：
* [Furion](https://dotnetchina.gitee.io/furion/docs/source)

👉前端文档：
* [vue-next-admin](https://lyt-top.gitee.io/vue-next-admin-doc-preview/)
* [Element Plus](https://element-plus.gitee.io/zh-CN/)


### 🥦 补充说明
* 开源项目中一部分采集/上传驱动插件源码不开放源码，但提供编译程序集（无限制）
* 集成后端管理框架Admin.NET，精简了多租户和代码生成功能，如果不需要可自行删减功能
* OPCUAServer插件需联系OPC基金会进行授权

###  🍄 现支持采集插件
* ModbusRtuOverTcp/Udp
* ModbusTcp/Udp
* Siemens-S7系列
* OPCDAClient(只支持win平台，需安装OPC Core Components Redistributable X64)
* 罗克韦尔Cip


###  🍄 现支持上传插件
* 默认Mqtt/json
* OPCUAServer
* IotSharp
* 中石化智能化油库数据采集(盈科)
* 中石化智能化油库双防平台数据采集


### 🍎 支持作者
<img src="https://gitee.com/diego2098/ThingsGateway/raw/master/Image/pay.png"/>


### 🍎 联系作者
* QQ群：[605534569]
* 邮箱：[2248356998@qq.com]