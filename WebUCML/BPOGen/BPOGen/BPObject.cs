using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlBPO
    {
        private string _Name;
        private AspxPage Page;
        private CSharpDoc _PageCs;
        private CSharpDoc PageDesignerCs;
        private BpoPropertySet BpoPropSet;
        private List<UcmlVcTabPage> VcTabList;
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
            Page = new AspxPage(bps.Name + ".aspx");
            PageCs = new CSharpDoc(bps.Name+".aspx.cs", Namespace);
            PageDesignerCs = new CSharpDoc(bps.Name + ".designer.cs", Namespace);

            VcTabList = new List<UcmlVcTabPage>();
            BCList = new List<UcmlBusiCompPropSet>();
        }

        public void AddVcTab(UcmlVcTabPage vcTab)
        { 
        }
        public bool BuildAspxPage()
        { 
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
            //添加BCnameBase
            initCtr.Content.AppendLine("");
            initCtr.Content.AppendLine("");
            initCtr.Content.AppendLine("");
            initCtr.Content.AppendLine("");
            initCtr.Content.AppendLine("");
            initCtr.Content.AppendLine("");
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
    }
}
