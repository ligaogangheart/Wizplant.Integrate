<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OfficePlayer.aspx.cs" Inherits="PR.WizPlant.Web.SystemModule.DataModelCenter.OfficePlayer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript">
        var fileUrl = "<%=url%>";
        var fileType = "<%=fileType%>";

        function command_onclick() {
            if (fileType == ".doc" || fileType == ".docx") {
                document.all.FramerControl1.Open(fileUrl, false, "Word.Document");
            }
            if (fileType == ".xls" || fileType == ".xlsx") {
                document.all.FramerControl1.Open(fileUrl, false, "Excel.Sheet");
            }
            if (fileType == ".ppt" || fileType == ".pptx") {
                document.all.FramerControl1.Open(fileUrl, false, "PowerPoint.Show");
            }
            //完全保护文档，密码为"pwd"   
            //document.all.FramerControl1.ProtectDoc(1, 1, "pwd");
            ////解除文档保护                
            //document.all.FramerControl1.ProtectDoc(0, 1, "pwd");
        }

        //关闭时关闭进程
        window.onunload = function DsoClose() {
            document.all.FramerControl1.Close();
        }
    </script>
</head>
<body onload="command_onclick()">
    <%--codebase="../2.3.0.1/dsoframer.ocx#version=2,3,0,1"   --%>
            
        <object width="100%" height="1000" id="FramerControl1" classid="clsid:00460182-9E5E-11d5-B7C8-B8269041DD57">
        <param name="BorderStyle" value="1" />
        <param name="Titlebar" value="0" />
        <param name="Toolbars" value="0" />
        <param name="Menubar" value="0" />
    </object>
    
</body>
</html>
