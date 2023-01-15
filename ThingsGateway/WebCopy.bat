@echo off
color 0e
@echo =====================
@echo 提醒：右键用管理员方式打开。
@echo =====================




set CPath=%~dp0

 set dir4=%CPath%WebQuickStart
 set dir5=%CPath%ThingsGateway.Web.Entry\bin\Release\net6.0\WebQuickStart
 set dir6=%CPath%ThingsGateway.Web.Entry\bin\Release\net7.0\WebQuickStart

set webpath=%dir5%\nginx
rem 打开nginx的存放路径
cd/d   %webpath%
rem 修改配置后重新加载生效，所以如果更新了文件夹或者conf配置，这行需要首先运行
nginx -s reload
rem 正常停止或关闭。先保存上面一步，在停止nginx在启动，所以其次运行
nginx -s quit

set webpath=%dir6%\nginx
rem 打开nginx的存放路径
cd/d   %webpath%
rem 修改配置后重新加载生效，所以如果更新了文件夹或者conf配置，这行需要首先运行
nginx -s reload
rem 正常停止或关闭。先保存上面一步，在停止nginx在启动，所以其次运行
nginx -s quit

 if not exist %dir5% ( md %dir5% )
xcopy "%dir4%"  "%dir5%" /e /s /Y
 if not exist %dir6% ( md %dir6% )
xcopy "%dir4%"  "%dir6%" /e /s /Y
@echo off
 
set CurrentPath=%~dp0
set P1Path=
set P2Path=
set P3Path=
set P4Path=
set P5Path=
 
:begin
for /f "tokens=1,* delims=\" %%i in ("%CurrentPath%") do (set content=%%i&&set CurrentPath=%%j)
if "%P1Path%%content%\" == "%~dp0" goto end
 

set P1Path=%P1Path%%content%\
 
goto begin
 
 
:end
echo BatPath=%~dp0
echo P1Path=%P1Path%

pause

set dist=%P1Path%Web\dist
@echo %dist%
echo d | xcopy "%dist%"  "%dir5%\nginx\html\dist" /e /s /Y
echo d | xcopy "%dist%"  "%dir6%\nginx\html\dist" /e /s /Y

cmd