<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VideoMonitor.aspx.cs" Inherits="PR.WizPlant.Integrate.WcfHost.VideoMonitor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <%--<script>
        //全局变量定义
        var m_iNowChanNo = -1;                           //当前通道号
        var m_iLoginUserId = -1;                         //注册设备用户ID
        var m_iChannelNum = -1;							 //模拟通道总数
        var m_bDVRControl = null;						 //OCX控件对象
        var m_iProtocolType = 0;                         //协议类型，0 – TCP， 1 - UDP
        var m_iStreamType = 0;                           //码流类型，0 表示主码流， 1 表示子码流
        var m_iPlay = 0;                                 //当前是否正在预览

        var videoPlayer = null;
        function initVideo() {
            if ("<%=port%>" == "8000") {
                document.getElementById("MonitorDiv").style.display = "block";
                document.getElementById("MonitorDivDaHua").style.display = "none";
                //海康威视监控
                videoPlayer = document.getElementById("HIKOBJECT1");
                if (videoPlayer && videoPlayer.object) {
                    play("<%=host%>", "<%=port%>", "<%=camera%>", "<%=userName%>", "<%=password%>");
                }
            }
            else {
                document.getElementById("MonitorDiv").style.display = "none";
                document.getElementById("MonitorDivDaHua").style.display = "block";
                //大华监控
                StartPreview("<%=host%>", "<%=port%>", "<%=camera%>", "<%=userName%>", "<%=password%>");
            }
        }

        function play(monitorIp,monitorPort, channelNo, userName,password) {
            var m_iLoginUserId = videoPlayer.Login(monitorIp, monitorPort, userName, password);
            if (m_iLoginUserId == -1) {
                document.getElementById("MonitorDiv").innerHTML = '<font color=red>注册失败！</font>';
                return;
            }
            var m_iNowChanNo = parseInt(channelNo);//通道号

            var bRet = videoPlayer.StartRealPlay(m_iNowChanNo, m_iProtocolType, m_iStreamType);
            if (bRet) {
                var str = "预览通道" + channelNo + "成功！";
                m_iPlay = 1;
            }
            else {
                document.getElementById("MonitorDiv").innerHTML = '<font color=red>通道 ' + channelNo + ' 预览失败！</font>';
            }
        }

        function StartPreview(monitorIp, monitorPort, channelNo, userName, password) {
            var SSOcx = document.getElementById("playOcx");
            SSOcx.SetDeviceInfo(monitorIp, monitorPort, channelNo, userName, password);
            SSOcx.StartPlay();
        }
    </script>--%>
</head>
<body onload="initVideo()" style="margin:0">
    <%--<b>视频</b>--%>
    <%--<div id="MonitorDiv" style="position:absolute;width:100%;height:100%">
        <object classid="CLSID:CAFCF48D-8E34-4490-8154-026191D73924" codebase="lib/NetVideoActiveX23.cab#version=2,3,11,2" standby="Waiting..." id="HIKOBJECT1" width="100%" height="100%" name="HIKOBJECT1" ></object>
    </div>
    <div id="MonitorDivDaHua" style="position:absolute;width:100%;height:100%">
        <object classid="clsid:30209FBC-57EB-4F87-BF3E-740E3D8019D2" codebase="lib/RealPlayX.CAB#version=1,0,0,1" standby="Waiting..." id="playOcx" width="100%" height="100%" name="playOcx" align="center" ></object>
    </div>--%>
    
    <object type='application/x-vlc-plugin' pluginspage="http://www.videolan.org/" id='vlc' events='false' style="width:100%;height:100%">
        <param name='mrl' value='rtsp://<%=userName%>:<%=password%>@<%=host%>:<%=port%>/MPEG-4/ch1/main/av_stream' />
        <param name='volume' value='50' />
        <param name='autoplay' value='true' />
        <param name='loop' value='false' />
        <param name='fullscreen' value='false' />
        <param name='controls' value='false' />
    </object>
</body>
</html>
