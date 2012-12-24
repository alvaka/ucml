using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlBPO
    {
        private string _Name;
        public string SavePath;
        public bool CompileMode;
        public AspxPage Page;
        private CSharpDoc _PageCs;
        private CSharpDoc PageDesignerCs;
        public CSharpDoc AsmxCs;
        private BpoPropertySet BpoPropSet;
        public  List<UcmlVcTabPage> VcTabList;
        public List<UcmlBusiCompPropSet> BCList;
        public AsmxDoc AsmxPage;
        public HtcDoc BpoHtc;
        public HtcDoc TableHtc;
        
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
            SavePath = "";

            BpoPropSet = bps;
            Page = new AspxPage(bps.Name + ".aspx",bps.Capiton);
            PageCs = new CSharpDoc(bps.Name+".aspx.cs", Namespace);
            PageDesignerCs = new CSharpDoc(bps.Name + ".aspx.designer.cs", Namespace);
            AsmxCs = new CSharpDoc(this.Name+".asmx.cs", Namespace);
            BpoHtc=new HtcDoc(this.Name+".htc");

            this.AsmxPage = new AsmxDoc(this.Name + ".asmx");

            VcTabList = new List<UcmlVcTabPage>();
            BCList = new List<UcmlBusiCompPropSet>();
        }

        /// <summary>
        /// 构建Aspx页面
        /// </summary>
        /// <returns></returns>
        public bool BuildAspxPage()
        {
            //初始化页面，构造head,body,form等基本节点
            Page.InitPage();
            Page.Head["runat"] = "server";
            //添加页面指令
            AspxDirective direc4Page = new AspxDirective("Page");
            direc4Page["language"] = "C#";
            if (this.CompileMode) direc4Page["codeBehind"] = Page.PageName + ".cs";
            else direc4Page["codeFile"] = Page.PageName + ".cs";
            
            direc4Page["Inherits"]="UCMLCommon."+this.Name;
            direc4Page["AutoEventWireup"]="False";
            direc4Page["ResponseEncoding"] = "UTF-8";

            AspxDirective direc4Reg = new AspxDirective("Register");
            direc4Reg["TagPrefix"]= "iewc";
            direc4Reg["Namespace"]="Microsoft.Web.UI.WebControls";
            direc4Reg["Assembly"]="Microsoft.Web.UI.WebControls";

            Page.Directives.Add(direc4Page);
            Page.Directives.Add(direc4Reg);

            //添加META标签
            HtmlNode metaCompatible1 = HtmlNode.CreateClosedNode("meta");
            metaCompatible1["http-equiv"] = "X-UA-Compatible";
            metaCompatible1["content"] = "IE=EmulateIE7";

            HtmlNode metaCompatible2 = HtmlNode.CreateClosedNode("meta");
            metaCompatible2["http-equiv"] = "X-UA-Compatible";
            metaCompatible2["content"] = "IE=7";
            this.Page.Head.Append(metaCompatible1);
            this.Page.Head.Append(metaCompatible2);
            
            //添加JS source 标签
            this.Page.Head.Append(JsContext.GetJsLinkTag("Model/rule/JSRule.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("Model/rule/initvalue.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("js/UCML_PublicApp.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("js/ig_shared.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("js/ig_edit.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("js/dnncore.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("js/dnn.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("js/dnn.dom.positioning.js"));
            this.Page.Head.Append(JsContext.GetJsLinkTag("js/ucmlapp.js"));

            //添加js 定义 标签
            JsContext js4Head = new JsContext();
            js4Head.Content.AppendLine("var UCMLResourcePath=\"\";");
            js4Head.Content.AppendLine("var UCMLLocalResourcePath=\"\";");
            js4Head.Content.AppendLine("var BPOName=\""+this.Name+"\";");
            JsFunction init = new JsFunction("Init");
            init.Content.AppendLine("var dobject=window.document.all[\"UCMLBUSIOBJECT\"];");
            init.Content.AppendLine("if (dobject==undefined) return;");
            init.Content.AppendLine(this.Name+"BPO.open();");

            string masterTable = "";
            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                string bcBase = bc.Name + "Base";
                if (bc.IsRootBC)
                {
                    masterTable = bcBase;

                    init.Content.AppendLine(bcBase + ".fIDENTITYKey =" + bc.fIDENTITYKey.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".AllowModifyJION = " + bc.AllowModifyJION.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".fHaveUCMLKey = " + bc.fHaveUCMLKey.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".PrimaryKey = \""+bc.PrimaryKey+"\";");
                    init.Content.AppendLine(bcBase + ".Columns = "+this.Name+"BPO."+bc.Name+"Columns;");
                    init.Content.AppendLine(bcBase + ".ChangeOnlyOwnerBy = " + bc.ChangeOnlyOwnerBy.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".BPOName = \"" + this.Name + "\";");
                }
                else
                {
                    init.Content.AppendLine(bcBase + ".fIDENTITYKey = " + bc.fIDENTITYKey.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".AllowModifyJION = " + bc.AllowModifyJION.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".fHaveUCMLKey = " + bc.fHaveUCMLKey.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".PrimaryKey = \""+bc.PrimaryKey+"\";");
                    init.Content.AppendLine(bcBase + ".Columns = " + this.Name + "BPO." + bc.Name + "Columns;");
                    init.Content.AppendLine(bcBase + ".ChangeOnlyOwnerBy = " + bc.ChangeOnlyOwnerBy.ToString().ToLower() + ";");
                    init.Content.AppendLine(bcBase + ".TableType = \"S\";");
                    init.Content.AppendLine(bcBase + ".LinkKeyName = \""+bc.LinkKeyName+"\";");
                    init.Content.AppendLine(bcBase + ".PK_COLUMN_NAME = \""+bc.PK_COLUMN_NAME+"\";");
                    init.Content.AppendLine(bcBase + ".MasterTable = "+masterTable+";");
                    init.Content.AppendLine(bcBase + ".BPOName = \""+this.Name+"\";");
                }
            }

            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    init.Content.AppendLine(vc.VCName + ".UserDefineHTML=\"" + vc.UserDefineHTML.ToString().ToLower() + "\";");
                    init.Content.AppendLine(vc.BCName+"Base.AddConnectControls("+vc.VCName+");");
                    
                    init.Content.AppendLine(vc.VCName+".BPOName=\""+this.Name+"\";");
                    init.Content.AppendLine(vc.VCName+".AppletName=\""+vc.VCName+"\";");
                    init.Content.AppendLine(vc.VCName + ".EnabledEdit=" + vc.EnabledEdit.ToString().ToLower() + ";");
                    init.Content.AppendLine(vc.VCName + ".haveMenu=" + vc.haveMenu.ToString().ToLower() + ";");
                    init.Content.AppendLine(vc.VCName+".parentNodeID=\"\";");
                    init.Content.AppendLine(vc.VCName+".HiddenID=\"TabStrip_"+vc.VCName+";MultiPage_"+vc.VCName+"\";");
                    init.Content.AppendLine(vc.VCName + ".fHidden=\"" + vc.fHidden.ToString().ToLower() + "\";");
                    init.Content.AppendLine(vc.VCName + ".alignHeight=\"" + vc.alignHeight.ToString().ToLower() + "\";");
                    init.Content.AppendLine(vc.VCName + ".alignWidth=\"" + vc.alignWidth.ToString().ToLower() + "\";");

                    init.Content.AppendLine(vc.VCName+".open();");
                }
            }

            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                init.Content.AppendLine(bc.Name + "Base.EnabledEdit=true;");
                init.Content.AppendLine(bc.Name + "Base.EnabledAppend=true;");
                init.Content.AppendLine(bc.Name + "Base.EnabledDelete=true;");
                init.Content.AppendLine(bc.Name + "Base.RecordOwnerType=0;");
                init.Content.AppendLine(bc.Name + "Base.open();");
                init.Content.AppendLine(this.Name+"BPO.AddUseTable("+bc.Name+"Base);");
                init.Content.AppendLine();
            }

            init.Content.AppendLine(this.Name+"BPO.InitBusinessEnv();");
            init.Content.AppendLine();
            //把Init函数加入到JS定义 中
            js4Head.JsFuncs.Add(init);
            this.Page.Head.Append(js4Head.GetJsBlockNode());

            #region 构建主页面
            //构建主页面
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                //TabStrip控件
                AspxNode tabStrip = new AspxNode("iewc:TabStrip");
                tabStrip["id"] = "TabStrip_" + vcTab.Name;
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
                multiPage["id"] = "MultiPage_" + vcTab.Name;
                multiPage["width"] = "100%";//待改成变量
                
                //把TabStrip控件和MultiPage控件挂到MainPanelNode下
                Page.MainPanelNode.Append(tabStrip);
                Page.MainPanelNode.Append(multiPage);

                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    //添加Tab控件
                    AspxNode tab = new AspxNode("iewc:Tab");
                    tab["id"] = "Tab_" + vc.VCName;
                    tab["Text"] = vc.Caption;

                    //添加TabSeperator控件
                    HtmlNode tabSep = HtmlNode.CreateClosedNode("iewc:TabSeparator");
                    //挂到tabStrip下
                    tabStrip.Append(tab);
                    tabStrip.Append(tabSep);

                    //添加PageView控件
                    AspxNode pageView = new AspxNode("iewc:PageView");
                    pageView["id"] = "PageView_" + vc.VCName;
                    pageView["Text"] = vc.Caption;

                    //添加Panel控件
                    AspxNode panel = new AspxNode("asp:Panel");
                    panel["id"] = vc.VCName + "_Module";
                    panel["style"] = "overflow:hidden";
                    panel["CssClass"] = "UCML-Panel";
                    panel["width"] = "100%";//待改成变量
                    //挂载PageView到MultiPage
                    multiPage.Append(pageView);
                    //挂载Panel到PageView下
                    pageView.Append(panel);

                    //添加ToolBar
                    if (vc.Buttons.Count != 0)
                    {
                        AspxNode toolBar = new AspxNode("iewc:Toolbar");
                        toolBar["id"] = "ToolBar" + vc.VCName;
                        toolBar["style"] = "Z-INDEX: 102; LEFT: 0px; TOP: 0px";
                        toolBar["Height"] = "23px";
                        toolBar["Width"] = "100%";
                        panel.Append(toolBar);

                        //添加ToolButton
                        foreach (UcmlVcButton button in vc.Buttons)
                        {
                            if (button.Type == 0)
                            {
                                HtmlNode btn = new HtmlNode("iewc:ToolbarButton");
                                btn["Text"] = button.Caption;
                                btn["ToolTip"] = button.ToolTip;
                                btn["ImageUrl"] = button.ImgeLink;
                                toolBar.Append(btn);
                            }
                            else if (button.Type == 4)
                            {
                                toolBar.Append(new HtmlNode("iewc:ToolbarSeparator"));
                            }
                        }
                    }

                    HtmlNode pageNode = null;
                    if (vc.VCNode == null) pageNode = new HtmlNode("div");
                    else 
                    {
                        if (vc.VCNode.Childs.Count == 1 && vc.VCNode.Childs[0].OnlyText) vc.VCNode.Childs.Clear();
                        pageNode = vc.VCNode;
                    }
                    //构造ContextMenu
                    HtmlNode span = new HtmlNode("span");
                    span["id"] = "theContextMenu" + vc.VCName;
                    span["style"] = "Z-INDEX:3103;LEFT:0px;VISIBILITY:hidden;BEHAVIOR: url(menubar.htc);WIDTH:200px;POSITION:absolute;TOP: 0px;HEIGHT:20px";
                    if (vc.Kind == 163)
                    {
                        panel.Append(pageNode);
                        //添加VCName Div
                        HtmlNode div = new HtmlNode("div");
                        div["id"] = vc.VCName;
                        div["style"] = "BEHAVIOR:url(UCMLDBGrid.htc);width:100%;height:200";
                        div["title"] = vc.Caption;
                        panel.Append(div);

                        div.Append(span);
                    }
                    else if (vc.Kind == 164||vc.Kind==165)
                    {
                        HtmlNode div = new HtmlNode("div");
                        div["id"] = vc.VCName;
                        div["style"] = "BEHAVIOR:url(UCMLEdit.htc);Width:100%;Height:200";
                        div["title"] = vc.Caption;
                        panel.Append(div);

                        div.Append(pageNode);
                        div.Append(span);
                    }
            #endregion 构建主页面
                }
            }

            JsContext tailJS = new JsContext();
            tailJS.Content.AppendLine("document.write('<DIV style=\"BORDER-RIGHT: #0066ff 1px solid; BORDER-TOP: #0066ff 1px solid; Z-INDEX: 103; LEFT: 201px; VISIBILITY: hidden; BORDER-LEFT: #0066ff 1px solid; WIDTH: 272px; BORDER-BOTTOM: #0066ff 1px solid; POSITION: absolute; TOP: 229px; HEIGHT: 27px; BACKGROUND-COLOR: #ffffcc; TEXT-ALIGN: center\" ms_positioning=\"FlowLayout\" id=\"MsgPanel\"></DIV>');");
            JsFunction contextMenu = new JsFunction("document.oncontextmenu");
            contextMenu.Content.AppendLine("event.returnValue = false; ");
            contextMenu.Content.AppendLine("event.cancelBubble = true; ");
            contextMenu.Content.AppendLine("return false; ");
            tailJS.JsFuncs.Add(contextMenu);

            this.Page.Body.Append(tailJS.GetJsBlockNode());
            return true; 
        }

        public bool BuildAspxPageCs()
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
            PageCs.ReferenceNS.Add("System.Collections.Specialized");

            //添加自定义Namespace引用 
            if (!String.IsNullOrWhiteSpace(this.BpoPropSet.RefCSharpLibrary))
            {
                string[] lines = Util.SplitLine(this.BpoPropSet.RefCSharpLibrary);
                foreach (string line in lines)
                {
                    PageCs.ReferenceNS.Add(line);
                }
            }

            PageCs.ReferenceNS.Add("DBLayer");

            //类定义
            CSharpClass bpoClass = new CSharpClass(this.Name);
            bpoClass.BaseClass = "UCMLCommon.UCMLBPObject";
            bpoClass.AccessAuth = AccessAuthority.PUBLIC;
            bpoClass.IsPartial = true;
            //添加字段
            //BPONameService字段
            CSharpClassField field = new CSharpClassField("UCMLWSBPObject", "busiObj");
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
            pageCaption.GetStatment.AppendLine("else");
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
            pageLoad.Content.AppendLine("    RePositionApplet();");
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
            initCtr.Content.AppendLine("InFlow=" + BpoPropSet.fInFlow.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("fSystemBPO=" + BpoPropSet.fSystemBPO.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("fHavePageNavi=" + BpoPropSet.fHavePageNavi.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("fRegisterBPO=" + BpoPropSet.fRegisterBPO.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("fMutiLangugeSupport=" + BpoPropSet.fMutiLangugeSupport.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("fXHTMLForm=" + BpoPropSet.fXHTMLForm.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("fEnableConfig=" + BpoPropSet.fEnableConfig.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("fUseSkin=" + BpoPropSet.fUseSkin.ToString().ToLower() + ";");
            initCtr.Content.AppendLine("RootPath=@\"\";");
            initCtr.Content.AppendLine("LocalResourcePath=\"\";");
            initCtr.Content.AppendLine("SkinSrc=\""+BpoPropSet.SkinSrc+"\";");
            initCtr.Content.AppendLine("if (fRegisterBPO == true && UCMLCommon.UCMLInitEnv.fInit==false)");
            initCtr.Content.AppendLine("{");
            initCtr.Content.AppendLine("    new DBLayer.LogicDBModel();");
            initCtr.Content.AppendLine("    UCMLCommon.UCMLLogicDBModelApp x = new UCMLCommon.UCMLLogicDBModelApp();");
            initCtr.Content.AppendLine("    x.PrepareModel();");
            initCtr.Content.AppendLine("    UCMLCommon.UCMLInitEnv.Server = this.Server;");
            initCtr.Content.AppendLine("    UCMLCommon.UCMLInitEnv.LoadEnvVariable();");
            initCtr.Content.AppendLine("}");
            initCtr.Content.AppendLine("BPOName=\""+this.Name+"\";");
            initCtr.Content.AppendLine("BPOCaption=\""+BpoPropSet.Capiton+"\";");
            initCtr.Content.AppendLine("base.InitControls();");
            initCtr.Content.AppendLine("if (fHavePageNavi==true || this.Request.QueryString[\"NaviPageBar\"]==\"1\")");
            initCtr.Content.AppendLine("{");
            initCtr.Content.AppendLine("    this.NaviPageBar.Visible=true;");
            initCtr.Content.AppendLine("    UCMLCommon.PageNaviControl ctrl = (UCMLCommon.PageNaviControl)this.LoadControl(@\"..\\OtherSource\\PageNaviControl.ascx\");");
            initCtr.Content.AppendLine("    ctrl.BPOName = BPOCaption;");
            initCtr.Content.AppendLine("    this.NaviPageBar.Controls.Add(ctrl);");
            initCtr.Content.AppendLine("}");
            initCtr.Content.AppendLine("PrepareColumn();");
            initCtr.Content.AppendLine("RegisterUseTable();");
            //添加BPOnameBPO标签
            string BponameBPO=this.Name+"BPO";
            initCtr.Content.AppendLine(BponameBPO+"= new System.Web.UI.WebControls.Panel();");
            initCtr.Content.AppendLine(BponameBPO+".ID = \""+BponameBPO+"\";");
            initCtr.Content.AppendLine(BponameBPO+".Style.Add(\"BEHAVIOR\",\"url("+this.Name+".htc)\");");
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
            initCtr.Content.AppendLine(BponameService+".ID =\""+BponameService+"\";");
            initCtr.Content.AppendLine(BponameService+".Style.Add(\"BEHAVIOR\",\"url(\"+LocalResourcePath+\"htc/webservice.htc)\");");
            initCtr.Content.AppendLine(BponameService+".Attributes.Add(\"onresult\",\""+BponameBPO+".LoadResults()\");");
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
                initCtr.Content.AppendLine(bcNameBase + ".Attributes.Add(\"TableName\",\"" + bc.TableName + "\");");
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
                    initCtr.Content.AppendLine("appletLink.fTreeGridMode = " + vc.fTreeGridMode.ToString().ToLower() + ";");
                    initCtr.Content.AppendLine("appletLink.fSubTableTreeMode = " + vc.fSubTableTreeMode.ToString().ToLower() + ";");
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
            if (this.CompileMode) initCtr.Content.AppendLine("busiObj =new " + this.Name + "Service(true);");
            else initCtr.Content.AppendLine("busiObj = UCMLBusinessObjectFactory.CreateBusinessObject(this.BPOName);");

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
                    initCtr.Content.AppendLine("InputHidden.ID = \"" + vc.VCName + "_ColumnSetup\";");
                    initCtr.Content.AppendLine("this.Controls.Add(InputHidden);");
                }
            }
            initCtr.Content.AppendLine("ReplaceResourceData();");

            initCtr.Content.AppendLine("this.Response.Write(\"<script language=javascript>var UCMLSkinPath='\"+this.SkinPath+\"'</script>\");");
            initCtr.Content.AppendLine("");
            //
            initCtr.Content.AppendLine("LiteralControl li1=new LiteralControl();");
            initCtr.Content.AppendLine("li1.Text=\"<script language=javascript>var UCMLSkinPath='\"+this.SkinPath+\"'</script>\";");
            initCtr.Content.AppendLine("Page.Header.Controls.AddAt(0,li1);");
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

            //添加RePositionApplet函数
            CSharpFunction rpApplet = new CSharpFunction("RePositionApplet");
            rpApplet.AccessAuth = AccessAuthority.PRIVATE;
            rpApplet.ReturnType = "void";

            //添加自定义代码
            foreach(UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    iocPost.Content.AppendLine(vc.HttpPostCSharpCode);
                    iocGet.Content.AppendLine(vc.HttpGetCSharpCode);
                }
            }
            bpoClass.AddFunction(iocPost);
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

        public bool BuildAspxPageDesignCs()
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
            this.PageDesignerCs.InnerClass.Add(bpoClass);
            return true; 
        }

        public bool BuildAsmxCs()
        {
            AsmxCs.ReferenceNS.Add("System.Collections");
            AsmxCs.ReferenceNS.Add("System.Data");
            AsmxCs.ReferenceNS.Add("System.Xml");
            AsmxCs.ReferenceNS.Add("System.ComponentModel");
            AsmxCs.ReferenceNS.Add("System.Web.Services");
            AsmxCs.ReferenceNS.Add("System.Web");
            AsmxCs.ReferenceNS.Add("System.Diagnostics");

            //添加自定义Namespace引用 
            if (!String.IsNullOrWhiteSpace(this.BpoPropSet.RefCSharpLibrary))
            {
                string[] lines = Util.SplitLine(this.BpoPropSet.RefCSharpLibrary);
                foreach (string line in lines)
                {
                    PageCs.ReferenceNS.Add(line);
                }
            }

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

            //添加属性
            foreach(UcmlBusiCompPropSet bc in this.BCList)
            {
                CSharpClassAttribute bcName = new CSharpClassAttribute(bc.Name);
                bcName.AccessAuth = AccessAuthority.PUBLIC;
                bcName.Type = "DataTable";
                bcName.GetStatment.AppendLine("ds"+bc.Name+" = this.BPODataSet.Tables[\""+bc.Name+"\"];");
                bcName.GetStatment.AppendLine("if (ds"+bc.Name+" == null)");
                bcName.GetStatment.AppendLine("{");
                bcName.GetStatment.AppendLine("   ds"+bc.Name+" = new DataTable(\""+bc.Name+"\");");
                bcName.GetStatment.AppendLine("   this.BPODataSet.Tables.Add(ds"+bc.Name+");");
                bcName.GetStatment.AppendLine("}");
                bcName.GetStatment.AppendLine("return ds"+bc.Name+";");

                bcName.SetStatment.AppendLine("ds"+bc.Name+" = value;");

                asmxClass.AttributeList.Add(bcName);

                CSharpClassAttribute bcNameBase = new CSharpClassAttribute(bc.Name+"Base");
                bcNameBase.Type = "UCMLCommon.UseEntityClass";
                bcNameBase.AccessAuth = AccessAuthority.PUBLIC;
                bcNameBase.GetStatment.AppendLine("return ds"+bc.Name+"Base;");
                bcNameBase.SetStatment.AppendLine("ds" + bc.Name + "Base=value;");
                asmxClass.AttributeList.Add(bcNameBase);
            }

            //添加函数
            //无参数构造函数
            asmxClass.AddDefaultConstruct();
            asmxClass.DefaultConstruct.Content.AppendLine("OwnerFlow=\"\";");
            asmxClass.DefaultConstruct.Content.AppendLine("OwnerActivity=\"\";");
            asmxClass.DefaultConstruct.Content.AppendLine("InFlow="+this.BpoPropSet.fInFlow.ToString().ToLower()+";");
            asmxClass.DefaultConstruct.Content.AppendLine("fSystemBPO=" + this.BpoPropSet.fSystemBPO.ToString().ToLower() + ";");
            asmxClass.DefaultConstruct.Content.AppendLine("fRegisterBPO=" + this.BpoPropSet.fRegisterBPO.ToString().ToLower() + ";");
            asmxClass.DefaultConstruct.Content.AppendLine("fEnableConfig=" + this.BpoPropSet.fEnableConfig.ToString().ToLower() + ";");
            asmxClass.DefaultConstruct.Content.AppendLine("fFromActivityPermiss=false;");
            asmxClass.DefaultConstruct.Content.AppendLine("fTransactionType = TransactionType.tsDB;");
            asmxClass.DefaultConstruct.Content.AppendLine("InitializeComponent();");
            
            //有参数构造函数
            CSharpFunction constructor = CSharpFunction.GetConstruction(asmxClass.Name);
            constructor.Parameters["bool"] = "fLocalClass";
            constructor.BaseParameters.Add("fLocalClass");
            constructor.Content.AppendLine("OwnerFlow=\"\";");
            constructor.Content.AppendLine("OwnerActivity=\"\";");
            constructor.Content.AppendLine("InFlow=" + this.BpoPropSet.fInFlow.ToString().ToLower() + ";");
            constructor.Content.AppendLine("fSystemBPO=" + this.BpoPropSet.fSystemBPO.ToString().ToLower() + ";");
            constructor.Content.AppendLine("fRegisterBPO=" + this.BpoPropSet.fRegisterBPO.ToString().ToLower() + ";");
            constructor.Content.AppendLine("fEnableConfig=" + this.BpoPropSet.fEnableConfig.ToString().ToLower() + ";");
            constructor.Content.AppendLine("fFromActivityPermiss=false;");
            constructor.Content.AppendLine("fTransactionType = TransactionType.tsDB;");
            constructor.Content.AppendLine("InitializeComponent();");

            asmxClass.FunctionList.Add(constructor);
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
                if (CompileMode)
                {
                    regUseTable.Content.AppendLine("item.DBObjectType = typeof(DBLayer." + bc.TableName + ");");
                    regUseTable.Content.AppendLine("item.DaoType = typeof(DBLayer." + bc.TableName + ");");
                }
                else
                {
                    regUseTable.Content.AppendLine("item.DBObjectType = typeof(DBLayer.CommonSQLDAO);");
                    regUseTable.Content.AppendLine("item.DaoType = typeof(DBLayer.CommonSQLDAO);");
                }
                regUseTable.Content.AppendLine("item.FieldInfoArray = "+bc.Name+"Column;");
                regUseTable.Content.AppendLine("item.DBType = UCMLCommon.DBType.MSSQL;");//数据库连接类型
                regUseTable.Content.AppendLine("item.Provider = UCMLCommon.DBProvider.MSSQLNA;");
                regUseTable.Content.AppendLine("item.DataOwnerType = (UCMLCommon.DataOwnerType)(0);");
                regUseTable.Content.AppendLine("item.DataAccessControlFieldName = \"\";");
                regUseTable.Content.AppendLine("item.CatalogFieldName = \"\";");
                regUseTable.Content.AppendLine("item.CatalogFieldValue = \"\";");
                regUseTable.Content.AppendLine("item.LinkKeyName = \""+bc.LinkKeyName+"\";");
                regUseTable.Content.AppendLine("item.LinkKeyType = "+bc.LinkKeyType+";");
                regUseTable.Content.AppendLine("item.PK_COLUMN_NAME = \""+bc.PK_COLUMN_NAME+"\";");
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
                regUseTable.Content.AppendLine("item.PageCount = "+bc.PageCount+";");
                regUseTable.Content.AppendLine("item.fNotReadData = false;");
                regUseTable.Content.AppendLine("item.IsRootBC = "+bc.IsRootBC.ToString().ToLower()+";");
                regUseTable.Content.AppendLine("item.JoinInfo = new UCMLJoinInfo[0];");
                regUseTable.Content.AppendLine("item.BusiViewModes = new UCMLCommon.UCMLBusiViewMode[0];");
                if (bc.ChildBC.Count != 0)
                {
                    regUseTable.Content.AppendLine("item.childTables = new DataTable["+bc.ChildBC.Count+"];");
                    for (int i = 0; i < bc.ChildBC.Count;i++ )
                    {
                        regUseTable.Content.AppendLine("item.childTables["+i+"] ="+bc.ChildBC[i].Name+";");
                    }
                }
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
                regUseTable.Content.AppendLine("UseEntityArray["+index+"] = item;");
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
                    PrepareColumn.Content.AppendLine("   column = new UCMLCommon.BusinessColumn();");
                    PrepareColumn.Content.AppendLine("   column.FieldName = \""+col.FieldName+"\";");
                    PrepareColumn.Content.AppendLine("   column.fDisplay = "+col.fDisplay.ToString().ToLower()+"; ");
                    PrepareColumn.Content.AppendLine("   column.fCanModify = " + col.fCanModify.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("   column.Pos = "+col.Pos+";");
                    PrepareColumn.Content.AppendLine("   column.Width = "+col.Width+";");
                    PrepareColumn.Content.AppendLine("   column.FieldType = "+col.FieldType+";");
                    PrepareColumn.Content.AppendLine("   column.StatMode = "+col.StatMode+";");
                    PrepareColumn.Content.AppendLine("   column.SortMode = "+col.SortMode+";");
                    PrepareColumn.Content.AppendLine("   column.fGroupBy = " + col.fGroupBy.ToString().ToLower() + "; ");
                    PrepareColumn.Content.AppendLine("   column.Caption = \""+col.Caption+"\";");
                    PrepareColumn.Content.AppendLine("   column.EditType = \""+col.EditType+"\";");
                    PrepareColumn.Content.AppendLine("   column.CodeTable = \""+col.CodeTable+"\";");
                    PrepareColumn.Content.AppendLine("   column.fUseCodeTable = " + col.fUseCodeTable.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("   column.fAllowNull = " + col.fAllowNull.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("   column.CurrentPos = "+col.CurrentPos+";");
                    PrepareColumn.Content.AppendLine("   column.DefaultValue = \""+col.DefaultValue+"\";");
                    PrepareColumn.Content.AppendLine("   column.fAllowPick = " + col.fAllowPick.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("   column.ForeignKeyField = \""+col.ForeignKeyField+"\";");
                    PrepareColumn.Content.AppendLine("   column.LookupKeyField = \""+col.LookupKeyField+"\";");
                    PrepareColumn.Content.AppendLine("   column.LookupDataSet = \""+col.LookupDataSet+"\";");
                    PrepareColumn.Content.AppendLine("   column.LookupResultField = \""+col.LookupResultField+"\";");
                    PrepareColumn.Content.AppendLine("   column.fForeignKey = " + col.fForeignKey.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("   column.FieldKind = "+col.FieldKind+";");
                    PrepareColumn.Content.AppendLine("   column.CustomSQLColumn = \""+col.CustomSQLColumn+"\";");
                    PrepareColumn.Content.AppendLine("   column.ExcelColNo = "+col.ExcelColNo+";");
                    PrepareColumn.Content.AppendLine("   "+bc.Name+"Column["+i+"] = column;");
                    PrepareColumn.Content.AppendLine("");
                    i++;
                }
                PrepareColumn.Content.AppendLine("}");
                PrepareColumn.Content.AppendLine(bc.Name+"CondiColumn = new UCMLCommon.BusinessColumn["+bc.CondiColumns.Count+"];");
                for (int j = 0; j < bc.CondiColumns.Count; j++)
                {
                    PrepareColumn.Content.AppendLine("column = new UCMLCommon.BusinessColumn();");
                    PrepareColumn.Content.AppendLine("column.FieldName = \""+bc.CondiColumns[j].FieldName+"\";");
                    PrepareColumn.Content.AppendLine("column.LeftBracket = \""+bc.CondiColumns[j].LeftBracket+"\";");
                    PrepareColumn.Content.AppendLine("column.RightBracket = \""+bc.CondiColumns[j].RightBracket+"\";");
                    PrepareColumn.Content.AppendLine("column.FieldType = "+bc.CondiColumns[j].FieldType+";");
                    PrepareColumn.Content.AppendLine("column.OperationIndent = \""+bc.CondiColumns[j].OperationIndent+"\";");
                    PrepareColumn.Content.AppendLine("column.LogicConnect = \""+bc.CondiColumns[j].LogicConnect+"\";");
                    PrepareColumn.Content.AppendLine("column.CondiFieldValue = \""+bc.CondiColumns[j].CondiFieldValue+"\";");
                    PrepareColumn.Content.AppendLine("column.fCondiField = "+bc.CondiColumns[j].fCondiField.ToString().ToLower()+";");
                    PrepareColumn.Content.AppendLine("column.fIsFunctionValue = "+bc.CondiColumns[j].fIsFunctionValue.ToString().ToLower()+";");
                    PrepareColumn.Content.AppendLine("column.fFreeWhere = "+bc.CondiColumns[j].fFreeWhere.ToString().ToLower()+"; ");
                    PrepareColumn.Content.AppendLine("column.Pos = "+bc.CondiColumns[j].Pos+";");
                    if (bc.CondiColumns[j].fIsFunctionValue)
                    {
                        PrepareColumn.Content.AppendLine("column.getValueHandler += new System.EventHandler(this.get" + bc.Name + bc.CondiColumns[j].FieldName + j + "ValueEx);");
                    }
                    else if (bc.CondiColumns[j].fFreeWhere)
                    {
                        PrepareColumn.Content.AppendLine("column.getWhereItemHandler += new System.EventHandler(this.get"+bc.Name+j+"WhereItemEx);");
                    }
                    PrepareColumn.Content.AppendLine(bc.Name+"CondiColumn["+j+"] = column;");
                    PrepareColumn.Content.AppendLine("");
                }
                
                PrepareColumn.Content.AppendLine("");
                PrepareColumn.Content.AppendLine("");

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
            

            //添加BeforeBusinessSubmit 函数
            CSharpFunction BeforeBusinessSubmit = new CSharpFunction("BeforeBusinessSubmit");
            BeforeBusinessSubmit.AccessAuth = AccessAuthority.PROTECTED;
            BeforeBusinessSubmit.IsOverride = true;
            BeforeBusinessSubmit.ReturnType = "bool";

            //添加AfterBusinessSubmit 函数
            CSharpFunction AfterBusinessSubmit = new CSharpFunction("AfterBusinessSubmit");
            AfterBusinessSubmit.AccessAuth = AccessAuthority.PROTECTED;
            AfterBusinessSubmit.IsOverride = true;
            AfterBusinessSubmit.ReturnType = "void";

            //添加BC级别的代码
            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                BusinessInit.Content.AppendLine(bc.InitCSharpCode);
                BeforeBusinessSubmit.Content.AppendLine(bc.BeforeSubmitCSharpCode);
                AfterBusinessSubmit.Content.AppendLine(bc.AfterSubmitCSharpCode);
            }

            //添加BPO级别的代码
            BusinessInit.Content.AppendLine(this.BpoPropSet.InitCSharpCode);

            BeforeBusinessSubmit.Content.AppendLine(this.BpoPropSet.BeforeSubmitCSharpCode);
            BeforeBusinessSubmit.Content.AppendLine("return true;");

            AfterBusinessSubmit.Content.AppendLine(this.BpoPropSet.AfterSubmitCSharpCode);

            asmxClass.AddFunction(BeforeBusinessSubmit);
            asmxClass.AddFunction(BusinessInit);
            asmxClass.AddFunction(AfterBusinessSubmit);

            
            //添加自定义函数
            foreach (CSharpFunction func in BpoPropSet.CSharpFuncs)
            {
                asmxClass.AddFunction(func);
            }
            //
            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                for (int i = 0; i < bc.CondiColumns.Count;i++ )
                {
                    if (bc.CondiColumns[i].fIsFunctionValue)
                    {
                        CSharpFunction getValueEx = new CSharpFunction("get" + bc.Name + bc.CondiColumns[i].FieldName+i+"ValueEx");
                        getValueEx.AccessAuth = AccessAuthority.PROTECTED;
                        getValueEx.ReturnType = "void";
                        getValueEx.Parameters.Add("object", "sender");
                        getValueEx.Parameters.Add("EventArgs","e");
                        getValueEx.Content.AppendLine("((UCMLCommon.FieldValueEventArgs)e).Value=(get"+ bc.Name + bc.CondiColumns[i].FieldName+i+"Value());");
                        asmxClass.AddFunction(getValueEx);

                        CSharpFunction getValue = new CSharpFunction("get" + bc.Name + bc.CondiColumns[i].FieldName + i + "Value");
                        getValue.AccessAuth = AccessAuthority.PROTECTED;
                        getValue.ReturnType = "string";
                        getValue.Content.AppendLine(bc.CondiColumns[i].valueFunction);
                        asmxClass.AddFunction(getValue);
                    }
                    else if (bc.CondiColumns[i].fFreeWhere)
                    {
                        CSharpFunction getWhereItemEx = new CSharpFunction("get" + bc.Name + i + "WhereItemEx");
                        getWhereItemEx.AccessAuth = AccessAuthority.PROTECTED;
                        getWhereItemEx.ReturnType = "void";
                        getWhereItemEx.Parameters.Add("object", "sender");
                        getWhereItemEx.Parameters.Add("EventArgs", "e");
                        getWhereItemEx.Content.AppendLine("((UCMLCommon.WhereItemEventArgs)e).WhereItem=(get" + bc.Name + i + "WhereItem());");
                        asmxClass.AddFunction(getWhereItemEx);

                        CSharpFunction getWhereItem = new CSharpFunction("get" + bc.Name + i + "WhereItem");
                        getWhereItem.AccessAuth = AccessAuthority.PROTECTED;
                        getWhereItem.ReturnType = "string";
                        getWhereItem.Content.AppendLine(bc.CondiColumns[i].SQL);
                        asmxClass.AddFunction(getWhereItem);
                    }
                }
            }
            AsmxCs.InnerClass.Add(asmxClass);
            return true;
        }

        /// <summary>
        /// 装配BPOName.htc脚本文件
        /// </summary>
        /// <returns></returns>
        public bool BuildBpoHtc()
        {
            //标准方法定义
            this.BpoHtc.Definition.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            this.BpoHtc.Definition.AppendLine("<public:component>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"open\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"LoadResults\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"AddUseTable\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"InitBusinessEnv\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"getRootTable\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"getTaskList\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"getServiceHandle\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"ChangeBusiView\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"condiQuery\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"condiQueryEx\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"condiActorQuery\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"ExportToExcel\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"Report_Compute\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"BusinessSubmit\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"FinishTask\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"StartBCLink\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"CallbuildTreeSubNodes\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"getBCList\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"HideSelect\"/>");
            this.BpoHtc.Definition.AppendLine("<public:method name=\"ShowSelect\"/>");
            foreach(JsFunction func in BpoPropSet.JsFuncs)
            {
                this.BpoHtc.Definition.AppendLine("<public:method name=\""+func.Name+"\"/>");
            }
            //标准事件定义
            this.BpoHtc.Definition.AppendLine("<public:event id=\"OnBeforeOpen\" name=\"onbeforeopen\">");
            this.BpoHtc.Definition.AppendLine("<public:event id=\"OnAfterOpen\" name=\"onafteropen\">");
            this.BpoHtc.Definition.AppendLine("<public:event id=\"OnBPObjectReady\" name=\"onbpobjectready\">");
            this.BpoHtc.Definition.AppendLine("<public:event id=\"OnAfterCondiQuery\" name=\"onaftercondiquery\">");
            //标准属性定义
            this.BpoHtc.Definition.AppendLine("<public:property name=\"BusiViewModes\" get=\"getBusiViewModes\"/>");
            this.BpoHtc.Definition.AppendLine("<public:property name=\"BusinessDataPacket\" get=\"getBusinessDataPacket\"/>");
            
            //BPOName 相关定义
            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                this.BpoHtc.Definition.AppendLine("<public:property name=\""+bc.Name+"Columns\" get=\"get"+bc.Name+"Columns\"/>");
            }
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    this.BpoHtc.Definition.AppendLine("<public:method name=\""+vc.VCName+"ExtMenuClick\"/>");
                    this.BpoHtc.Definition.AppendLine("<public:property name=\"" + vc.VCName + "Columns\" get=\"get" + vc.VCName + "Columns\"/>");
                }
            }
            this.BpoHtc.Definition.AppendLine("</public:component>");
            //变量定义
            this.BpoHtc.Statement.AppendLine("var __BusiViewModes=null;");
            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                this.BpoHtc.Statement.AppendLine("var __" + bc.Name + "Columns;");
            }
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    this.BpoHtc.Statement.AppendLine("var __" + vc.VCName + "Columns;");
                }
            }
            this.BpoHtc.Statement.AppendLine("var objColumn;");
            this.BpoHtc.Statement.AppendLine("var TaskList = new Array();");
            this.BpoHtc.Statement.AppendLine("var iCallID=0;");
            this.BpoHtc.Statement.AppendLine("var iApplyCallID=-1;");
            this.BpoHtc.Statement.AppendLine("var theBPOName=\"" + this.Name + "\";");
            this.BpoHtc.Statement.AppendLine("var BusinessID=0;");
            this.BpoHtc.Statement.AppendLine("var ServiceHandle=null;");
            this.BpoHtc.Statement.AppendLine("var UseTableList = new Array();");
            this.BpoHtc.Statement.AppendLine("var theDataPacket ;");
            this.BpoHtc.Statement.AppendLine("var iSingleCallID=9;");
            this.BpoHtc.Statement.AppendLine("var theSingleTable=null;");
            this.BpoHtc.Statement.AppendLine("var fPromptSubmit=true;");
            this.BpoHtc.Statement.AppendLine("var theDeltaData=createXMLObject();");
            this.BpoHtc.Statement.AppendLine("theDeltaData.loadXML(\"<root/>\");");
            this.BpoHtc.Statement.AppendLine("var COMMAND=\"\";");
            this.BpoHtc.Statement.AppendLine("var OwnerFlow=\"\";");
            this.BpoHtc.Statement.AppendLine("var  objChangeBusiView = new ChangeBusiViewObj();");
            this.BpoHtc.Statement.AppendLine("TaskList[TaskList.length] = objChangeBusiView;");
            this.BpoHtc.Statement.AppendLine("var  objcondiQuery = new condiQueryObj();");
            this.BpoHtc.Statement.AppendLine("TaskList[TaskList.length] = objcondiQuery;");
            this.BpoHtc.Statement.AppendLine("ObjectgetTotalDataTask = new DoResultgetTotalData();");
            this.BpoHtc.Statement.AppendLine("TaskList[TaskList.length] = ObjectgetTotalDataTask;");

            this.BpoHtc.Statement.AppendLine("var TotalTextBoxIndex=0;");
            this.BpoHtc.Statement.AppendLine("var TotalVCName=\"\";");

            this.BpoHtc.Statement.AppendLine("ObjectputTOExcelTask = new DoResultputTOExcel();");
            this.BpoHtc.Statement.AppendLine("TaskList[TaskList.length] = ObjectputTOExcelTask;");

            this.BpoHtc.Statement.AppendLine("var  objcondiQueryEx = new condiQueryExObj();");
            this.BpoHtc.Statement.AppendLine("TaskList[TaskList.length] = objcondiQueryEx;");

            this.BpoHtc.Statement.AppendLine("var flowTask=false;");

            this.BpoHtc.Statement.AppendLine("var  localxml = createXMLObject();");

            this.BpoHtc.Statement.AppendLine("var theCurrentBCLink=null;");
            this.BpoHtc.Statement.AppendLine("var BCLinkWin=null;");

            this.BpoHtc.Statement.AppendLine("");
            this.BpoHtc.Statement.AppendLine("");

            this.BpoHtc.Statement.AppendLine("");
            //函数定义
            //BPO相关函数

            //PrepareColumn
            JsFunction PrepareColumn = new JsFunction("PrepareColumn");
            PrepareColumn.Content.AppendLine("__BusiViewModes = new Array();");

            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                JsFunction bcColumn = new JsFunction("get"+bc.Name+"Columns");
                bcColumn.Content.AppendLine("return __"+bc.Name+"Columns;");
                BpoHtc.FuncList.Add(bcColumn);

                //OnFieldChange
                JsFunction OnFieldChange = new JsFunction(bc.Name + "OnFieldChange");
                BpoHtc.FuncList.Add(OnFieldChange);

                //Prepare BC Column
                string bcCol = "__" + bc.Name + "Columns";
                PrepareColumn.Content.AppendLine(bcCol+" = new Array();");
               
                int i = 0;
                int index = 0;
                foreach (BusiCompColumn column in bc.Columns)
                {
                    if (!column.fDisplay) { i++; continue; };
                    PrepareColumn.Content.AppendLine("objColumn = new Object();");
                    PrepareColumn.Content.AppendLine("objColumn.FieldName = \""+bc.Columns[i].FieldName+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.Caption = \""+bc.Columns[i].Caption+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.Width = "+bc.Columns[i].Width+";");
                    PrepareColumn.Content.AppendLine("objColumn.FieldLength = "+bc.Columns[i].FieldLength+";");
                    PrepareColumn.Content.AppendLine("objColumn.DecLength ="+bc.Columns[i].DecLength+";");
                    PrepareColumn.Content.AppendLine("objColumn.EditType = \""+bc.Columns[i].EditType+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.FieldType = \"" + GetUcmlTypeName(bc.Columns[i].FieldType) + "\";");
                    PrepareColumn.Content.AppendLine("objColumn.CodeTable = \""+bc.Columns[i].CodeTable+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.fUseCodeTable = "+bc.Columns[i].fUseCodeTable.ToString().ToLower()+";");
                    PrepareColumn.Content.AppendLine("objColumn.fAllowNull = " + bc.Columns[i].fAllowNull.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("objColumn.fDisplay = " + bc.Columns[i].fDisplay.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("objColumn.CurrentPos = "+bc.Columns[i].CurrentPos+";");
                    PrepareColumn.Content.AppendLine("objColumn.DefaultValue = \""+bc.Columns[i].DefaultValue+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.fCanModify = " + bc.Columns[i].fCanModify.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("objColumn.fAllowPick = " + bc.Columns[i].fAllowPick.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("objColumn.RoleTable = \""+bc.Columns[i].RoleTable+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.ForeignKeyField = \""+bc.Columns[i].ForeignKeyField+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.LookupKeyField = \""+bc.Columns[i].LookupKeyField+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.LookupDataSet = \""+bc.Columns[i].LookupDataSet+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.LookupResultField = \""+bc.Columns[i].LookupResultField+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.QueryBPOID = \""+bc.Columns[i].QueryBPOID+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.fForeignKey = " + bc.Columns[i].fForeignKey.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("objColumn.FieldKind = "+bc.Columns[i].FieldKind+";");
                    PrepareColumn.Content.AppendLine("objColumn.IsMultiValueField = " + bc.Columns[i].IsMultiValueField.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("objColumn.MultiValueTable = \""+bc.Columns[i].MultiValueTable+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.fFunctionInitValue = " + bc.Columns[i].fFunctionInitValue.ToString().ToLower() + ";");
                    PrepareColumn.Content.AppendLine("objColumn.InitValueFunc = \""+bc.Columns[i].InitValueFunc+"\";");
                    PrepareColumn.Content.AppendLine("objColumn.ExcelColNo = "+bc.Columns[i].ExcelColNo+";");
                    if (!String.IsNullOrWhiteSpace(bc.Columns[i].BCLink.AppletName))
                    {
                        PrepareColumn.Content.AppendLine("var objBCLinkColl = new Array();");
                        PrepareColumn.Content.AppendLine("var objBCLink = new Object();");
                        PrepareColumn.Content.AppendLine("objBCLink.QueryFieldName = \""+bc.Columns[i].BCLink.QueryFieldName+"\";");
                        PrepareColumn.Content.AppendLine("objBCLink.fQuickQuery = " + bc.Columns[i].BCLink.fQuickQuery.ToString().ToLower()+ ";");
                        PrepareColumn.Content.AppendLine("objBCLink.fDropDownMode= " + bc.Columns[i].BCLink.fDropDownMode.ToString().ToLower() + ";");
                        PrepareColumn.Content.AppendLine("objBCLink.DropDownWidth= " + bc.Columns[i].BCLink.DropDownWidth+ ";");
                        PrepareColumn.Content.AppendLine("objBCLink.DropDownHeight= " + bc.Columns[i].BCLink.DropDownHeight+ ";");
                        PrepareColumn.Content.AppendLine("objBCLink.BCName = \"" + bc.Columns[i].BCLink.AppletName+ "\";");
                        PrepareColumn.Content.AppendLine("objBCLink.Caption = \"" + bc.Columns[i].BCLink.Caption+ "\";");
                        PrepareColumn.Content.AppendLine("objBCLink.BCRunMode = "+bc.Columns[i].BCLink.RunMode+";");
                        PrepareColumn.Content.AppendLine("objBCLink.srcFieldName = \"" + bc.Columns[i].BCLink.srcFieldName+ "\";");
                        PrepareColumn.Content.AppendLine("objBCLink.destFieldName = \""+bc.Columns[i].BCLink.destFieldName+"\";");
                        PrepareColumn.Content.AppendLine("objBCLink.condiFieldName = \"" + bc.Columns[i].BCLink.condiFieldName+ "\";");
                        PrepareColumn.Content.AppendLine("objBCLink.constValue = \""+bc.Columns[i].BCLink.Value+"\";");
                        PrepareColumn.Content.AppendLine("objBCLink.targetTable = \""+bc.Name+"\";");
                        PrepareColumn.Content.AppendLine("objBCLink.urlFuncName = \""+bc.Name+"_"+bc.Columns[i].FieldName+"0\";");
                        PrepareColumn.Content.AppendLine("objBCLinkColl[objBCLinkColl.length] = objBCLink;");
                        PrepareColumn.Content.AppendLine("var objBCLoadCondiColl = new Array();");
                        PrepareColumn.Content.AppendLine("objBCLink.objBCLoadCondiColl = objBCLoadCondiColl;");
                        PrepareColumn.Content.AppendLine("objColumn.objBCLinkColl = objBCLinkColl;");
                    }
                    PrepareColumn.Content.AppendLine(bcCol+"["+index+"] = objColumn;");
                    index++;
                    i++;
                    PrepareColumn.Content.AppendLine("");
                }
            }

            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    JsFunction vcColumn = new JsFunction("get"+vc.VCName+"Columns");
                    vcColumn.Content.Append("return __"+vc.VCName+"Columns");
                    BpoHtc.FuncList.Add(vcColumn);

                    //ExtMenuClick
                    JsFunction ExtMenuClick = new JsFunction(vc.VCName + "ExtMenuClick");
                    ExtMenuClick.Params.Add("cmd");
                    BpoHtc.FuncList.Add(ExtMenuClick);

                    //menuready
                    JsFunction menuready = new JsFunction(vc.VCName + "menuready");
                    BpoHtc.FuncList.Add(menuready);

                    //Prepare for VC Column
                    string vcCol = "__" + vc.VCName + "Columns";
                    PrepareColumn.Content.AppendLine(vcCol+" = new Array();");
                    int i = 0;
                    foreach (UcmlVcColumn column in vc.Columns)
                    {
                        
                        PrepareColumn.Content.AppendLine("objColumn = new Object();");
                        PrepareColumn.Content.AppendLine("objColumn.FieldName = \""+vc.Columns[i].FieldName+"\";");
                        PrepareColumn.Content.AppendLine("objColumn.Caption = \""+vc.Columns[i].Caption+"\";");
                        PrepareColumn.Content.AppendLine("objColumn.fDisplay = " + vc.Columns[i].fDisplay.ToString().ToLower() + ";");
                        PrepareColumn.Content.AppendLine("objColumn.fCanModify = " + vc.Columns[i].fCanModify.ToString().ToLower() + ";");
                        PrepareColumn.Content.AppendLine("objColumn.CurrentPos = "+vc.Columns[i].CurrentPos+";");
                        
                        //计算列宽
                        if (vc.Columns[i].Width == 0) PrepareColumn.Content.AppendLine("objColumn.Width =50;");
                        PrepareColumn.Content.AppendLine("objColumn.Width = "+(vc.Columns[i].Width+50)+";");
                        PrepareColumn.Content.AppendLine("objColumn.fFixColumn = " + vc.Columns[i].fFixColumn.ToString().ToLower() + ";");
                        PrepareColumn.Content.AppendLine("objColumn.FixColumnValue = \""+vc.Columns[i].FixColumnValue+"\";");
                        PrepareColumn.Content.AppendLine("objColumn.fCustomerControl = " + vc.Columns[i].fCustomerControl.ToString().ToLower() + ";");
                        PrepareColumn.Content.AppendLine("objColumn.CustomerControlHTC = \""+vc.Columns[i].CustomerControlHTC+"\";");
                        PrepareColumn.Content.AppendLine("objColumn.ControlID = \""+vc.Columns[i].ControlID+"\";");
                        PrepareColumn.Content.AppendLine("objColumn.EditContrl = \"\";");
                        //查询VC需要的字段
                        if (vc.Kind == 165)
                        {
                            PrepareColumn.Content.AppendLine("objColumn.LeftBracket = \"" + vc.Columns[i].LeftBracket + "\";");
                            PrepareColumn.Content.AppendLine("objColumn.RightBracket = \"" + vc.Columns[i].RightBracket + "\";");
                            PrepareColumn.Content.AppendLine("objColumn.LogicConnect = \"" + vc.Columns[i].LogicConnect + "\";");
                            PrepareColumn.Content.AppendLine("objColumn.CondiFieldValue = \"" + vc.Columns[i].CondiFieldValue + "\";");
                            PrepareColumn.Content.AppendLine("objColumn.fIsFunctionValue = \"" + vc.Columns[i].fIsFunctionValue.ToString().ToLower() + "\";");
                            PrepareColumn.Content.AppendLine("objColumn.InnerLinkComp = \"" + vc.Columns[i].InnerLinkComp.ToString().ToLower() + "\";");
                            PrepareColumn.Content.AppendLine("objColumn.OperationIndent = \"" + vc.Columns[i].OperationIndent + "\";");
                        }
                        PrepareColumn.Content.AppendLine(vcCol + "[" + i + "] = objColumn;");
                        i++;
                    }
                }
            }

            //添加PrepareColumn
            BpoHtc.FuncList.Add(PrepareColumn);

            //BusinessInit
            JsFunction BusinessInit = new JsFunction("BusinessInit");

            //BeforeSubmit
            JsFunction BeforeSubmit = new JsFunction("BeforeSubmit");

            //AfterSubmit
            JsFunction AfterSubmit = new JsFunction("AfterSubmit");

            //添加BC级别的JS代码
            foreach (UcmlBusiCompPropSet bc in this.BCList)
            {
                //注册BCNameBase.OnCalculate事件，并添加BCNameOnCalculate函数
                if(!String.IsNullOrWhiteSpace(bc.OnCalculateScript))
                {
                    BusinessInit.Content.AppendLine(bc.Name+"Base.OnCalculate="+bc.Name+"OnCalculate;");
                    JsFunction bcOnCalculate = new JsFunction(bc.Name+"OnCalculate");
                    bcOnCalculate.Content.AppendLine(bc.OnCalculateScript);
                    BpoHtc.FuncList.Add(bcOnCalculate);
                }

                //注册BCNameBase.OnRecordChange事件，并添加BCNameOnRecordChange函数
                if (!String.IsNullOrWhiteSpace(bc.OnRecordChangeScript))
                {
                    BusinessInit.Content.AppendLine(bc.Name + "Base.OnRecordChange=" + bc.Name + "OnRecordChange;");
                    JsFunction bcOnRecordChange = new JsFunction(bc.Name + "OnRecordChange");
                    bcOnRecordChange.Content.AppendLine(bc.OnRecordChangeScript);
                    BpoHtc.FuncList.Add(bcOnRecordChange);
                }

                //注册BCNameBase.OnBeforeInsert事件，并添加BCNameOnBeforeInsert函数
                if (!String.IsNullOrWhiteSpace(bc.OnBeforeInsertScript))
                {
                    BusinessInit.Content.AppendLine(bc.Name+"Base.OnBeforeInsert="+bc.Name+"OnBeforeInsert;");
                    JsFunction bcOnBeforeInsert = new JsFunction(bc.Name+"OnBeforeInsert");
                    bcOnBeforeInsert.Content.AppendLine(bc.OnBeforeInsertScript);
                    BpoHtc.FuncList.Add(bcOnBeforeInsert);
                }

                //注册BCNameBase.OnAfterInsert事件，并添加BCNameOnAfterInsert函数
                if (!String.IsNullOrWhiteSpace(bc.OnAfterInsertScript))
                {
                    BusinessInit.Content.AppendLine(bc.Name + "Base.OnAfterInsert=" + bc.Name + "OnAfterInsert;");
                    JsFunction bcOnAfterInsert = new JsFunction(bc.Name + "OnAfterInsert");
                    bcOnAfterInsert.Content.AppendLine(bc.OnAfterInsertScript);
                    BpoHtc.FuncList.Add(bcOnAfterInsert);
                }

                //注册BCNameBase.OnFieldChange事件，并添加BCNameOnFieldChange函数
                BusinessInit.Content.AppendLine(bc.Name+"Base.OnFieldChange="+bc.Name+"OnFieldChange;");
                JsFunction bcOnFieldChange = new JsFunction(bc.Name+"OnFieldChange");
                foreach (BusiCompColumn column in bc.Columns)
                {
                    if(!String.IsNullOrWhiteSpace(column.OnFieldChangeScript))
                    {
                        bcOnFieldChange.Content.AppendLine("if (event.FieldName==\""+column.FieldName+"\")");
                        bcOnFieldChange.Content.AppendLine("{");
                        bcOnFieldChange.Content.AppendLine(column.OnFieldChangeScript);
                        bcOnFieldChange.Content.AppendLine("}");
                    }
                }
                BpoHtc.FuncList.Add(bcOnFieldChange);

                //初始化代码
                BusinessInit.Content.AppendLine(bc.InitScript);
                //提交前代码
                BeforeSubmit.Content.AppendLine(bc.BeforeUpdateScript);
                //提交后代码
                AfterSubmit.Content.AppendLine(bc.AfterUpdateScript);
            }

            //添加VC级别的JS代码
            foreach (UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    BusinessInit.Content.AppendLine(vc.InitScript);

                    //注册按钮事件
                    if (vc.Buttons.Count != 0)
                    {
                        BusinessInit.Content.AppendLine("ToolBar" + vc.VCName + ".onbuttonclick=" + vc.VCName + "ButtonClick;");
                        JsFunction btnClick = new JsFunction(vc.VCName + "ButtonClick");
                        int btnIndex = 0;
                        foreach (UcmlVcButton button in vc.Buttons)
                        {
                            if (button.Type == 0)
                            {
                                btnClick.Content.AppendLine("if(event.flatIndex==" + btnIndex + ")");
                                btnClick.Content.AppendLine("{");
                                string[] lines = Util.SplitLine(button.OnClickScript);
                                foreach (string line in lines) btnClick.Content.AppendLine("   " + line.Trim());
                                btnClick.Content.AppendLine("   return;");
                                btnClick.Content.AppendLine("}");
                            }
                            btnIndex++;
                        }
                        BpoHtc.FuncList.Add(btnClick);
                    }
                    BeforeSubmit.Content.AppendLine(vc.BeforeUpdateScript);
                    AfterSubmit.Content.AppendLine(vc.AfterApplyScript);
                }
            }

            //添加BPO级别的JS代码
            BusinessInit.Content.AppendLine(this.BpoPropSet.InitScript);
            BeforeSubmit.Content.AppendLine(this.BpoPropSet.BeforeSubmitScript);
            AfterSubmit.Content.AppendLine(this.BpoPropSet.AfterSubmitScript);
            
            BeforeSubmit.Content.AppendLine("return true;");

            BpoHtc.FuncList.Add(BusinessInit);
            BpoHtc.FuncList.Add(BeforeSubmit);
            BpoHtc.FuncList.Add(AfterSubmit);

            //CanSubmit
            JsFunction CanSubmit = new JsFunction("CanSubmit");
            CanSubmit.Content.AppendLine("var result=true;");
            CanSubmit.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            CanSubmit.Content.AppendLine("{");
            CanSubmit.Content.AppendLine("   result = UseTableList[i].Valiate();");
            CanSubmit.Content.AppendLine("   if (result==false) break;");
            CanSubmit.Content.AppendLine("}");
            CanSubmit.Content.AppendLine("return result;");
            BpoHtc.FuncList.Add(CanSubmit);

            //BusinessSubmit
            JsFunction BusinessSubmit = new JsFunction("BusinessSubmit");
            BusinessSubmit.Content.AppendLine("if ( CanSubmit()==false) return;");
            BusinessSubmit.Content.AppendLine("if ( BeforeSubmit()==false) return;");
            BusinessSubmit.Content.AppendLine("this.ServiceHandle = eval(ServiceID);");
            BusinessSubmit.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            BusinessSubmit.Content.AppendLine("{");
            BusinessSubmit.Content.AppendLine("   UseTableList[i].NotifyControls(\"Post\");");
            BusinessSubmit.Content.AppendLine("}");
            BusinessSubmit.Content.AppendLine("if (ServiceHandle!=null)");
            BusinessSubmit.Content.AppendLine("{");
            BusinessSubmit.Content.AppendLine("   ShowMessage(\"正在提交信息，请等待......\");");
            BusinessSubmit.Content.AppendLine("   ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            BusinessSubmit.Content.AppendLine("   var srv=eval(\"ServiceHandle.\"+theBPOName+\"Service\");");
            BusinessSubmit.Content.AppendLine("   this.iApplyCallID=srv.callService(\"BusinessSubmit\",\"<![CDATA[\" + theDeltaData.documentElement.xml + \"]]>\");");
            BusinessSubmit.Content.AppendLine("}");
            BpoHtc.FuncList.Add(BusinessSubmit);

            //getBusiViewModes
            JsFunction getbusiView = new JsFunction("getBusiViewModes");
            getbusiView.Content.Append("return __BusiViewModes;");
            BpoHtc.FuncList.Add(getbusiView);

            //getTaskList
            JsFunction getTaskList = new JsFunction("getTaskList");
            getTaskList.Content.Append("return TaskList;");
            BpoHtc.FuncList.Add(getTaskList);

            //open函数
            JsFunction open = new JsFunction("open");
            open.Content.AppendLine("var  pk=createXMLObject();");
            open.Content.AppendLine("pk.loadXML(UCMLBUSIOBJECT.innerHTML);");
            open.Content.AppendLine("theDataPacket = pk;");
            open.Content.AppendLine("var evObj = createEventObject();");
            open.Content.AppendLine(" OnBeforeOpen.fire(evObj);");
            open.Content.AppendLine("PrepareColumn();");
            open.Content.AppendLine("var evObj = createEventObject();");
            open.Content.Append("OnAfterOpen.fire(evObj);");
            BpoHtc.FuncList.Add(open);
            
            //HideSelect
            JsFunction hideSelect = new JsFunction("HideSelect");
            hideSelect.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            hideSelect.Content.AppendLine("{");
            hideSelect.Content.AppendLine("    UseTableList[i].NotifyControls(\"HideSelect\");");
            hideSelect.Content.AppendLine("}");
            BpoHtc.FuncList.Add(hideSelect);
            
            //ShowSelect
            JsFunction showSelect = new JsFunction("ShowSelect");
            showSelect.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            showSelect.Content.AppendLine("{");
            showSelect.Content.AppendLine("UseTableList[i].NotifyControls(\"ShowSelect\");");
            showSelect.Content.AppendLine("}");
            BpoHtc.FuncList.Add(showSelect);

            //AddUseTable
            JsFunction addUseTable = new JsFunction("AddUseTable");
            addUseTable.Params.Add("DataTable");
            addUseTable.Content.AppendLine("UseTableList[UseTableList.length]=DataTable;");
            addUseTable.Content.AppendLine("DataTable.DeltaData = theDeltaData;");
            BpoHtc.FuncList.Add(addUseTable);

            //NotifyTable
            JsFunction notifyTable = new JsFunction("NotifyTable");
            notifyTable.Params.Add("xmldoc");
            notifyTable.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            notifyTable.Content.AppendLine("{");
            notifyTable.Content.AppendLine("    UseTableList[i].SetDataPacket(xmldoc);");
            notifyTable.Content.AppendLine("}");
            notifyTable.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            notifyTable.Content.AppendLine("{");
            notifyTable.Content.AppendLine("    UseTableList[i].SetControlDataPacket(xmldoc);");
            notifyTable.Content.AppendLine("}");
            BpoHtc.FuncList.Add(notifyTable);

            //NotifyTable
            JsFunction notifyTableNoneActor = new JsFunction("NotifyTableNoneActor");
            notifyTableNoneActor.Params.Add("xmldoc");
            notifyTableNoneActor.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            notifyTableNoneActor.Content.AppendLine("{");
            notifyTableNoneActor.Content.AppendLine("    if (UseTableList[i].TableType!=\"A\")");
            notifyTableNoneActor.Content.AppendLine("        UseTableList[i].SetDataPacketAndChild(xmldoc);");
            notifyTableNoneActor.Content.AppendLine("}");
            notifyTableNoneActor.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            notifyTableNoneActor.Content.AppendLine("{");
            notifyTableNoneActor.Content.AppendLine("    if (UseTableList[i].TableType!=\"A\")");
            notifyTableNoneActor.Content.AppendLine("        UseTableList[i].SetControlDataPacket(xmldoc);");
            notifyTableNoneActor.Content.AppendLine("}");
            BpoHtc.FuncList.Add(notifyTableNoneActor);

            //getRootTable
            JsFunction getRootTable = new JsFunction("getRootTable");
            getRootTable.Content.AppendLine("return UseTableList[0];");
            BpoHtc.FuncList.Add(getRootTable);

            //
            JsFunction InitBusinessEnv = new JsFunction("InitBusinessEnv");
            InitBusinessEnv.Content.AppendLine("COMMAND=getURLParameters(\"COMMAND\");");
            InitBusinessEnv.Content.AppendLine("var Enabled=getURLParameters(\"ENABLED\");");
            InitBusinessEnv.Content.AppendLine("if (Enabled==\"false\")");
            InitBusinessEnv.Content.AppendLine("{");
            InitBusinessEnv.Content.AppendLine("    for ( var i=0;i<UseTableList.length;i++)");
            InitBusinessEnv.Content.AppendLine("    {");
            InitBusinessEnv.Content.AppendLine("        UseTableList[i].EnableApplets(false);");
            InitBusinessEnv.Content.AppendLine("    }");
            InitBusinessEnv.Content.AppendLine("}");
            InitBusinessEnv.Content.AppendLine("OwnerFlow=\"\";");
            InitBusinessEnv.Content.AppendLine("ServiceHandle = eval(ServiceID);");
            InitBusinessEnv.Content.AppendLine("if(ServiceHandle!=null)");
            InitBusinessEnv.Content.AppendLine("{");
            InitBusinessEnv.Content.AppendLine("    ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            InitBusinessEnv.Content.AppendLine("}");
            InitBusinessEnv.Content.AppendLine("NotifyTable(theDataPacket);");
            InitBusinessEnv.Content.AppendLine("BusinessInit();");
            InitBusinessEnv.Content.AppendLine("var evObj = createEventObject();");
            InitBusinessEnv.Content.AppendLine("OnBPObjectReady.fire(evObj);");
            InitBusinessEnv.Content.AppendLine("return;");
            BpoHtc.FuncList.Add(InitBusinessEnv);

            //getServiceHandle
            JsFunction getServiceHandle = new JsFunction("getServiceHandle");
            getServiceHandle.Content.AppendLine("ServiceHandle = eval(ServiceID);");
            getServiceHandle.Content.AppendLine("return ServiceHandle;");
            BpoHtc.FuncList.Add(getServiceHandle);

            //ChangeBusiViewObj
            JsFunction ChangeBusiViewObj = new JsFunction("ChangeBusiViewObj");
            ChangeBusiViewObj.Content.AppendLine("var CallID;");
            ChangeBusiViewObj.Content.AppendLine("this.DoResult = function(result)");
            ChangeBusiViewObj.Content.AppendLine("{");
            ChangeBusiViewObj.Content.AppendLine("    HideMessage();");
            ChangeBusiViewObj.Content.AppendLine("    if(result.error==true)");
            ChangeBusiViewObj.Content.AppendLine("    {");
            ChangeBusiViewObj.Content.AppendLine("        var xfaultstring = result.errorDetail.string;");
            ChangeBusiViewObj.Content.AppendLine("        alert(xfaultstring);");
            ChangeBusiViewObj.Content.AppendLine("        return;");
            ChangeBusiViewObj.Content.AppendLine("    }");
            ChangeBusiViewObj.Content.AppendLine("    var pk = result.raw;");
            ChangeBusiViewObj.Content.AppendLine("    if (theDataPacket==null)");
            ChangeBusiViewObj.Content.AppendLine("    {");
            ChangeBusiViewObj.Content.AppendLine("        theDataPacket = pk;");
            ChangeBusiViewObj.Content.AppendLine("        NotifyTable(pk);");
            ChangeBusiViewObj.Content.AppendLine("        BusinessInit();");
            ChangeBusiViewObj.Content.AppendLine("    }");
            ChangeBusiViewObj.Content.AppendLine("    else");
            ChangeBusiViewObj.Content.AppendLine("    {");
            ChangeBusiViewObj.Content.AppendLine("        NotifyTable(pk);");
            ChangeBusiViewObj.Content.AppendLine("    }");
            ChangeBusiViewObj.Content.AppendLine("}");
            BpoHtc.FuncList.Add(ChangeBusiViewObj);

            //ChangeBusiView
            JsFunction ChangeBusiView = new JsFunction("ChangeBusiView");
            ChangeBusiView.Params.Add("viewType");
            ChangeBusiView.Content.AppendLine("this.ServiceHandle = eval(ServiceID);");
            ChangeBusiView.Content.AppendLine("if (ServiceHandle!=null)");
            ChangeBusiView.Content.AppendLine("{");
            ChangeBusiView.Content.AppendLine("    ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            ChangeBusiView.Content.AppendLine("    eval(\"objChangeBusiView.CallID =ServiceHandle.\"+theBPOName+\"Service.callService(\\\"ChangeBusiView\\\",viewType)\");");
            ChangeBusiView.Content.AppendLine("}");
            BpoHtc.FuncList.Add(ChangeBusiView);

            //condiQueryObj
            JsFunction condiQueryObj = new JsFunction("condiQueryObj");
            condiQueryObj.Content.AppendLine("var CallID;");
            condiQueryObj.Content.AppendLine("this.DoResult = function(result)");
            condiQueryObj.Content.AppendLine("{");
            condiQueryObj.Content.AppendLine("    HideMessage();");
            condiQueryObj.Content.AppendLine("    if(result.error==true)");
            condiQueryObj.Content.AppendLine("    {");
            condiQueryObj.Content.AppendLine("        var xfaultstring = result.errorDetail.string;");
            condiQueryObj.Content.AppendLine("        alert(xfaultstring);");
            condiQueryObj.Content.AppendLine("        return;");
            condiQueryObj.Content.AppendLine("    }");
            condiQueryObj.Content.AppendLine("    var pk = result.raw;");
            condiQueryObj.Content.AppendLine("    if (theDataPacket==null)");
            condiQueryObj.Content.AppendLine("    {");
            condiQueryObj.Content.AppendLine("        NotifyTableNoneActor(pk);;");
            condiQueryObj.Content.AppendLine("        BusinessInit();");
            condiQueryObj.Content.AppendLine("    }");
            condiQueryObj.Content.AppendLine("    else");
            condiQueryObj.Content.AppendLine("    {");
            condiQueryObj.Content.AppendLine("        NotifyTableNoneActor(pk);");
            condiQueryObj.Content.AppendLine("    }");
            condiQueryObj.Content.AppendLine("    var evObj = createEventObject();");
            condiQueryObj.Content.AppendLine("    OnAfterCondiQuery.fire(evObj);");
            condiQueryObj.Content.AppendLine("}");
            BpoHtc.FuncList.Add(condiQueryObj);

            //getTotalData
            JsFunction getTotalData = new JsFunction("getTotalData");
            getTotalData.Params.Add("BCName");
            getTotalData.Params.Add("fieldList");
            getTotalData.Params.Add("valueList");
            getTotalData.Params.Add("condiIndentList");
            getTotalData.Params.Add("SQLCondi");
            getTotalData.Params.Add("SQLCondiType");
            getTotalData.Params.Add("SQLFix");
            getTotalData.Params.Add("TotalFields");
            getTotalData.Params.Add("TotalModes");
            getTotalData.Content.AppendLine("var BCBase = eval(BCName+\"Base\");");
            getTotalData.Content.AppendLine("fieldList = BCBase.fieldList; valueList = BCBase.valueList; condiIndentList = BCBase.condiIndentList; SQLCondi=BCBase.SQLCondi;");
            getTotalData.Content.AppendLine("this.ServiceHandle=eval(ServiceID);");
            getTotalData.Content.AppendLine("if(ServiceHandle!=null)");
            getTotalData.Content.AppendLine("{");
            getTotalData.Content.AppendLine("    ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            getTotalData.Content.AppendLine("    var srv=eval(\"ServiceHandle.\"+theBPOName+\"Service\");");
            getTotalData.Content.AppendLine("    ObjectgetTotalDataTask.CallID=srv.callService(\"getTotalData\",BCName,fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType,SQLFix,TotalFields,TotalModes);");
            getTotalData.Content.AppendLine("}");
            BpoHtc.FuncList.Add(getTotalData);

            //DoResultgetTotalData
            JsFunction DoResultgetTotalData = new JsFunction("DoResultgetTotalData");
            DoResultgetTotalData.Content.AppendLine("this.DoResult = function(result)");
            DoResultgetTotalData.Content.AppendLine("{");
            DoResultgetTotalData.Content.AppendLine("    if(result.error==false)");
            DoResultgetTotalData.Content.AppendLine("    {");
            DoResultgetTotalData.Content.AppendLine("        var values=event.result.value;");
            DoResultgetTotalData.Content.AppendLine("        var obj = eval(\"ToolBar\"+TotalVCName+\".getItem(TotalTextBoxIndex)\");");
            DoResultgetTotalData.Content.AppendLine("        obj.setAttribute(\"Value\",values);");
            DoResultgetTotalData.Content.AppendLine("    }");
            DoResultgetTotalData.Content.AppendLine("}");
            BpoHtc.FuncList.Add(DoResultgetTotalData);

            //ExportToExcel
            JsFunction ExportToExcel = new JsFunction("ExportToExcel");
            ExportToExcel.Content.AppendLine("ShowMessage(\"正在执行导出，请等待......\");");
            ExportToExcel.Content.AppendLine("var nFieldLists=\"\";");
            ExportToExcel.Content.AppendLine("if (MergerFieldIndex==undefined)  MergerFieldIndex=\"\";");
            ExportToExcel.Content.AppendLine("if (TitleTextLists==undefined)  TitleTextLists=\"\";");
            ExportToExcel.Content.AppendLine("for (var i=0; i<VCObject.AppletColumn.length; i++)");
            ExportToExcel.Content.AppendLine("{");
            ExportToExcel.Content.AppendLine("    if (nFieldLists!=\"\") nFieldLists+=\";\";");
            ExportToExcel.Content.AppendLine("    nFieldLists += VCObject.AppletColumn[i].FieldName;");
            ExportToExcel.Content.AppendLine("}");
            ExportToExcel.Content.AppendLine("var BCBase = eval(BCName+\"Base\");");
            ExportToExcel.Content.AppendLine("var fieldList = BCBase.fieldList; var valueList = BCBase.valueList; var condiIndentList = BCBase.condiIndentList;");
            ExportToExcel.Content.AppendLine("if (SQLCondi==undefined||SQLCondi==\"\") SQLCondi=BCBase.SQLCondi;");
            ExportToExcel.Content.AppendLine("var SQLCondiType=0;");
            ExportToExcel.Content.AppendLine("this.ServiceHandle = eval(ServiceID);");
            ExportToExcel.Content.AppendLine("condiIndentList=\"<![CDATA[\" + condiIndentList + \"]]>\";");
            ExportToExcel.Content.AppendLine("valueList=\"<![CDATA[\" + valueList + \"]]>\";");
            ExportToExcel.Content.AppendLine("if(ServiceHandle!=null)");
            ExportToExcel.Content.AppendLine("{");
            ExportToExcel.Content.AppendLine("    ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            ExportToExcel.Content.AppendLine("    var srv=eval(\"ServiceHandle.\"+theBPOName+\"Service\");");
            ExportToExcel.Content.AppendLine("    ObjectputTOExcelTask.CallID=srv.callService(\"PutToExcelEX\",BCName,nFieldLists,0,fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType,\"\",UCMLLocalResourcePath,VCObject.id,MergerFieldIndex, TitleTextLists);");
            ExportToExcel.Content.AppendLine("}");
            BpoHtc.FuncList.Add(ExportToExcel);

            //DoResultputTOExcel
            JsFunction DoResultputTOExcel = new JsFunction("DoResultputTOExcel");
            DoResultputTOExcel.Content.AppendLine("this.DoResult = function(result)");
            DoResultputTOExcel.Content.AppendLine("{");
            DoResultputTOExcel.Content.AppendLine("    if(result.error==false)");
            DoResultputTOExcel.Content.AppendLine("    {");
            DoResultputTOExcel.Content.AppendLine("        var xmldoc=event.result.raw;");
            DoResultputTOExcel.Content.AppendLine("        var FiledName = event.result.value;");
            DoResultputTOExcel.Content.AppendLine("        if( FiledName.length>0)");
            DoResultputTOExcel.Content.AppendLine("        {");
            DoResultputTOExcel.Content.AppendLine("            window.open(FiledName) ;");
            DoResultputTOExcel.Content.AppendLine("        }");
            DoResultputTOExcel.Content.AppendLine("        else");
            DoResultputTOExcel.Content.AppendLine("        {");
            DoResultputTOExcel.Content.AppendLine("            alert(\"导出失败\");");
            DoResultputTOExcel.Content.AppendLine("        }");
            DoResultputTOExcel.Content.AppendLine("    }");
            DoResultputTOExcel.Content.AppendLine("}");
            BpoHtc.FuncList.Add(DoResultputTOExcel);

            //condiQuery
            JsFunction condiQuery = new JsFunction("condiQuery");
            condiQuery.Params.Add("fieldList");
            condiQuery.Params.Add("valueList");
            condiQuery.Params.Add("condiIndentList");
            condiQuery.Params.Add("SQLCondi");
            condiQuery.Params.Add("SQLCondiType");
            condiQuery.Params.Add("nPageCount");
            condiQuery.Content.AppendLine("ShowMessage(\"正在执行查询，请等待......\");");
            condiQuery.Content.AppendLine("var rootTable = getRootTable();");
            condiQuery.Content.AppendLine("rootTable.SetCondiList(fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType);");
            condiQuery.Content.AppendLine("this.ServiceHandle = eval(ServiceID);");
            condiQuery.Content.AppendLine("if (SQLCondi==undefined)");
            condiQuery.Content.AppendLine("{");
            condiQuery.Content.AppendLine("    SQLCondi=\"\";");
            condiQuery.Content.AppendLine("    SQLCondiType=0;");
            condiQuery.Content.AppendLine("}");
            condiQuery.Content.AppendLine("if (nPageCount==undefined)");
            condiQuery.Content.AppendLine("{");
            condiQuery.Content.AppendLine("    nPageCount=-1;");
            condiQuery.Content.AppendLine("}");
            condiQuery.Content.AppendLine("condiIndentList=\"<![CDATA[\" + condiIndentList + \"]]>\";");
            condiQuery.Content.AppendLine("valueList=\"<![CDATA[\" + valueList + \"]]>\";");
            condiQuery.Content.AppendLine("if (ServiceHandle!=null)");
            condiQuery.Content.AppendLine("{");
            condiQuery.Content.AppendLine("    ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            condiQuery.Content.AppendLine("    eval(\"objcondiQuery.CallID =ServiceHandle.\"+theBPOName+\"Service.callService('getCondiBusinessData',0,nPageCount,fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType)\");");
            condiQuery.Content.AppendLine("}");
            BpoHtc.FuncList.Add(condiQuery);

            //condiQueryExObj
            JsFunction condiQueryExObj = new JsFunction("condiQueryExObj");
            condiQueryExObj.Content.AppendLine("var CallID;");
            condiQueryExObj.Content.AppendLine("this.DoResult = function(result)");
            condiQueryExObj.Content.AppendLine("{");
            condiQueryExObj.Content.AppendLine("    HideMessage();");
            condiQueryExObj.Content.AppendLine("    if(result.error==true)");
            condiQueryExObj.Content.AppendLine("    {");
            condiQueryExObj.Content.AppendLine("       var xfaultstring = result.errorDetail.string;");
            condiQueryExObj.Content.AppendLine("       alert(xfaultstring);");
            condiQueryExObj.Content.AppendLine("       return;");
            condiQueryExObj.Content.AppendLine("    }");
            condiQueryExObj.Content.AppendLine("    var pk = result.raw;");
            condiQueryExObj.Content.AppendLine("    if (theDataPacket==null)");
            condiQueryExObj.Content.AppendLine("    {");
            condiQueryExObj.Content.AppendLine("       NotifyTable(pk);");
            condiQueryExObj.Content.AppendLine("       BusinessInit();");
            condiQueryExObj.Content.AppendLine("    }");
            condiQueryExObj.Content.AppendLine("    else");
            condiQueryExObj.Content.AppendLine("    {");
            condiQueryExObj.Content.AppendLine("       NotifyTable(pk);");
            condiQueryExObj.Content.AppendLine("    }");
            condiQueryExObj.Content.AppendLine("    var evObj = createEventObject();");
            condiQueryExObj.Content.AppendLine("    OnAftercondiQuery.fire(evObj);");
            condiQueryExObj.Content.AppendLine("}");
            condiQueryExObj.Content.AppendLine("");
            condiQueryExObj.Content.AppendLine("");
            BpoHtc.FuncList.Add(condiQueryExObj);

            //condiQueryEx
            JsFunction condiQueryEx = new JsFunction("condiQueryEx");
            condiQueryEx.Params.Add("fieldList");
            condiQueryEx.Params.Add("valueList");
            condiQueryEx.Params.Add("condiIndentList");
            condiQueryEx.Params.Add("SQLCondi");
            condiQueryEx.Params.Add("SQLCondiType");
            condiQueryEx.Content.AppendLine("ShowMessage(\"正在执行查询，请等待......\");");
            condiQueryEx.Content.AppendLine("var rootTable = getRootTable();");
            condiQueryEx.Content.AppendLine("rootTable.SetCondiList(fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType);");
            condiQueryEx.Content.AppendLine("this.ServiceHandle = eval(ServiceID);");
            condiQueryEx.Content.AppendLine("if (SQLCondi==undefined)");
            condiQueryEx.Content.AppendLine("{");
            condiQueryEx.Content.AppendLine("    SQLCondi=\"\";");
            condiQueryEx.Content.AppendLine("    SQLCondiType=0;");
            condiQueryEx.Content.AppendLine("}");
            condiQueryEx.Content.AppendLine("condiIndentList=\"<![CDATA[\" + condiIndentList + \"]]>\";");
            condiQueryEx.Content.AppendLine("valueList=\"<![CDATA[\" + valueList + \"]]>\";");
            condiQueryEx.Content.AppendLine("if (ServiceHandle!=null)");
            condiQueryEx.Content.AppendLine("{");
            condiQueryEx.Content.AppendLine("    ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            condiQueryEx.Content.AppendLine("    eval(\"objcondiQueryEx.CallID =ServiceHandle.\"+theBPOName+\"Service.callService('getCondiBusinessDataEx',0,10,fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType,'',SubBC_SQLCondi)\");");
            condiQueryEx.Content.AppendLine("}");
            BpoHtc.FuncList.Add(condiQueryEx);

            //condiActorQuery
            JsFunction condiActorQuery = new JsFunction("condiActorQuery");
            condiActorQuery.Params.Add("TableName");
            condiActorQuery.Params.Add("fieldList");
            condiActorQuery.Params.Add("valueList");
            condiActorQuery.Params.Add("condiIndentList");
            condiActorQuery.Params.Add("SQLCondi");
            condiActorQuery.Params.Add("SQLCondiType");
            condiActorQuery.Content.AppendLine("ShowMessage(\"正在执行查询，请等待......\");");
            condiActorQuery.Content.AppendLine("theSingleTable=TableName;");
            condiActorQuery.Content.AppendLine("this.ServiceHandle = eval(ServiceID);");
            condiActorQuery.Content.AppendLine("if (SQLCondi==undefined)");
            condiActorQuery.Content.AppendLine("{");
            condiActorQuery.Content.AppendLine("   SQLCondi=\"\";");
            condiActorQuery.Content.AppendLine("   SQLCondiType=0;");
            condiActorQuery.Content.AppendLine("}");
            condiActorQuery.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            condiActorQuery.Content.AppendLine("{");
            condiActorQuery.Content.AppendLine("   if (UseTableList[i].TableName==TableName)");
            condiActorQuery.Content.AppendLine("   {");
            condiActorQuery.Content.AppendLine("      UseTableList[i].SetCondiList(fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType);");
            condiActorQuery.Content.AppendLine("      break;");
            condiActorQuery.Content.AppendLine("   }");
            condiActorQuery.Content.AppendLine("}");
            condiActorQuery.Content.AppendLine("condiIndentList=\"<![CDATA[\" + condiIndentList + \"]]>\";");
            condiActorQuery.Content.AppendLine("valueList=\"<![CDATA[\" + valueList + \"]]>\";");
            condiActorQuery.Content.AppendLine("if (ServiceHandle!=null)");
            condiActorQuery.Content.AppendLine("{");
            condiActorQuery.Content.AppendLine("   ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            condiActorQuery.Content.AppendLine("   eval(\"this.iSingleCallID =ServiceHandle.\"+theBPOName+\"Service.callService('getCondiActorData',TableName,0,10,fieldList,valueList,condiIndentList,SQLCondi,SQLCondiType)\");");
            condiActorQuery.Content.AppendLine("}");
            BpoHtc.FuncList.Add(condiActorQuery);

            //Report_Compute
            JsFunction Report_Compute = new JsFunction("Report_Compute");
            BpoHtc.FuncList.Add(Report_Compute);

            //getBCList
            JsFunction getBCList = new JsFunction("getBCList");
            getBCList.Content.AppendLine("return UseTableList;");
            BpoHtc.FuncList.Add(getBCList);

            //FinishTask
            JsFunction FinishTask = new JsFunction("FinishTask");
            FinishTask.Content.AppendLine("this.ServiceHandle = eval(ServiceID);");
            FinishTask.Content.AppendLine("if (ServiceHandle!=null)");
            FinishTask.Content.AppendLine("{");
            FinishTask.Content.AppendLine("   ServiceHandle.useService(theBPOName+\".asmx?WSDL\",theBPOName+\"Service\");");
            FinishTask.Content.AppendLine("   eval(\"this.iApplyCallID =ServiceHandle.\"+theBPOName+\"Service.callService(\\\"FinishTask\\\",(TaskID))\");");
            FinishTask.Content.AppendLine("}");
            BpoHtc.FuncList.Add(FinishTask);

            //LoadResults
            JsFunction LoadResults = new JsFunction("LoadResults");
            LoadResults.Params.Add("result");
            LoadResults.Content.AppendLine("HideMessage();");
            LoadResults.Content.AppendLine("if(event.result.error==true)");
            LoadResults.Content.AppendLine("{");
            LoadResults.Content.AppendLine("   var xfaultstring = event.result.errorDetail.string;");
            LoadResults.Content.AppendLine("   alert(xfaultstring);");
            LoadResults.Content.AppendLine("   return;");
            LoadResults.Content.AppendLine("}");
            LoadResults.Content.AppendLine("if(iSingleCallID == event.result.id)");
            LoadResults.Content.AppendLine("{");
            LoadResults.Content.AppendLine("   var pk = event.result.raw;");
            LoadResults.Content.AppendLine("   for ( var i=0;i<UseTableList.length;i++)");
            LoadResults.Content.AppendLine("   {");
            LoadResults.Content.AppendLine("      if (theSingleTable==UseTableList[i].TableName)");
            LoadResults.Content.AppendLine("      {");
            LoadResults.Content.AppendLine("         UseTableList[i].SetDataPacketAndChild(pk);");
            LoadResults.Content.AppendLine("         break;");
            LoadResults.Content.AppendLine("      }");
            LoadResults.Content.AppendLine("   }");
            LoadResults.Content.AppendLine("}");
            LoadResults.Content.AppendLine("if(iApplyCallID == event.result.id)");
            LoadResults.Content.AppendLine("{");
            LoadResults.Content.AppendLine("   for ( var i=0;i<UseTableList.length;i++)");
            LoadResults.Content.AppendLine("   {");
            LoadResults.Content.AppendLine("      if (UseTableList[i].fIDENTITYKey == true)");
            LoadResults.Content.AppendLine("      {");
            LoadResults.Content.AppendLine("         fPromptSubmit=false;");
            LoadResults.Content.AppendLine("         var rows=event.result.value.split(\"@\");");
            LoadResults.Content.AppendLine("         for(var m=0; m<rows.length; m++)");
            LoadResults.Content.AppendLine("         {");
            LoadResults.Content.AppendLine("            var fields=rows[m].split(\";\");");
            LoadResults.Content.AppendLine("            if (fields.length==1) break;");
            LoadResults.Content.AppendLine("            var bco=eval(fields[0]+\"Base\");");
            LoadResults.Content.AppendLine("            if (bco.LocateOID_GUID(fields[1])!=null)");
            LoadResults.Content.AppendLine("               bco.setFieldValueEx(bco.PrimaryKey,fields[2]);");
            LoadResults.Content.AppendLine("         }");
            LoadResults.Content.AppendLine("         break;");
            LoadResults.Content.AppendLine("      }");
            LoadResults.Content.AppendLine("   }");
            LoadResults.Content.AppendLine("   MergeChange();");
            LoadResults.Content.AppendLine("   if (fPromptSubmit==true)alert(event.result.value);");
            LoadResults.Content.AppendLine("   if (flowTask==true)");
            LoadResults.Content.AppendLine("   {");
            LoadResults.Content.AppendLine("      var hasRiskPoint = getURLParameters(\"hasRiskPoint\");");
            LoadResults.Content.AppendLine("      var openBPOName=getURLParameters(\"BPOName\")+\"BPO\";");
            LoadResults.Content.AppendLine("      try");
            LoadResults.Content.AppendLine("      {");
            LoadResults.Content.AppendLine("         var objTable;");
            LoadResults.Content.AppendLine("         if (hasRiskPoint && hasRiskPoint == 1) {");
            LoadResults.Content.AppendLine("            objTable = eval(\"window.parent.opener.opener.\"+openBPOName+\".getRootTable()\");");
            LoadResults.Content.AppendLine("            objTable.Refresh();");
            LoadResults.Content.AppendLine("            window.parent.opener.close();");
            LoadResults.Content.AppendLine("         }");
            LoadResults.Content.AppendLine("         else{");
            LoadResults.Content.AppendLine("            objTable = eval(\"window.parent.opener.\"+openBPOName+\".getRootTable()\");");
            LoadResults.Content.AppendLine("            objTable.Refresh();");
            LoadResults.Content.AppendLine("         }");
            LoadResults.Content.AppendLine("      }");
            LoadResults.Content.AppendLine("      catch(e){}");
            LoadResults.Content.AppendLine("      window.parent.close();");
            LoadResults.Content.AppendLine("   }");
            LoadResults.Content.AppendLine("   flowTask=false;");
            LoadResults.Content.AppendLine("   AfterSubmit();");
            LoadResults.Content.AppendLine("   return;");
            LoadResults.Content.AppendLine("}");
            LoadResults.Content.AppendLine("for ( var i=0;i<TaskList.length;i++)");
            LoadResults.Content.AppendLine("{");
            LoadResults.Content.AppendLine("   var obj = TaskList[i];");
            LoadResults.Content.AppendLine("   if(obj.CallID == event.result.id)");
            LoadResults.Content.AppendLine("   {");
            LoadResults.Content.AppendLine("      obj.DoResult(event.result);");
            LoadResults.Content.AppendLine("      return;");
            LoadResults.Content.AppendLine("   }");
            LoadResults.Content.AppendLine("}");
            BpoHtc.FuncList.Add(LoadResults);

            //getBusinessDataPacket
            JsFunction getBusinessDataPacket = new JsFunction("getBusinessDataPacket");
            getBusinessDataPacket.Content.AppendLine("return theDataPacket;");
            BpoHtc.FuncList.Add(getBusinessDataPacket);

            //MergeChange
            JsFunction MergeChange = new JsFunction("MergeChange");
            MergeChange.Content.AppendLine("while(true)");
            MergeChange.Content.AppendLine("{");
            MergeChange.Content.AppendLine("   if (theDeltaData.documentElement.childNodes.length<=0) break;");
            MergeChange.Content.AppendLine("   var node = theDeltaData.documentElement.childNodes[0];");
            MergeChange.Content.AppendLine("   theDeltaData.documentElement.removeChild(node);");
            MergeChange.Content.AppendLine("}");
            BpoHtc.FuncList.Add(MergeChange);

            //DeltaUpdate
            JsFunction DeltaUpdate = new JsFunction("DeltaUpdate");
            DeltaUpdate.Params.Add("recordNode");
            DeltaUpdate.Params.Add("FieldName");
            DeltaUpdate.Params.Add("value");
            DeltaUpdate.Params.Add("DataName");
            DeltaUpdate.Content.AppendLine("if (value==null) return;");
            DeltaUpdate.Content.AppendLine("var srcNode=null;");
            DeltaUpdate.Content.AppendLine("var changeNode=null;");
            DeltaUpdate.Content.AppendLine("var reocordTagName=DataName;");
            DeltaUpdate.Content.AppendLine("var seleNodes = theDeltaData.documentElement.selectNodes(\"//\"+reocordTagName);");
            DeltaUpdate.Content.AppendLine("for (var i=0;i<seleNodes.length;i++)");
            DeltaUpdate.Content.AppendLine("{");
            DeltaUpdate.Content.AppendLine("   var node = seleNodes[i];");
            DeltaUpdate.Content.AppendLine("   if (node.getAttribute(\"UpdateKind\")!=null)");
            DeltaUpdate.Content.AppendLine("   {");
            DeltaUpdate.Content.AppendLine("      srcNode = node;");
            DeltaUpdate.Content.AppendLine("      changeNode = seleNodes[i+1];");
            DeltaUpdate.Content.AppendLine("      break;");
            DeltaUpdate.Content.AppendLine("   }");
            DeltaUpdate.Content.AppendLine("}");
            DeltaUpdate.Content.AppendLine("if ( srcNode==null )");
            DeltaUpdate.Content.AppendLine("{");
            DeltaUpdate.Content.AppendLine("   var rNode = theDeltaData.createNode(1,reocordTagName,\"\");");
            DeltaUpdate.Content.AppendLine("   changeNode = theDeltaData.createNode(1,reocordTagName,\"\");");
            DeltaUpdate.Content.AppendLine("   rNode.setAttribute(\"UpdateKind\", \"ukModify\");");
            DeltaUpdate.Content.AppendLine("   for ( var i=0;i<recordNode.childNodes.length;i++)");
            DeltaUpdate.Content.AppendLine("   {");
            DeltaUpdate.Content.AppendLine("      var fnode=theDeltaData.createNode(1,recordNode.childNodes[i].nodeName,\"\");");
            DeltaUpdate.Content.AppendLine("      fnode.text=recordNode.childNodes[i].text;");
            DeltaUpdate.Content.AppendLine("      rNode.appendChild(fnode);");
            DeltaUpdate.Content.AppendLine("   }");
            DeltaUpdate.Content.AppendLine("   for ( var i=0;i<recordNode.childNodes.length;i++)");
            DeltaUpdate.Content.AppendLine("   {");
            DeltaUpdate.Content.AppendLine("      var fnode=theDeltaData.createNode(1,recordNode.childNodes[i].nodeName,\"\");");
            DeltaUpdate.Content.AppendLine("      if (fnode.nodeName==FieldName){");
            DeltaUpdate.Content.AppendLine("         fnode.text=value;");
            DeltaUpdate.Content.AppendLine("      }");
            DeltaUpdate.Content.AppendLine("      else{");
            DeltaUpdate.Content.AppendLine("         fnode.text=\"null\";");
            DeltaUpdate.Content.AppendLine("      }");
            DeltaUpdate.Content.AppendLine("      changeNode.appendChild(fnode);");
            DeltaUpdate.Content.AppendLine("   }");
            DeltaUpdate.Content.AppendLine("   theDeltaData.documentElement.appendChild(rNode);");
            DeltaUpdate.Content.AppendLine("   theDeltaData.documentElement.appendChild(changeNode);");
            DeltaUpdate.Content.AppendLine("}");
            DeltaUpdate.Content.AppendLine("else{");
            DeltaUpdate.Content.AppendLine("   for ( var i=0;i<changeNode.childNodes.length;i++)");
            DeltaUpdate.Content.AppendLine("   {");
            DeltaUpdate.Content.AppendLine("      var fnode=changeNode.childNodes[i];");
            DeltaUpdate.Content.AppendLine("      if (fnode.nodeName==FieldName){");
            DeltaUpdate.Content.AppendLine("         fnode.text=value;");
            DeltaUpdate.Content.AppendLine("      }");
            DeltaUpdate.Content.AppendLine("   }");
            DeltaUpdate.Content.AppendLine("}");

            BpoHtc.FuncList.Add(DeltaUpdate);

            //getResourceData
            JsFunction getResourceData = new JsFunction("getResourceData");
            getResourceData.Params.Add("keyName");
            getResourceData.Content.AppendLine("var Node = theDataPacket.selectSingleNode(\"//ResourceData[KeyName='\"+keyName+\"']\");");
            getResourceData.Content.AppendLine("if (Node!=null) return Node.childNodes[1].text;");
            getResourceData.Content.AppendLine("else return \"\";");
            BpoHtc.FuncList.Add(getResourceData);

            //CallbuildTreeSubNodes
            JsFunction CallbuildTreeSubNodes = new JsFunction("CallbuildTreeSubNodes");
            CallbuildTreeSubNodes.Params.Add("BCName");
            CallbuildTreeSubNodes.Params.Add("parentFieldName");
            CallbuildTreeSubNodes.Params.Add("parentOID");
            CallbuildTreeSubNodes.Params.Add("CaptionField");
            CallbuildTreeSubNodes.Params.Add("node");
            CallbuildTreeSubNodes.Params.Add("VCName");
            CallbuildTreeSubNodes.Params.Add("mode");
            CallbuildTreeSubNodes.Content.AppendLine("var srv=eval(\"ServiceHandle.\"+theBPOName+\"Service\");");
            CallbuildTreeSubNodes.Content.AppendLine("var headObj = new Object();");
            CallbuildTreeSubNodes.Content.AppendLine("callObj = ServiceHandle.createCallOptions();");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.async = false;");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.params = new Array();");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.params.BCName = BCName;");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.params.parentFieldName = parentFieldName;");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.params.parentOID = parentOID;");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.params.CaptionField = CaptionField;");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.funcName = \"buildTreeSubNodes\";");
            CallbuildTreeSubNodes.Content.AppendLine("if (mode==1){");
            CallbuildTreeSubNodes.Content.AppendLine("   callObj.funcName = \"buildTreeSubNodesWithChild\";");
            CallbuildTreeSubNodes.Content.AppendLine("   callObj.params.VCName = VCName;");
            CallbuildTreeSubNodes.Content.AppendLine("}");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.SOAPHeader = new Array();");
            CallbuildTreeSubNodes.Content.AppendLine("callObj.SOAPHeader[0] = headObj;");
            CallbuildTreeSubNodes.Content.AppendLine("var result =srv.callService(callObj);");
            CallbuildTreeSubNodes.Content.AppendLine("if(result.error==true){");
            CallbuildTreeSubNodes.Content.AppendLine("   var xfaultstring = result.errorDetail.string;");
            CallbuildTreeSubNodes.Content.AppendLine("   alert(xfaultstring);");
            CallbuildTreeSubNodes.Content.AppendLine("   return;");
            CallbuildTreeSubNodes.Content.AppendLine("}");
            CallbuildTreeSubNodes.Content.AppendLine("if(result.error==false){");
            CallbuildTreeSubNodes.Content.AppendLine("   var xmldoc=result.raw;");
            CallbuildTreeSubNodes.Content.AppendLine("   var s=result.value.split(\"~UCMLSPLIT~\");");
            CallbuildTreeSubNodes.Content.AppendLine("   if (s.length>1){");
            CallbuildTreeSubNodes.Content.AppendLine("      localxml.loadXML(s[1]);");
            CallbuildTreeSubNodes.Content.AppendLine("      for ( var i=0;i<UseTableList.length;i++)");
            CallbuildTreeSubNodes.Content.AppendLine("      {");
            CallbuildTreeSubNodes.Content.AppendLine("         UseTableList[i].AddDataRecords(localxml);");
            CallbuildTreeSubNodes.Content.AppendLine("      }");
            CallbuildTreeSubNodes.Content.AppendLine("   }");
            CallbuildTreeSubNodes.Content.AppendLine("   node.setAttribute(\"TreeNodeSrc\",s[0]);");
            CallbuildTreeSubNodes.Content.AppendLine("}");
            
            BpoHtc.FuncList.Add(CallbuildTreeSubNodes);

            //StartBCLink
            JsFunction StartBCLink = new JsFunction("StartBCLink");
            StartBCLink.Params.Add("BCLink");
            StartBCLink.Content.AppendLine("if (BCLink.BCRunMode!=4){");
            StartBCLink.Content.AppendLine("   if (BCLink.BCRunMode==5){");
            StartBCLink.Content.AppendLine("      eval(BCLink.urlFuncName+\"(BCLink)\");");
            StartBCLink.Content.AppendLine("      return;");
            StartBCLink.Content.AppendLine("   }");
            StartBCLink.Content.AppendLine("   var theMainPanel=window.document.forms[0];");
            StartBCLink.Content.AppendLine("   var bcObject= theMainPanel.parentElement.all[BCLink.BCName+'BPO'];");
            StartBCLink.Content.AppendLine("   for ( var i=0;i<UseTableList.length;i++)");
            StartBCLink.Content.AppendLine("   {");
            StartBCLink.Content.AppendLine("      UseTableList[i].NotifyControls(\"HideSelect\");");
            StartBCLink.Content.AppendLine("   }");
            StartBCLink.Content.AppendLine("   if (bcObject==null){");
            StartBCLink.Content.AppendLine("      var sHTML ='<div id=\"'+BCLink.BCName+'BPO\" style=\"Z-INDEX: 103;LEFT: 0px; BEHAVIOR: url('+UCMLLocalResourcePath+'BCClient/'+BCLink.BCName+'.htc); WIDTH: 100%; POSITION: absolute; TOP: 0px; HEIGHT: 200px\"></div>';");
            StartBCLink.Content.AppendLine("      theMainPanel.parentElement.insertAdjacentHTML(\"BeforeEnd\", sHTML);");
            StartBCLink.Content.AppendLine("      var bcObject= theMainPanel.parentElement.all[BCLink.BCName+'BPO'];");
            StartBCLink.Content.AppendLine("      theCurrentBCLink=BCLink;");
            StartBCLink.Content.AppendLine("      bcObject.moveable=true;");
            StartBCLink.Content.AppendLine("      bcObject.attachEvent(\"onreadystatechange\", OnBCReady);");
            StartBCLink.Content.AppendLine("   }");
            StartBCLink.Content.AppendLine("   else{");
            StartBCLink.Content.AppendLine("      bcObject.Visible=false;");
            StartBCLink.Content.AppendLine("      bcObject.BCLink=BCLink;");
            StartBCLink.Content.AppendLine("      bcObject.targetTable = eval(BCLink.targetTable + \"Base\");");
            StartBCLink.Content.AppendLine("      bcObject.srcFieldName = BCLink.srcFieldName;");
            StartBCLink.Content.AppendLine("      bcObject.destFieldName = BCLink.destFieldName;");
            StartBCLink.Content.AppendLine("      bcObject.Visible=true;");
            StartBCLink.Content.AppendLine("      bcObject.InitBusinessEnv();");
            StartBCLink.Content.AppendLine("   }");
            StartBCLink.Content.AppendLine("}");
            StartBCLink.Content.AppendLine("else{");
            StartBCLink.Content.AppendLine("   window.open(eval(BCLink.urlFuncName+\"(BCLink)\"),\"\",\"location=no,menubar=yes,toolbar=no,status=no,directories=no,scrollbars=yes,resizable=yes\");");
            StartBCLink.Content.AppendLine("}");

            BpoHtc.FuncList.Add(StartBCLink);

            //OnBCReady
            JsFunction OnBCReady = new JsFunction("OnBCReady");
            OnBCReady.Content.AppendLine("var theMainPanel=window.document.forms[0];");
            OnBCReady.Content.AppendLine("var bcObject= theMainPanel.parentElement.all[theCurrentBCLink.BCName+'BPO'];");
            OnBCReady.Content.AppendLine("bcObject.onclose = onpopupclose;");
            OnBCReady.Content.AppendLine("bcObject.targetTable = eval(theCurrentBCLink.targetTable+\"Base\");");
            OnBCReady.Content.AppendLine("bcObject.srcFieldName = theCurrentBCLink.srcFieldName;");
            OnBCReady.Content.AppendLine("bcObject.destFieldName = theCurrentBCLink.destFieldName;");
            OnBCReady.Content.AppendLine("bcObject.InitApplet(theCurrentBCLink);");

            BpoHtc.FuncList.Add(OnBCReady);

            //onpopupclose
            JsFunction onpopupclose = new JsFunction("onpopupclose");
            onpopupclose.Content.AppendLine("for ( var i=0;i<UseTableList.length;i++)");
            onpopupclose.Content.AppendLine("{");
            onpopupclose.Content.AppendLine("   UseTableList[i].NotifyControls(\"ShowSelect\");");
            onpopupclose.Content.AppendLine("}");
            BpoHtc.FuncList.Add(onpopupclose);



            //getURLParameters
            JsFunction getURLParameters = new JsFunction("getURLParameters");
            getURLParameters.Params.Add("ParamName");
            getURLParameters.Content.AppendLine("var sURL = window.document.URL.toString();");
            getURLParameters.Content.AppendLine("if (sURL.indexOf(\"?\") > 0){");
            getURLParameters.Content.AppendLine("   var arrParams = sURL.split(\"?\");");
            getURLParameters.Content.AppendLine("   var arrURLParams = arrParams[1].split(\" & \");");
            getURLParameters.Content.AppendLine("   var arrParamNames = new Array(arrURLParams.length);");
            getURLParameters.Content.AppendLine("   var arrParamValues = new Array(arrURLParams.length);");
            getURLParameters.Content.AppendLine("   var i = 0;");
            getURLParameters.Content.AppendLine("   for (i=0;i<arrURLParams.length;i++)");
            getURLParameters.Content.AppendLine("   {");
            getURLParameters.Content.AppendLine("      var sParam =  arrURLParams[i].split(\" = \");");
            getURLParameters.Content.AppendLine("      arrParamNames[i] = sParam[0];");
            getURLParameters.Content.AppendLine("      if (sParam[1] != \"\")arrParamValues[i] = unescape(sParam[1]);");
            getURLParameters.Content.AppendLine("      else arrParamValues[i] = \"No Value\";");
            getURLParameters.Content.AppendLine("   }");
            getURLParameters.Content.AppendLine("   for (i=0;i<arrURLParams.length;i++)");
            getURLParameters.Content.AppendLine("   {");
            getURLParameters.Content.AppendLine("      if (arrParamNames[i]==ParamName) return  arrParamValues[i];");
            getURLParameters.Content.AppendLine("   }");
            getURLParameters.Content.AppendLine("}");
            getURLParameters.Content.AppendLine("return null;");
            
            BpoHtc.FuncList.Add(getURLParameters);

            //createXMLObject
            JsFunction createXMLObject = new JsFunction("createXMLObject");
            createXMLObject.Content.AppendLine("try {");
            createXMLObject.Content.AppendLine("   var l_objActiveXObject = new ActiveXObject(\"Msxml2.DOMDocument\");");
            createXMLObject.Content.AppendLine("   return l_objActiveXObject;");
            createXMLObject.Content.AppendLine("}");
            createXMLObject.Content.AppendLine("catch(e){");
            createXMLObject.Content.AppendLine("   try{");
            createXMLObject.Content.AppendLine("      l_objActiveXObject = new ActiveXObject(\"Msxml.DOMDocument\");");
            createXMLObject.Content.AppendLine("      return l_objActiveXObject;");
            createXMLObject.Content.AppendLine("   }");
            createXMLObject.Content.AppendLine("   catch(e){");
            createXMLObject.Content.AppendLine("      try{");
            createXMLObject.Content.AppendLine("         l_objActiveXObject = new ActiveXObject(\"Microsoft.XMLDOM\");");
            createXMLObject.Content.AppendLine("         return l_objActiveXObject;");
            createXMLObject.Content.AppendLine("      }");
            createXMLObject.Content.AppendLine("      catch(e){");
            createXMLObject.Content.AppendLine("         errorHandler(e, \"createXMLObject\"); ");
            createXMLObject.Content.AppendLine("      }");
            createXMLObject.Content.AppendLine("   }");
            createXMLObject.Content.AppendLine("}");
            
            BpoHtc.FuncList.Add(createXMLObject);

            //ShowMessage
            JsFunction ShowMessage = new JsFunction("ShowMessage");
            ShowMessage.Params.Add("str");
            ShowMessage.Content.AppendLine("MsgPanel.style.visibility=\"visible\";");
            ShowMessage.Content.AppendLine("MsgPanel.innerHTML=str;");
            BpoHtc.FuncList.Add(ShowMessage);

            //HideMessage
            JsFunction HideMessage = new JsFunction("HideMessage");
            HideMessage.Content.AppendLine("MsgPanel.style.visibility=\"hidden\";");
            BpoHtc.FuncList.Add(HideMessage);

            //自定义函数
            foreach (JsFunction func in BpoPropSet.JsFuncs)
            {
                BpoHtc.FuncList.Add(func);
            }

            //装备完毕

            return true;
        }

        /// <summary>
        /// 装配BPOName.asmx页面
        /// </summary>
        /// <returns></returns>
        public bool BuildAsmxPage()
        {
            AspxDirective directive = new AspxDirective("WebService");
            directive["Language"] = "c#";
            if (this.CompileMode) directive["CodeBehind"] = this.Name + "asmx.cs";
            else directive["CodeFile"] = this.Name+"asmx.cs";
            directive["Class"] = Namespace+"."+this.Name+"Service";
            this.AsmxPage.Directives.Add(directive);

            return true;
        }

        /// <summary>
        /// 保存Aspx页面文件
        /// </summary>
        /// <returns></returns>
        public bool SaveAspxPage()
        {
            return this.Page.Save(this.SavePath);
        }
        public bool SaveAspxCs()
        {
            return this.PageCs.Save(this.SavePath);
        }
        public bool SavePageDesignCs()
        {
            return this.PageDesignerCs.Save(this.SavePath);
        }
        public bool SaveAsmxCs()
        {
            string path = null;
            if (this.CompileMode)
            {
                path = this.SavePath;
                return this.AsmxCs.Save(path);
            }
            else
            {
                path = this.SavePath + "\\DCompile";
                return this.AsmxCs.SaveAscii(path);
            }
            
        }

        public bool SaveAsmxPage()
        {
            return this.AsmxPage.Save(this.SavePath);
        }
        public bool SaveHtc()
        {
            return this.BpoHtc.Save(this.SavePath);
        }

        public void SetVCPostion()
        {
            foreach(UcmlVcTabPage vcTab in this.VcTabList)
            {
                foreach (UcmlViewCompnent vc in vcTab.VCList)
                {
                    foreach (UcmlVcColumn vcCol in vc.Columns)
                    {
                        foreach (UcmlBusiCompPropSet bc in this.BCList)
                        {
                            if (bc.Name == vc.BCName)
                            {
                                vcCol.CurrentPos = -1;
                                int pos = 0;
                                for (int i = 0; i < bc.Columns.Count;i++ )
                                {
                                    if (bc.Columns[i].fDisplay)
                                    {
                                        if (vcCol.FieldName == bc.Columns[i].FieldName)
                                        {
                                            vcCol.CurrentPos = pos;
                                            break;
                                        }
                                        else pos++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string GetUcmlTypeName(int type)
        {
            string sqlType = "";
            switch (type)
            {
                case 0: sqlType = "Char"; break;
                case 2: sqlType = "VarChar"; break;
                case 4: sqlType = "Numberic"; break;
                case 6: sqlType = "Text"; break;
                case 8: sqlType = "Date"; break;
                case 10: sqlType = "Int"; break;
                case 11: sqlType = "Short"; break;
                case 12: sqlType = "Byte"; break;
                case 13: sqlType = "Float"; break;
                case 14: sqlType = "Double"; break;
                case 15: sqlType = "Money"; break;
                case 17: sqlType = "Blob"; break;
                case 20: sqlType = "Boolean"; break;
                case 31: sqlType = "Long"; break;
                case 32: sqlType = "Time"; break;
                case 33: sqlType = "DateTime"; break;
                case 40: sqlType = "Image"; break;
                case 41: sqlType = "WORD"; break;
                case 42: sqlType = "EXCEL"; break;
                case 43: sqlType = "HTML"; break;
                case 45: sqlType = "Guid"; break;
                case 46: sqlType = "UCMLKey"; break;
            }
            return sqlType;
        }

    }
}
