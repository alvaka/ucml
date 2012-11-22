﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlBPO
    {
        private string _Name;
        public AspxPage Page;
        private CSharpDoc _PageCs;
        private CSharpDoc PageDesignerCs;
        public CSharpDoc AsmxCs;
        private BpoPropertySet BpoPropSet;
        public  List<UcmlVcTabPage> VcTabList;
        private List<UcmlBusiCompPropSet> BCList;
        public string Namespace;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public CSharpDoc PageCs
        {
            get { return _PageCs; }
            private set { _PageCs = value; }
        }
        public UcmlBPO(BpoPropertySet bps,string ns)
        {
            this.Name = bps.Name;
            this.Namespace = ns;
            BpoPropSet = bps;
            Page = new AspxPage(bps.Name + ".aspx",bps.Capiton);
            PageCs = new CSharpDoc(bps.Name+".aspx.cs", Namespace);
            PageDesignerCs = new CSharpDoc(bps.Name + ".designer.cs", Namespace);
            AsmxCs = new CSharpDoc(this.Name, Namespace);

            VcTabList = new List<UcmlVcTabPage>();
            BCList = new List<UcmlBusiCompPropSet>();
        }

        public void AddVcTab(UcmlVcTabPage vcTab)
        { 
        }
        /// <summary>
        /// 构建Aspx页面
        /// </summary>
        /// <returns></returns>
        public bool BuildAspxPage()
        {
            //初始化页面，构造head,body,form等基本节点
            Page.InitPage();
            //添加页面指令
            AspxDirective direc4Page = new AspxDirective("Page");
            direc4Page["language"]= "C#";
            direc4Page["codeFile"]= Page.PageName+".cs";
            direc4Page["Inherits"]="UCMLCommon."+this.Name;
            direc4Page["AutoEvenWireup"]="False";

            AspxDirective direc4Reg = new AspxDirective("Register");
            direc4Reg["TagPrefix"]= "iewc";
            direc4Reg["Namespace"]="Microsoft.Web.UI.WebControls";
            direc4Reg["Assembly"]="Microsoft.Web.UI.WebControls";

            Page.Directives.Add(direc4Page);
            Page.Directives.Add(direc4Reg);
            #region 构建主页面
            //构建主页面
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                //TabStrip控件
                AspxNode tabStrip = new AspxNode("iewc:TabStrip");
                tabStrip["id"] = "TabStrip_"+vcTab.Name;
                tabStrip["TargetID"] = "MultiPage_" + vcTab.Name;
                tabStrip["CssClass"] = "UCML-TAB";
                tabStrip["TabSelectedStyle"] = "border-right:#aca899 1px solid;border-top:white 1px solid;background:#ece9d8;border-left:white 1px solid;color:#0;border-bottom:#aca899 1px solid;";
                tabStrip["TabHoverStyle"] = "border-right:#aca899 1px solid;border-top:white 1px solid;background:#ece9d8;border-left:white 1px solid;color:red;border-bottom:#aca899 1px solid;";
                tabStrip["TabDefaultStyle"] = "border-right:#aca899 1px solid;padding-right:2px;border-top:#aca899 1px solid;padding-left:2px;background:#ece9d8;padding-bottom:2px;border-left:#aca899 1px solid;padding-top:2px;border-bottom:#aca899 1px solid;";
                tabStrip["ForeColor"] = "Black";
                tabStrip["BorderColor"] = "CornflowerBlue";
                tabStrip["BackColor"] = "PapayaWhip";
                tabStrip["Font-Names"] = "Verdana";
                tabStrip["Font-Size"] = "8pt";
                tabStrip["EnableViewState"] = "False";
                //MultiPage控件
                AspxNode multiPage = new AspxNode("iewc:MultiPage");
                multiPage["id"] = "MultiPage_"+vcTab.Name;
                multiPage["width"] = "100%";//待改成变量

                //把TabStrip控件和MultiPage控件挂到MainPanelNode下
                Page.MainPanelNode.Append(tabStrip);
                Page.MainPanelNode.Append(multiPage);

                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    //添加Tab控件
                    AspxNode tab = new AspxNode("iewc:Tab");
                    tab["id"] = "Tab_"+vc.VCName;
                    tab["Text"] = vc.Caption;
                    //添加TabSeperator控件
                    HtmlNode tabSep = HtmlNode.CreateClosedNode("iewc:TabSeparator");
                    //挂到tabStrip下
                    tabStrip.Append(tab);
                    tabStrip.Append(tabSep);

                    //添加PageView控件
                    AspxNode pageView = new AspxNode("iewc:PageView");
                    pageView["id"] = "PageView_"+vc.VCName;
                    pageView["Text"] = vc.Caption;
                    //添加Panel控件
                    AspxNode panel = new AspxNode("asp:Panel");
                    panel["id"] = vc.VCName+"_Module";
                    panel["style"] = "overflow:hidden";
                    panel["CssClass"] = "UCML-Panel";
                    panel["width"] = "100%";//待改成变量
                    //挂载PageView到MultiPage
                    multiPage.Append(pageView);
                    //挂载Panel到PageView下
                    pageView.Append(panel);
                    //添加ToolBar
                    //添加ToolButton
                    //挂载VC到panel下
                    panel.Append(vc.VCNode);
                    //添加VCName Div
                    HtmlNode div = new HtmlNode("div");
                    div["id"] = vc.VCName;
                    div["style"] = "BEHAVIOR:url(UCMLDBGrid.htc);width:100%;height:200";
                    div["title"] = vc.Caption;
                    panel.Append(div);
                    //添加ContextMenu
                    HtmlNode span = new HtmlNode("span");
                    span["id"] = "theContextMenu"+vc.VCName;
                    span["style"] = "Z-INDEX: 3103; LEFT: 0px; VISIBILITY: hidden; BEHAVIOR: url(menubar.htc); WIDTH: 200px; POSITION: absolute; TOP: 0px; HEIGHT: 20px";
                    div.Append(span);
                }
            #endregion 构建主页面
            }
            return true; 
        }
        public bool BuildPageCs()
        { 
            //添加using 引用
            PageCs.ReferenceNS.Add("System.Drawing");
            PageCs.ReferenceNS.Add("System.Data");
            PageCs.ReferenceNS.Add("System.Web");
            PageCs.ReferenceNS.Add("System.Web.UI");
            PageCs.ReferenceNS.Add("System.Web.UI.WebControls");
            PageCs.ReferenceNS.Add("System.Web.UI.HtmlControls");
            PageCs.ReferenceNS.Add("Microsoft.Web.UI.WebControls");
            PageCs.ReferenceNS.Add("System.Data.SqlClient");
            PageCs.ReferenceNS.Add("DBLayer");
            PageCs.ReferenceNS.Add("System.Collections.Specialized");

            //类定义
            CSharpClass bpoClass = new CSharpClass(this.Name);
            bpoClass.BaseClass = "UCMLCommon.BPObject";
            bpoClass.AccessAuth = AccessAuthority.PUBLIC;
            bpoClass.IsPartial = true;
            //添加字段
            //BPONameService字段
            CSharpClassField field = new CSharpClassField(this.Name + "Service", "busiObj");
            field.AccessAuth = AccessAuthority.PRIVATE;
            field.InitStatment = "null";
            bpoClass.FieldList.Add(field);
            //添加FBPODataSet字段
            field = new CSharpClassField("DataSet", "FBPODataSet");
            field.AccessAuth = AccessAuthority.PRIVATE;
            field.InitStatment = "null";
            bpoClass.FieldList.Add(field);
            //添加BPONameBPO字段
            field = new CSharpClassField("System.Web.UI.WebControls.Panel", this.Name + "BPO");
            field.AccessAuth = AccessAuthority.PROTECTED;
            bpoClass.FieldList.Add(field);
            //添加BPONameService字段
            field = new CSharpClassField("System.Web.UI.WebControls.Panel", this.Name + "Service");
            field.AccessAuth = AccessAuthority.PROTECTED;
            bpoClass.FieldList.Add(field);
            //添加BC字段
            foreach (UcmlBusiCompPropSet bc in BCList)
            {
                //添加BCNameBase字段
                field = new CSharpClassField("System.Web.UI.WebControls.Panel",  bc.Name+ "Base");
                field.AccessAuth = AccessAuthority.PROTECTED;
                bpoClass.FieldList.Add(field);

                //添加BCName字段
                field = new CSharpClassField("System.Web.UI.WebControls.Panel", bc.Name);
                field.AccessAuth = AccessAuthority.PROTECTED;
                bpoClass.FieldList.Add(field);

                //添加BCNameService字段
                field = new CSharpClassField("System.Web.UI.WebControls.Panel", bc.Name + "Service");
                field.AccessAuth = AccessAuthority.PROTECTED;
                bpoClass.FieldList.Add(field);

                //添加BCNameColumn字段
                field = new CSharpClassField("UCMLCommon.BusinessColumn[]",bc.Name+"Column");
                field.AccessAuth = AccessAuthority.PUBLIC;
                bpoClass.FieldList.Add(field);

                //添加BCNameCondiColumn字段
                field = new CSharpClassField("UCMLCommon.BusinessColumn[]", bc.Name + "CondiColumn");
                field.AccessAuth = AccessAuthority.PUBLIC;
                bpoClass.FieldList.Add(field);
                
            }
            //添加column字段
            field = new CSharpClassField("UCMLCommon.BusinessColumn", "column");
            field.AccessAuth = AccessAuthority.PRIVATE;
            bpoClass.FieldList.Add(field);

            //添加UseEntityClass字段
            field = new CSharpClassField("UCMLCommon.UseEntityClass[]", "UseEntityArray");
            field.AccessAuth = AccessAuthority.PRIVATE;
            bpoClass.FieldList.Add(field);

            //添加业务链接组件BC字段
            
            //添加属性
            //添加ResourceData属性
            CSharpClassAttribute rsData = new CSharpClassAttribute("ResourceData");
            rsData.AccessAuth = AccessAuthority.PRIVATE;
            rsData.Type = "DataTable";
            rsData.GetStatment.AppendLine("return this.busiObj.BPODataSet.Tables[\"ResourceData\"];");
            bpoClass.AttributeList.Add(rsData);
            //添加PageCaption属性
            CSharpClassAttribute pageCaption = new CSharpClassAttribute("PageCaption");
            pageCaption.AccessAuth = AccessAuthority.PROTECTED;
            pageCaption.IsOverride = true;
            pageCaption.Type = "string";
            pageCaption.GetStatment.AppendLine("if (langugeKind==\"1\")");
            pageCaption.GetStatment.AppendLine("{");
            pageCaption.GetStatment.AppendLine("return \""+BpoPropSet.Capiton+"\";");
            pageCaption.GetStatment.AppendLine("}");
            pageCaption.GetStatment.AppendLine("else if(langugeKind==\"2\")");
            pageCaption.GetStatment.AppendLine("{");
            pageCaption.GetStatment.AppendLine("return \"\";");
            pageCaption.GetStatment.AppendLine("}");
            pageCaption.GetStatment.AppendLine("else if(langugeKind==\"3\")");
            pageCaption.GetStatment.AppendLine("{");
            pageCaption.GetStatment.AppendLine("return \"\";");
            pageCaption.GetStatment.AppendLine("}");

            bpoClass.AttributeList.Add(pageCaption);
            //添加Page_Load函数
            CSharpFunction pageLoad = new CSharpFunction("Page_Load");
            pageLoad.AccessAuth = AccessAuthority.PRIVATE;
            pageLoad.ReturnType = "void";
            pageLoad["Object"] = "sender";
            pageLoad["System.EventArgs"] = "e";
            //添加函数体
            pageLoad.Content.AppendLine("if(IsPostBack==false)");
            pageLoad.Content.AppendLine("{");
            pageLoad.Content.AppendLine("    this.InitControls();");
            pageLoad.Content.AppendLine("    DoHttpGet();");
            pageLoad.Content.AppendLine("    RepositionApplet();");
            pageLoad.Content.AppendLine("}");
            pageLoad.Content.AppendLine("else");
            pageLoad.Content.AppendLine("{");
            pageLoad.Content.AppendLine("    DoHttpPost();");
            pageLoad.Content.AppendLine("    IOC_HttpPost();");
            pageLoad.Content.AppendLine("}");

            bpoClass.AddFunction(pageLoad);
            
            //添加OnInit()函数
            CSharpFunction onInit = new CSharpFunction("OnInit");
            onInit.AccessAuth = AccessAuthority.PROTECTED;
            onInit.IsOverride = true;
            onInit.ReturnType = "void";
            onInit["EventArgs"] = "e";
            onInit.Content.AppendLine("UCMLInitializeComponent();");
            onInit.Content.AppendLine("InitializeComponent();");
            onInit.Content.AppendLine("base.OnInit(e);");
            bpoClass.AddFunction(onInit);

            //添加 InitializeComponent函数
            CSharpFunction initComp = new CSharpFunction("InitializeComponent");
            initComp.AccessAuth = AccessAuthority.PRIVATE;
            initComp.ReturnType = "void";
            initComp.Content.AppendLine("this.Load += new System.EventHandler(this.Page_Load);");
            bpoClass.AddFunction(initComp);

            //添加InitControls函数
            CSharpFunction initCtr = new CSharpFunction("InitControls");
            initCtr.IsOverride = true;
            initCtr.AccessAuth = AccessAuthority.PUBLIC;
            initCtr.ReturnType = "void";
            #region InitControls 函数体
            initCtr.Content.AppendLine("InFlow=false;");
            initCtr.Content.AppendLine("fSystemBPO="+BpoPropSet.fSystemBPO+";");
            initCtr.Content.AppendLine("fHavePageNavi="+BpoPropSet.fHavePageNavi+";");
            initCtr.Content.AppendLine("fRegisterBPO="+BpoPropSet.fRegisterBPO+";");
            initCtr.Content.AppendLine("fMutiLangugeSupport="+BpoPropSet.fMutiLangugeSupport+";");
            initCtr.Content.AppendLine("fXHTMLForm="+BpoPropSet.fXHTMLForm+";");
            initCtr.Content.AppendLine("fEnableConfig="+BpoPropSet.fEnableConfig+";");
            initCtr.Content.AppendLine("fUseSkin="+BpoPropSet.fUseSkin+";");
            initCtr.Content.AppendLine("RootPath=@\"\";");
            initCtr.Content.AppendLine("LocalResourcePath=\"\";");
            initCtr.Content.AppendLine("SkinSrc=\"\";");
            initCtr.Content.AppendLine("if (fRegisterBPO == true && UCMLCommon.UCMLInitEnv.fInit==false)");
            initCtr.Content.AppendLine("{");
            initCtr.Content.AppendLine("    new DBLayer.LogicDBModel();");
            initCtr.Content.AppendLine("    UCMLCommon.UCMLLogicDBModelApp x = new UCMLCommon.UCMLLogicDBModelApp();");
            initCtr.Content.AppendLine("    x.PrepareModel();");
            initCtr.Content.AppendLine("    UCMLCommon.UCMLInitEnv.Server = this.Server;");
            initCtr.Content.AppendLine("    UCMLCommon.UCMLInitEnv.LoadEnvVariable();");
            initCtr.Content.AppendLine("}");
            initCtr.Content.AppendLine("BPOName="+this.Name+";");
            initCtr.Content.AppendLine("BPOCaption="+BpoPropSet.Capiton+";");
            initCtr.Content.AppendLine("base.InitControls();");
            initCtr.Content.AppendLine("if (fHavePageNavi==true || this.Request.QueryString[\"NaviPageBar\"]==\"1\")");
            initCtr.Content.AppendLine("{");
            initCtr.Content.AppendLine("    this.NaviPageBar.Visible=true;");
            initCtr.Content.AppendLine("    UCMLCommon.PageNaviControl ctrl = (UCMLCommon.PageNaviControl)this.LoadControl(\"OtherSource\\PageNaviControl.ascx\");");
            initCtr.Content.AppendLine("    ctrl.BPOName = BPOCaption;");
            initCtr.Content.AppendLine("    this.NaviPageBar.Controls.Add(ctrl);");
            initCtr.Content.AppendLine("}");
            initCtr.Content.AppendLine("PrepareColumn();");
            initCtr.Content.AppendLine("RegisterUseTable();");
            //添加BPOnameBPO标签
            string BponameBPO=this.Name+"BPO";
            initCtr.Content.AppendLine(BponameBPO+"= new System.Web.UI.WebControls.Panel();");
            initCtr.Content.AppendLine(BponameBPO+".ID = "+BponameBPO+";");
            initCtr.Content.AppendLine(BponameBPO+".Style.Add(\"BEHAVIOR\",\"url("+BponameBPO+".htc)\");");
            initCtr.Content.AppendLine(BponameBPO+".Attributes.Add(\"ServiceID\",\""+this.Name+"Service\");");
            initCtr.Content.AppendLine("NameValueCollection coll=Request.QueryString; ");
            initCtr.Content.AppendLine("String[] keys = coll.AllKeys;");
            initCtr.Content.AppendLine("for (int i = 0; i < keys.Length; i++) ");
            initCtr.Content.AppendLine("{");
            initCtr.Content.AppendLine("String[] values = coll.GetValues(keys[i]);");
            initCtr.Content.AppendLine(BponameBPO+".Attributes.Add(keys[i],values[0]);");
            initCtr.Content.AppendLine("}");
            initCtr.Content.AppendLine("this.Controls.Add("+BponameBPO+");");

            //添加BPONameService标签
            string BponameService = this.Name+"Service";
            initCtr.Content.AppendLine(BponameService+"= new System.Web.UI.WebControls.Panel();");
            initCtr.Content.AppendLine(BponameService+".ID =\""+BponameService+"\"");
            initCtr.Content.AppendLine(BponameService+".Style.Add(\"BEHAVIOR\",\"url(\"+LocalResourcePath+\"htc/webservice.htc)\");");
            initCtr.Content.AppendLine(".Attributes.Add(\"onresult\",\""+BponameBPO+".LoadResults()\");");
            initCtr.Content.AppendLine("this.Controls.Add("+BponameService+");");
            initCtr.Content.AppendLine("");
            //添加BC Panel
            foreach (UcmlBusiCompPropSet bc in BCList)
            {
                
                string bcNameService = bc.Name + "Service";
                string bcNameBase = bc.Name + "Base";
                string bcName = bc.Name;
                //添加BCNameService
                initCtr.Content.AppendLine(bcNameService+"= new System.Web.UI.WebControls.Panel();");
                initCtr.Content.AppendLine(bcNameService+".ID = \""+bcNameService+"\";");
                initCtr.Content.AppendLine(bcNameService+".Style.Add(\"BEHAVIOR\",\"url(\"+LocalResourcePath+\"htc/webservice.htc)\");");
                initCtr.Content.AppendLine(bcNameService+".Attributes.Add(\"onresult\",\""+bcNameBase+".LoadResults()\");");
                initCtr.Content.AppendLine(" this.Controls.Add("+bcNameService+");");
                initCtr.Content.AppendLine();
                //添加BCNameBase
                initCtr.Content.AppendLine(bcNameBase+"= new System.Web.UI.WebControls.Panel();");
                initCtr.Content.AppendLine(bcNameBase+".ID = \""+bcNameBase+"\";");
                initCtr.Content.AppendLine(bcNameBase+".Style.Add(\"BEHAVIOR\",\"url(\"+LocalResourcePath+\"UCMLTable.htc)\");");
                initCtr.Content.AppendLine(bcNameBase+".Attributes.Add(\"ServiceID\",\""+bcNameService+"\");");
                initCtr.Content.AppendLine(bcNameBase+".Attributes.Add(\"TableName\",\""+bc.TableName+")\";");
                initCtr.Content.AppendLine(bcNameBase+".Attributes.Add(\"BCName\",\""+bc.Name+"\");");
                initCtr.Content.AppendLine("this.Controls.Add("+bcNameBase+");");
                initCtr.Content.AppendLine("");
                //添加BCName
                initCtr.Content.AppendLine(bcName+"= new System.Web.UI.WebControls.Panel();");
                initCtr.Content.AppendLine(bcName+".ID = \""+bcName+"\";");
                initCtr.Content.AppendLine(bcName+".Style.Add(\"BEHAVIOR\",\"url(\"+LocalResourcePath+\"Model/HTC/"+bc.TableName+".htc)\");");
                initCtr.Content.AppendLine(bcName+".Attributes.Add(\"DataTable\",\""+bcNameBase+"\");");
                initCtr.Content.AppendLine(" this.Controls.Add("+bcName+");");
               
                initCtr.Content.AppendLine("");
            }
            int vcNum = 0;
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    vcNum++;
                }
            }

            initCtr.Content.AppendLine("AppletLinkArray = new UCMLCommon.UCMLPortal.AppletLinkInfo["+vcNum+"];");
            initCtr.Content.AppendLine("UCMLCommon.UCMLPortal.AppletLinkInfo appletLink = null;");
            int i = 0;
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    initCtr.Content.AppendLine("appletLink = new UCMLCommon.UCMLPortal.AppletLinkInfo();");
                    initCtr.Content.AppendLine("appletLink.AppletName = \""+vc.VCName+"\";");
                    initCtr.Content.AppendLine("appletLink.Caption = \""+vc.Caption+"\";");
                    initCtr.Content.AppendLine("appletLink.Pane = \"\";");
                    initCtr.Content.AppendLine("appletLink.fTreeGridMode = "+vc.fTreeGridMode+";");
                    initCtr.Content.AppendLine("appletLink.fSubTableTreeMode = "+vc.fSubTableTreeMode+";");
                    initCtr.Content.AppendLine("appletLink.ImageLink = \""+vc.ImageLink+"\";");
                    initCtr.Content.AppendLine("appletLink.SubBCs = \""+vc.SubBCs+"\";");
                    initCtr.Content.AppendLine("appletLink.SubParentFields = \""+vc.SubParentFields+"\";");
                    initCtr.Content.AppendLine("appletLink.SubLabelFields = \""+vc.SubLabelFields+"\";");
                    initCtr.Content.AppendLine("appletLink.SubPicFields = \""+vc.SubPicFields+"\";");
                    initCtr.Content.AppendLine("appletLink.SubFKFields = \""+vc.SubFKFields+"\";");
                    initCtr.Content.AppendLine("AppletLinkArray["+i+"] = appletLink;");
                    initCtr.Content.AppendLine("");
                    i++;
                }
            }
            initCtr.Content.AppendLine("InjectModule();");
            initCtr.Content.AppendLine("");
            //实例化业务逻辑类
            initCtr.Content.AppendLine("DataSet ds = null;");
            initCtr.Content.AppendLine("busiObj = UCMLBusinessObjectFactory.CreateBusinessObject(this.BPOName);");
            

            initCtr.Content.AppendLine("if (fEnableConfig==true)");
            initCtr.Content.AppendLine("{");
            initCtr.Content.AppendLine("    busiObj.AppletLinkArray = this.AppletLinkArray;");
            initCtr.Content.AppendLine("}");
            //读取数据
            initCtr.Content.AppendLine(" ds = busiObj.InitBusinessEnv();");
            initCtr.Content.AppendLine("InitViewComponent(busiObj);");
            initCtr.Content.AppendLine("IOC_HttpGet();");

            //GRID个性设置信息读取
            initCtr.Content.AppendLine("SysDBModel.PersonApplet obj = new SysDBModel.PersonApplet();");
            initCtr.Content.AppendLine("SysDBModel.PersonAppletInfo[] arrInfo = null;");
            initCtr.Content.AppendLine("System.Web.UI.HtmlControls.HtmlInputHidden InputHidden = null;");
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    initCtr.Content.AppendLine("arrInfo =  obj.ReadByPersonVC(busiObj.loginUserInfo.UserOID, \""+vc.VCName+"\");");
                    initCtr.Content.AppendLine("InputHidden = new System.Web.UI.HtmlControls.HtmlInputHidden();");
                    initCtr.Content.AppendLine("if (arrInfo.Length > 0)");
                    initCtr.Content.AppendLine("    InputHidden.Value = arrInfo[0].FieldName;");
                    initCtr.Content.AppendLine("else");
                    initCtr.Content.AppendLine("    InputHidden.Value = \"\";");
                    initCtr.Content.AppendLine("InputHidden.ID = "+vc.VCName+"\"_ColumnSetup\";");
                    initCtr.Content.AppendLine("this.Controls.Add(InputHidden);");
                }
            }
            initCtr.Content.AppendLine("ReplaceResourceData();");

            initCtr.Content.AppendLine("this.Response.Write(\"<script language=javascript>var UCMLSkinPath='\"+this.SkinPath+\"';</script>\");");
            initCtr.Content.AppendLine("");
            //换肤风格构造
            initCtr.Content.AppendLine("HtmlLink myHtmlLink = new HtmlLink();");
            initCtr.Content.AppendLine("myHtmlLink.Href = this.LocalResourcePath+SkinPath + \"css/ucmlapp.css\";");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"rel\",\"stylesheet\");");
            initCtr.Content.AppendLine("Page.Header.Controls.AddAt(1,myHtmlLink);");
            initCtr.Content.AppendLine("myHtmlLink = new HtmlLink();");
            initCtr.Content.AppendLine("myHtmlLink.Href = this.LocalResourcePath+SkinPath + \"UCMLPortal/RES/module.css\";");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"rel\", \"stylesheet\");");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"type\",\"text/css\");");
            initCtr.Content.AppendLine("Page.Header.Controls.AddAt(2,myHtmlLink);");
            initCtr.Content.AppendLine("myHtmlLink = new HtmlLink();");
            initCtr.Content.AppendLine("myHtmlLink.Href = this.LocalResourcePath+SkinPath + \"UCMLPortal/RES/default.css\";");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"rel\", \"stylesheet\");");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"type\",\"text/css\");");
            initCtr.Content.AppendLine("Page.Header.Controls.AddAt(3,myHtmlLink);");
            initCtr.Content.AppendLine("myHtmlLink = new HtmlLink();");
            initCtr.Content.AppendLine("myHtmlLink.Href = this.LocalResourcePath+SkinPath + \"UCMLPortal/RES/skin.css\";");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"rel\", \"stylesheet\");");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"type\",\"text/css\");");
            initCtr.Content.AppendLine("Page.Header.Controls.AddAt(4,myHtmlLink);");
            initCtr.Content.AppendLine("myHtmlLink = new HtmlLink();");
            initCtr.Content.AppendLine("myHtmlLink.Href = this.LocalResourcePath+SkinPath + \"UCMLPortal/RES/container.css\";");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"rel\", \"stylesheet\");");
            initCtr.Content.AppendLine("myHtmlLink.Attributes.Add(\"type\",\"text/css\");");
            initCtr.Content.AppendLine("Page.Header.Controls.AddAt(5,myHtmlLink);");
            initCtr.Content.AppendLine("");

            initCtr.Content.AppendLine("string xml = ds.GetXml();");
            initCtr.Content.AppendLine("LiteralControl li = new LiteralControl();");
            initCtr.Content.AppendLine("li.Text = \"<xml id= UCMLBUSIOBJECT>\"+xml+\"</xml>\";");
            initCtr.Content.AppendLine("Page.Controls.Add(li);");
            initCtr.Content.AppendLine("if (fMutiLangugeSupport==true||UCMLCommon.UCMLInitEnv.fMutiLangugeSupport==true)");
            initCtr.Content.AppendLine("{");
            initCtr.Content.AppendLine("    this.Response.Write(\"<xml id= UCMLlanguge>\" + busiObj.LoadLanugeXml() + \"</xml>\");");
            initCtr.Content.AppendLine("    this.Response.Write(\"<title>\"+PageCaption+\"</title>\");");
            initCtr.Content.AppendLine("}");
            initCtr.Content.AppendLine("");
            #endregion InitControls
            bpoClass.AddFunction(initCtr);

            //RegisterUseTable函数
            CSharpFunction regUseTable = new CSharpFunction("RegisterUseTable");
            regUseTable.AccessAuth = AccessAuthority.PUBLIC;
            regUseTable.ReturnType = "void";
            //RegisterUseTable函数体
            bpoClass.AddFunction(regUseTable);

            //PrepareColumn函数
            CSharpFunction preColumn = new CSharpFunction("PrepareColumn");
            preColumn.ReturnType = "void";
            preColumn.AccessAuth = AccessAuthority.PUBLIC;
            preColumn.IsOverride = true;
            //PrepareColumn函数体

            bpoClass.AddFunction(preColumn);

            //添加InitTreeNodePage函数
            CSharpFunction initTree = new CSharpFunction("InitTreeNodePage");
            initTree.AccessAuth = AccessAuthority.PUBLIC;
            initTree.IsOverride = true;
            initTree.ReturnType = "void";

            bpoClass.AddFunction(initTree);

            //添加IOC_HttpGet函数
            CSharpFunction iocGet = new CSharpFunction("IOC_HttpGet");
            iocGet.AccessAuth = AccessAuthority.PRIVATE;
            iocGet.ReturnType = "void";
            bpoClass.AddFunction(iocGet);

            //添加IOC_HttpPost函数
            CSharpFunction iocPost = new CSharpFunction("IOC_HttpPost");
            iocPost.AccessAuth = AccessAuthority.PRIVATE;
            iocPost.ReturnType = "void";
            bpoClass.AddFunction(iocPost);

            //添加RePositionApplet函数
            CSharpFunction rpApplet = new CSharpFunction("RePositionApplet");
            rpApplet.AccessAuth = AccessAuthority.PRIVATE;
            rpApplet.ReturnType = "void";
            bpoClass.AddFunction(rpApplet);

            //添加ReplaceResourceData函数
            CSharpFunction rrData = new CSharpFunction("ReplaceResourceData");
            rrData.AccessAuth = AccessAuthority.PUBLIC;
            rrData.ReturnType = "void";
            rrData.Content.AppendLine("if (fMutiLangugeSupport==false&&UCMLCommon.UCMLInitEnv.fMutiLangugeSupport==false) return;");
            rrData.Content.AppendLine("DataRow row=null;");
            rrData.Content.AppendLine("if (langugeKind==\"1\")");
            rrData.Content.AppendLine("{");
            rrData.Content.AppendLine("}");
            rrData.Content.AppendLine("else if (langugeKind==\"2\")");
            rrData.Content.AppendLine("{");
            rrData.Content.AppendLine("}");
            rrData.Content.AppendLine("");
            rrData.Content.AppendLine("else if (langugeKind==\"3\")");
            rrData.Content.AppendLine("{");
            rrData.Content.AppendLine("}");
            bpoClass.AddFunction(rrData);
            //添加类到Doc
            PageCs.InnerClass.Add(bpoClass);
            return true; 
        }
        public bool BuildPageDesignCs()
        {
            CSharpClass bpoClass = new CSharpClass(this.Name);
            bpoClass.IsPartial = true;
            bpoClass.AccessAuth = AccessAuthority.PUBLIC;
            //字段定义
            //NaviPageBar Panel字段
            CSharpClassField field = new CSharpClassField("System.Web.UI.WebControls.Panel", "NaviPageBar");
            field.AccessAuth = AccessAuthority.PROTECTED;
            bpoClass.FieldList.Add(field);
            
            foreach (UcmlVcTabPage tab in this.VcTabList)
            {
                //TabStrip字段定义
                field = new CSharpClassField("Microsoft.Web.UI.WebControls.TabStrip", "TabStrip_" + tab.Name);
                field.AccessAuth = AccessAuthority.PROTECTED;
                bpoClass.FieldList.Add(field);
                //MultiView
                field = new CSharpClassField("Microsoft.Web.UI.WebControls.MultiPage", "MultiPage_" + tab.Name);
                field.AccessAuth = AccessAuthority.PROTECTED;
                bpoClass.FieldList.Add(field);
                //Panel定义
                foreach (UcmlViewCompnent vc in tab.VCList)
                {
                    field = new CSharpClassField("System.Web.UI.WebControls.Panel", vc.VCName+"_Module");
                    field.AccessAuth = AccessAuthority.PROTECTED;
                    bpoClass.FieldList.Add(field);
                }
            }
            return true; 
        }
        public bool BuildAsmxCs()
        {
            AsmxCs.ReferenceNS.Add("System.Collections;");
            AsmxCs.ReferenceNS.Add("System.Data");
            AsmxCs.ReferenceNS.Add("System.Xml");
            AsmxCs.ReferenceNS.Add("System.ComponentModel");
            AsmxCs.ReferenceNS.Add("System.Web.Services");
            AsmxCs.ReferenceNS.Add("System.Web");
            AsmxCs.ReferenceNS.Add("System.Data.SqlClient");
            AsmxCs.ReferenceNS.Add("System.Diagnostics");
            AsmxCs.ReferenceNS.Add("DBLayer");

            CSharpClass asmxClass = new CSharpClass(this.Name+"Service");
            asmxClass.AccessAuth = AccessAuthority.PUBLIC;
            asmxClass.IsPartial = true;
            asmxClass.BaseClass = "UCMLCommon.UCMLWSBPObject";
            //添加字段
            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                //添加dsBCName字段
                CSharpClassField bcTable = new CSharpClassField("DataTable", "ds" + bc.Name);
                bcTable.AccessAuth = AccessAuthority.PRIVATE;
                asmxClass.FieldList.Add(bcTable);
                //添加dsBCNameBase字段
                CSharpClassField bcBase = new CSharpClassField("UCMLCommon.UseEntityClass", "ds" + bc.Name + "Base");
                bcBase.AccessAuth = AccessAuthority.PRIVATE;
                asmxClass.FieldList.Add(bcBase);
                //添加BCNameColumn字段
                CSharpClassField bcColumn = new CSharpClassField("UCMLCommon.BusinessColumn[]", bc.Name + "Column");
                bcColumn.AccessAuth = AccessAuthority.PUBLIC;
                asmxClass.FieldList.Add(bcColumn);
                //添加BCNameCondiColumn字段
                CSharpClassField bcCondiColumn = new CSharpClassField("UCMLCommon.BusinessColumn[]", bc.Name + "CondiColumn");
                bcCondiColumn.AccessAuth = AccessAuthority.PUBLIC;
                asmxClass.FieldList.Add(bcCondiColumn);
            }
            //添加Column字段
            CSharpClassField column = new CSharpClassField("UCMLCommon.BusinessColumn", "column");
            column.AccessAuth = AccessAuthority.PRIVATE;
            asmxClass.FieldList.Add(column);

            //添加 IContainer字段
            CSharpClassField container = new CSharpClassField("IContainer", "components");
            container.AccessAuth = AccessAuthority.PRIVATE;
            container.InitStatment = "null";
            asmxClass.FieldList.Add(container);
            //添加函数
            //无参数构造函数
            asmxClass.DefaultConstruct.Content.AppendLine("OwnerFlow=\"\";");
            asmxClass.DefaultConstruct.Content.AppendLine("OwnerActivity=\"\"");
            asmxClass.DefaultConstruct.Content.AppendLine("InFlow=false;");
            asmxClass.DefaultConstruct.Content.AppendLine("fSystemBPO="+this.BpoPropSet.fSystemBPO+";");
            asmxClass.DefaultConstruct.Content.AppendLine("fRegisterBPO"+this.BpoPropSet.fRegisterBPO+";");
            asmxClass.DefaultConstruct.Content.AppendLine("fEnableConfig"+this.BpoPropSet.fEnableConfig+";");
            asmxClass.DefaultConstruct.Content.AppendLine("fFromActivityPermiss=false;");
            asmxClass.DefaultConstruct.Content.AppendLine("fTransactionType = TransactionType.tsDB;");
            asmxClass.DefaultConstruct.Content.AppendLine("InitializeComponent();");
            
            //添加InitiallizeComponent函数
            CSharpFunction InitializeComponent = new CSharpFunction("InitializeComponent");
            InitializeComponent.AccessAuth = AccessAuthority.PRIVATE;
            InitializeComponent.ReturnType = "void";
            asmxClass.AddFunction(InitializeComponent);

            //添加Dispose函数
            CSharpFunction dispose = new CSharpFunction("Dispose");
            dispose.AccessAuth = AccessAuthority.PROTECTED;
            dispose.ReturnType = "void";
            dispose.IsOverride = true;
            dispose["bool"] = "disposing";
            dispose.Content.AppendLine("if(disposing && components != null)");
      		dispose.Content.AppendLine("{");
      	    dispose.Content.AppendLine("    components.Dispose();");
      		dispose.Content.AppendLine("}");
            dispose.Content.AppendLine("base.Dispose(disposing);");
            asmxClass.AddFunction(dispose);

            //添加RegisterUseTable 函数PreparePropertyList
            CSharpFunction regUseTable = new CSharpFunction("RegisterUseTable");
            regUseTable.AccessAuth = AccessAuthority.PUBLIC;
            regUseTable.IsOverride = true;
            regUseTable.ReturnType = "void";
            //RegisterUseTable函数体
            regUseTable.Content.AppendLine("BPOName = \""+this.Name+"\";");
            regUseTable.Content.AppendLine("UseEntityArray = new UCMLCommon.UseEntityClass["+BCList.Count+"];");
            regUseTable.Content.AppendLine("UCMLCommon.UseEntityClass item;");
            int index = 0;
            foreach (UcmlBusiCompPropSet bc in BCList)
            {
                regUseTable.Content.AppendLine("item = new UCMLCommon.UseEntityClass();");
                regUseTable.Content.AppendLine("item.TableName = \""+bc.TableName+"\";");
                regUseTable.Content.AppendLine("item.BCName = \""+bc.Name+"\";");
                regUseTable.Content.AppendLine("item.dataTable = "+bc.Name+";");
                regUseTable.Content.AppendLine("item.entityType = UCMLCommon.EntityType.MOTIF;");
                regUseTable.Content.AppendLine("item.Columns = "+bc.Name+"Column;");
                regUseTable.Content.AppendLine("item.condiColumns = "+bc.Name+"CondiColumn;");
                regUseTable.Content.AppendLine("item.DBObjectType = typeof(DBLayer."+bc.Name+");");
                regUseTable.Content.AppendLine("item.DaoType = typeof(DBLayer."+bc.Name+");");
                regUseTable.Content.AppendLine("item.FieldInfoArray = "+bc.Name+"Column;");
                regUseTable.Content.AppendLine("item.DBType = UCMLCommon.DBType.MSSQL;");//数据库连接类型
                regUseTable.Content.AppendLine("item.Provider = UCMLCommon.DBProvider.MSSQLNA;");
                regUseTable.Content.AppendLine("item.DataOwnerType = (UCMLCommon.DataOwnerType)(0);");
                regUseTable.Content.AppendLine("item.DataAccessControlFieldName = \"\";");
                regUseTable.Content.AppendLine("item.CatalogFieldName = \"\";");
                regUseTable.Content.AppendLine("item.CatalogFieldValue = \"\";");
                regUseTable.Content.AppendLine("item.LinkKeyName = \"\";");
                regUseTable.Content.AppendLine("item.LinkKeyType = 46;");
                regUseTable.Content.AppendLine("item.PK_COLUMN_NAME = \"\";");
                regUseTable.Content.AppendLine("item.CascadeDeleteMode = 1;");
                regUseTable.Content.AppendLine("item.CascadeTableName = \"\";");
                regUseTable.Content.AppendLine("item.CascadeFKName = \"\";");
                regUseTable.Content.AppendLine("item.SelectLastFix = \"\";");
                regUseTable.Content.AppendLine("item.WhereLastFix = \"\";");
                regUseTable.Content.AppendLine("item.IsSODMode = false;");
                regUseTable.Content.AppendLine("item.fCustomerSQL = false;");
                regUseTable.Content.AppendLine("item.CustomerSQLSelect = \"\";");
                regUseTable.Content.AppendLine("item.AllowModifyJION = false;");
                regUseTable.Content.AppendLine("item.AllowModifyFK = \"\";");
                regUseTable.Content.AppendLine("item.fNumToString =  false;");
                regUseTable.Content.AppendLine("item.PageCount = 10;");
                regUseTable.Content.AppendLine("item.fNotReadData = false;");
                regUseTable.Content.AppendLine("item.IsRootBC = true;");
                regUseTable.Content.AppendLine("item.JoinInfo = new UCMLJoinInfo[0];");
                regUseTable.Content.AppendLine("item.BusiViewModes = new UCMLCommon.UCMLBusiViewMode[0];");
                regUseTable.Content.AppendLine(" if (fLoalClass)");
                regUseTable.Content.AppendLine("    item.BuildDataTableEx();");
                regUseTable.Content.AppendLine("if (item.dataTable.Columns.Count==0)");
                regUseTable.Content.AppendLine("{");
                regUseTable.Content.AppendLine("    for ( int i=0; i<item.Columns.Length;i++)");
                regUseTable.Content.AppendLine("    {");
                regUseTable.Content.AppendLine("        item.dataTable.Columns.Add(new DataColumn(item.Columns[i].FieldName, SystemTypeToCSharp(item.Columns[i].FieldType)));");
                regUseTable.Content.AppendLine("    }");
                regUseTable.Content.AppendLine("}");
                regUseTable.Content.AppendLine("item.fHaveUCMLKey = true;");
                regUseTable.Content.AppendLine("item.fIDENTITYKey = false;");
                regUseTable.Content.AppendLine("item.BaseKeyField = \""+bc.TableName+"OID\";");
                regUseTable.Content.AppendLine(" UseEntityArray["+index+"] = item;");
                index++;
            }
            
            asmxClass.AddFunction(regUseTable);

            //添加PreparePropertyList 函数
            CSharpFunction prePropertyList = new CSharpFunction("PreparePropertyList");
            prePropertyList.AccessAuth = AccessAuthority.PROTECTED;
            prePropertyList.IsOverride = true;
            prePropertyList.ReturnType = "void";
            prePropertyList.Content.AppendLine("if (PropertyList.Columns.Count==0)");
            prePropertyList.Content.AppendLine("{");
            prePropertyList.Content.AppendLine("}");
            asmxClass.AddFunction(prePropertyList);

            //添加ReadPropertyList 函数
            CSharpFunction ReadPropertyList = new CSharpFunction("ReadPropertyList");
            ReadPropertyList.AccessAuth = AccessAuthority.PROTECTED;
            ReadPropertyList.IsOverride = true;
            ReadPropertyList.ReturnType = "void";
            ReadPropertyList.Content.AppendLine("if (PropertyList.Columns.Count==0)");
            ReadPropertyList.Content.AppendLine("{");
            ReadPropertyList.Content.AppendLine("}");
            asmxClass.AddFunction(ReadPropertyList);

            //添加PrepareColumn 函数
            CSharpFunction PrepareColumn = new CSharpFunction("PrepareColumn");
            PrepareColumn.AccessAuth = AccessAuthority.PUBLIC;
            PrepareColumn.IsOverride = true;
            PrepareColumn.ReturnType = "void";
            //函数体
            foreach(UcmlBusiCompPropSet bc in this.BCList)
            {
                PrepareColumn.Content.AppendLine(bc.Name+"Column = LoadColumnFromXML(\""+bc.Name+"\");");
                PrepareColumn.Content.AppendLine("if ("+bc.Name+"Column == null)");
                PrepareColumn.Content.AppendLine("{");
                PrepareColumn.Content.AppendLine("    "+bc.Name+"Column = new UCMLCommon.BusinessColumn["+bc.Columns.Count+"];");
                int i = 0;
                foreach (BusiCompColumn col in bc.Columns)
                {
                    PrepareColumn.Content.AppendLine("column = new UCMLCommon.BusinessColumn();");
                    PrepareColumn.Content.AppendLine("column.FieldName = \""+col.FieldName+"\";");
                    PrepareColumn.Content.AppendLine("column.fDisplay = "+col.fDisplay+"; ");
                    PrepareColumn.Content.AppendLine("column.fCanModify = "+col.fCanModify+";");
                    PrepareColumn.Content.AppendLine("column.Pos = "+col.Pos+";");
                    PrepareColumn.Content.AppendLine("column.Width = "+col.Width+";");
                    PrepareColumn.Content.AppendLine("column.FieldType = "+col.FieldType+";");
                    PrepareColumn.Content.AppendLine("column.StatMode = "+col.StatMode+";");
                    PrepareColumn.Content.AppendLine("column.SortMode = "+col.SortMode+";");
                    PrepareColumn.Content.AppendLine("column.fGroupBy = "+col.fGroupBy+"; ");
                    PrepareColumn.Content.AppendLine("column.Caption = \""+col.Caption+"\";");
                    PrepareColumn.Content.AppendLine("column.EditType = \""+col.EditType+"\";");
                    PrepareColumn.Content.AppendLine("column.CodeTable = \""+col.CodeTable+"\";");
                    PrepareColumn.Content.AppendLine("column.fUseCodeTable = "+col.fUseCodeTable+";");
                    PrepareColumn.Content.AppendLine("column.fAllowNull = "+col.fAllowNull+";");
                    PrepareColumn.Content.AppendLine("column.CurrentPos = "+col.CurrentPos+";");
                    PrepareColumn.Content.AppendLine("column.DefaultValue = \""+col.DefaultValue+"\";");
                    PrepareColumn.Content.AppendLine("column.fAllowPick = "+col.fAllowPick+";");
                    PrepareColumn.Content.AppendLine("column.ForeignKeyField = \""+col.ForeignKeyField+"\";");
                    PrepareColumn.Content.AppendLine("column.LookupKeyField = \""+col.LookupKeyField+"\";");
                    PrepareColumn.Content.AppendLine("column.LookupDataSet = \""+col.LookupDataSet+"\";");
                    PrepareColumn.Content.AppendLine("column.LookupResultField = \""+col.LookupResultField+"\";");
                    PrepareColumn.Content.AppendLine("column.fForeignKey = "+col.fForeignKey+";");
                    PrepareColumn.Content.AppendLine("column.FieldKind = "+col.FieldKind+";");
                    PrepareColumn.Content.AppendLine("column.CustomSQLColumn = \""+col.CustomSQLColumn+"\";");
                    PrepareColumn.Content.AppendLine("column.ExcelColNo = "+col.ExcelColNo+";");
                    PrepareColumn.Content.AppendLine(bc.Name+"Column["+i+"] = column;");
                    PrepareColumn.Content.AppendLine("");
                }
                PrepareColumn.Content.AppendLine();
            }
            asmxClass.AddFunction(PrepareColumn);

            //添加ReadFromKPISection 函数
            CSharpFunction ReadFromKPISection = new CSharpFunction("ReadFromKPISection");
            ReadFromKPISection.AccessAuth = AccessAuthority.PUBLIC;
            ReadFromKPISection.ReturnType = "void";
            asmxClass.AddFunction(ReadFromKPISection);

            //添加WriteToKPISection 函数
            CSharpFunction WriteToKPISection = new CSharpFunction("WriteToKPISection");
            WriteToKPISection.AccessAuth = AccessAuthority.PUBLIC;
            WriteToKPISection.ReturnType = "void";
            asmxClass.AddFunction(WriteToKPISection);

            //添加ReadDataFromFlow 函数
            CSharpFunction ReadDataFromFlow = new CSharpFunction("ReadDataFromFlow");
            ReadDataFromFlow.AccessAuth = AccessAuthority.PUBLIC;
            ReadDataFromFlow.IsOverride = true;
            ReadDataFromFlow.ReturnType = "void";
            asmxClass.AddFunction(ReadDataFromFlow);

            //添加ReadDataFromFlowEx 函数
            CSharpFunction ReadDataFromFlowEx = new CSharpFunction("ReadDataFromFlowEx");
            ReadDataFromFlowEx.AccessAuth = AccessAuthority.PUBLIC;
            ReadDataFromFlowEx.IsOverride = true;
            ReadDataFromFlowEx.ReturnType = "void";
            ReadDataFromFlowEx.Content.AppendLine("Object obj=null;");
            asmxClass.AddFunction(ReadDataFromFlowEx);

            //添加SaveDataToFlow 函数
            CSharpFunction SaveDataToFlow = new CSharpFunction("SaveDataToFlow");
            SaveDataToFlow.AccessAuth = AccessAuthority.PUBLIC;
            SaveDataToFlow.IsOverride = true;
            SaveDataToFlow.ReturnType = "void";
            asmxClass.AddFunction(SaveDataToFlow);

            //添加BusinessInit 函数
            CSharpFunction BusinessInit = new CSharpFunction("BusinessInit");
            BusinessInit.AccessAuth = AccessAuthority.PROTECTED;
            BusinessInit.IsOverride = true;
            BusinessInit.ReturnType = "void";
            BusinessInit.Content.AppendLine("base.BusinessInit();");
            asmxClass.AddFunction(BusinessInit);

            //添加BeforeBusinessSubmit 函数
            CSharpFunction BeforeBusinessSubmit = new CSharpFunction("BeforeBusinessSubmit");
            BeforeBusinessSubmit.AccessAuth = AccessAuthority.PROTECTED;
            BeforeBusinessSubmit.IsOverride = true;
            BeforeBusinessSubmit.ReturnType = "bool";
            BeforeBusinessSubmit.Content.AppendLine("return true;");
            asmxClass.AddFunction(BeforeBusinessSubmit);

            //添加AfterBusinessSubmit 函数
            CSharpFunction AfterBusinessSubmit = new CSharpFunction("AfterBusinessSubmit");
            AfterBusinessSubmit.AccessAuth = AccessAuthority.PROTECTED;
            AfterBusinessSubmit.IsOverride = true;
            AfterBusinessSubmit.ReturnType = "void";
            asmxClass.AddFunction(AfterBusinessSubmit);


            AsmxCs.InnerClass.Add(asmxClass);
            return true;
        }
    }
}
