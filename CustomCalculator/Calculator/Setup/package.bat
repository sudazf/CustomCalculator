::以管理员运行
%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit
cd /d "%~dp0"

::解决中文乱码
chcp 65001

::不要显示指令
@echo off

::删除文件
::echo 'remove all files in %~dp0..\bin\Debug\Config\Data\DeviceConfigFileTemplates'
::del /S /Q %~dp0..\bin\Debug\Config\Data\DeviceConfigFileTemplates\*.*

::拷贝文件
::echo 'copy pulse templates from %~dp0..\Config\Data\DeviceConfigFileTemplates to %~dp0..\bin\Debug\Config\Data\DeviceConfigFileTemplates'
copy %~dp0LicenseAgreement.rtf %~dp0..\bin\Debug  /y
copy %~dp0..\Data\Pig.ico %~dp0..\bin\Debug\logo.ico  /y

::调用 inno setup 开始打包
echo 'packaging...'
iscc %~dp0setup.iss

::显示输出文件夹
start "" "%~dp0Output"

pause