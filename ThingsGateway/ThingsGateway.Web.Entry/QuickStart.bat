@echo off
@echo =====================
@echo 提醒：右键用管理员方式打开。
@echo =====================

set netpath=%~dp0
set webpath=%~dp0WebQuickStart\nginx
rem 打开nginx的存放路径
cd/d   %webpath%
rem 修改配置后重新加载生效，所以如果更新了文件夹或者conf配置，这行需要首先运行
nginx -s reload
rem 正常停止或关闭。先保存上面一步，在停止nginx在启动，所以其次运行
nginx -s quit
rem 根据程序名来关闭进程。这里强制关闭nginx.exe，防止上一步未停止nginx。注：这个命令会杀掉所以正在运行的nginx
rem taskkill /f /t /im nginx.exe
start nginx
nginx -V
rem 启动命令行。放在最后就是起显示作用，上面执行了加上这行可以显示
cd/d %netpath%
ThingsGateway.Web.Entry.exe
