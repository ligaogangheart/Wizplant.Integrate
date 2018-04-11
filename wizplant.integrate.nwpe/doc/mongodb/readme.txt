1.选安装mongodb.(windows版本：\\fs01.wizplant.online\sharedata\tools\DB\mongodb\mongodb-win32-x86_64-2008plus-ssl-3.4.7-signed.msi)
2.windows可选:当注册mongodb服务时提示（无法启动此程序，因为计算机中丢失 api-ms-win-crt-runtime-l1-1-0.dll。尝试重新安装该程序以解决此问题。 ）时，安装\\fs01.wizplant.online\sharedata\tools\DB\mongodb\vc_redist.x64.exe
3.安装mongodb实例服务
 3.1 为每个实例编写配置配置文件(参考mongodb1.conf)   
 3.2 为每个实例编写安装mongodb实例服务脚本和删除mongodb实例服务脚本(参考install_mongdbservice_scadadb1.bat和uninstall_mongdbservice_scadadb1.bat)
 3.3 执行每个实例的服务安装脚本
4.建立mongodb副本集(参考mongodb副本集.txt)
5.创建scadaDB数据库 (参考schema.txt)
 5.1 在客户端工具中执行脚本：use scadaDB;
 5.2 创建数据集合：ScadaData,CurrentScadaData (此两集合结构一样）
 5.3 创建数据集合索引
4.配置授权 (参考mongodb_权限控制设置.txt)
5.配置ssl （参考mongodb_ssl配置.txt)


