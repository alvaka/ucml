<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Page language="c#" CodeBehind="BPO_bpolist.aspx.cs" AutoEventWireup="false" Inherits="UCMLCommon.BPO_bpolist" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
	<html xmlns="http://www.w3.org/1999/xhtml" >
	<head runat="server">
		<title>业务单元列表</title>
<meta http-equiv="Content-Type" content="text/html; charset=gb2312"/>
<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
<meta http-equiv="X-UA-Compatible" content="IE=7" />
<script type="text/javascript" language="javascript" src="Model/rule/JSRule.js"></script>
<script type="text/javascript" language="javascript" src="Model/rule/initvalue.js"></script>
<script type="text/javascript" language="javascript" src="js/UCML_PublicApp.js"></script>
<script type="text/javascript" language="javascript" src="js/ig_shared.js"></script>
<script type="text/javascript" language="javascript" src="js/ig_edit.js"></script>
<script type="text/javascript" language="javascript" src="js/dnncore.js"></script>
<script type="text/javascript" language="javascript" src="js/dnn.js"></script>
<script type="text/javascript" language="javascript" src="js/dnn.dom.positioning.js"></script>
<script type="text/javascript" language="javascript" src="js/ucmlapp.js"></script>


   	<script type="text/javascript" language="javascript">
   	    var UCMLResourcePath="";
   	    var UCMLLocalResourcePath="";
   	    var BPOName="BPO_bpolist";
   	    function Init()
   	    {
            var dobject=window.document.all["UCMLBUSIOBJECT"];
            if (dobject==undefined) return;
            BPO_bpolistBPO.open();
            BC_UCMLClassDataSet_BPOBase.fIDENTITYKey = false;
            BC_UCMLClassDataSet_BPOBase.AllowModifyJION = false;
            BC_UCMLClassDataSet_BPOBase.fHaveUCMLKey = true;
            BC_UCMLClassDataSet_BPOBase.PrimaryKey = "UCMLClassDataSetOID";
            BC_UCMLClassDataSet_BPOBase.Columns = BPO_bpolistBPO.BC_UCMLClassDataSet_BPOColumns;
            BC_UCMLClassDataSet_BPOBase.BPOName = "BPO_bpolist";
            BC_UCMLClassDataSet_BPOBase.ChangeOnlyOwnerBy = false;
            BC_UCMLClassDataSet_BPOBase.AddConnectControls(VC_UCMLClassDataSet_BPOList);
            VC_UCMLClassDataSet_BPOList.BPOName="BPO_bpolist";
            VC_UCMLClassDataSet_BPOList.AppletName="VC_UCMLClassDataSet_BPOList";
            VC_UCMLClassDataSet_BPOList.EnabledEdit=true;
            VC_UCMLClassDataSet_BPOList.haveMenu=false;
            VC_UCMLClassDataSet_BPOList.parentNodeID="";
            VC_UCMLClassDataSet_BPOList.HiddenID="TabStrip_VC_UCMLClassDataSet_BPOList;MultiPage_VC_UCMLClassDataSet_BPOList";
            VC_UCMLClassDataSet_BPOList.fHidden="false";
            VC_UCMLClassDataSet_BPOList.alignHeight="true";
            VC_UCMLClassDataSet_BPOList.alignWidth="true";

VC_UCMLClassDataSet_BPOList.haveIndexStatusBar = "true";

            VC_UCMLClassDataSet_BPOList.open();
            BC_UCMLClassDataSet_BPOBase.EnabledEdit=true;
            BC_UCMLClassDataSet_BPOBase.EnabledAppend=true;
            BC_UCMLClassDataSet_BPOBase.EnabledDelete=true;
            BC_UCMLClassDataSet_BPOBase.RecordOwnerType=0;
            BC_UCMLClassDataSet_BPOBase.open();
            BPO_bpolistBPO.AddUseTable(BC_UCMLClassDataSet_BPOBase);
            BPO_bpolistBPO.InitBusinessEnv();
   	    }
   	</script>
	</head>
	<body  onload= "Init()" class="UCML-BODY" leftMargin="0" topMargin="0">


<Form id="Form1" method="post" runat="server">
<asp:Panel id="NaviPageBar" style="Z-INDEX: 100; LEFT: 0px;TOP: 0px" Height="20px" Width="100%" runat="server" Visible="False">
</asp:Panel>
<div id="MainPanel" style="Z-INDEX: 100; left: 0px;top: 0px" class="UCML-MailPanel">
<iewc:TabStrip id="TabStrip_VC_UCMLClassDataSet_BPOList" runat="server" TargetID="MultiPage_VC_UCMLClassDataSet_BPOList" CssClass="UCML-TAB" TabSelectedStyle="border-right:#aca899 1px solid;border-top:white 1px solid;background:#ece9d8;border-left:white 1px solid;color:#0;border-bottom:#aca899 1px solid;"  TabHoverStyle="border-right:#aca899 1px solid;border-top:white 1px solid;background:#ece9d8;border-left:white 1px solid;color:red;border-bottom:#aca899 1px solid;"  TabDefaultStyle="border-right:#aca899 1px solid;padding-right:2px;border-top:#aca899 1px solid;padding-left:2px;background:#ece9d8;padding-bottom:2px;border-left:#aca899 1px solid;padding-top:2px;border-bottom:#aca899 1px solid;" ForeColor="Black" BorderColor="CornflowerBlue" BackColor="PapayaWhip" Font-Names="Verdana" Font-Size="8pt" EnableViewState="False">
<iewc:Tab id="Tab_VC_UCMLClassDataSet_BPOList" runat="server" Text = "业务单元列表">
</iewc:Tab>
<iewc:TabSeparator>
</iewc:TabSeparator>
</iewc:TabStrip>
<iewc:MultiPage id="MultiPage_VC_UCMLClassDataSet_BPOList" Width="100%" runat="server">
<iewc:PageView id="PageView_10727" Text = "业务单元列表">
<asp:Panel id="VC_UCMLClassDataSet_BPOList_Module" style="overflow:hidden" CssClass="UCML-Panel" Width="100%" runat="server">
<iewc:Toolbar id="ToolBarVC_UCMLClassDataSet_BPOList" style="Z-INDEX: 102; LEFT: 0px; TOP: 0px" runat="server" Height="23px" Width="100%">
<iewc:ToolbarButton Text="新建..." ImageUrl="" ToolTip="">
</iewc:ToolbarButton>
<iewc:ToolbarSeparator>
</iewc:ToolbarSeparator>
<iewc:ToolbarButton Text="编辑" ImageUrl="" ToolTip="">
</iewc:ToolbarButton>
<iewc:ToolbarSeparator>
</iewc:ToolbarSeparator>
<iewc:ToolbarButton Text="删除" ImageUrl="" ToolTip="">
</iewc:ToolbarButton>
<iewc:ToolbarSeparator>
</iewc:ToolbarSeparator>
</iewc:Toolbar>
<div id="VC_UCMLClassDataSet_BPOList" style="BEHAVIOR:url(UCMLDBGrid.htc);width:100%;height:100%" title="业务单元列表">
</div>
</asp:Panel>
</iewc:PageView>
</iewc:MultiPage>
</div>
</Form>
<script language="javascript">
			document.write('<DIV style="BORDER-RIGHT: #0066ff 1px solid; BORDER-TOP: #0066ff 1px solid; Z-INDEX: 103; LEFT: 201px; VISIBILITY: hidden; BORDER-LEFT: #0066ff 1px solid; WIDTH: 272px; BORDER-BOTTOM: #0066ff 1px solid; POSITION: absolute; TOP: 229px; HEIGHT: 27px; BACKGROUND-COLOR: #ffffcc; TEXT-ALIGN: center" ms_positioning="FlowLayout" id="MsgPanel"></DIV>');
function document.oncontextmenu()
{
  event.returnValue = false; 
  event.cancelBubble = true; 
  return false; 
}
</script>

</body>
</html>
