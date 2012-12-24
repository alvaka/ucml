using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace UCML.IDE.WebUCML
{
    //准备装配BPO所需的数据
    public class BPOPrepare
    {
        public static BpoPropertySet GetBPS(SqlConnection conn, string guid)
        {
            BpoPropertySet bps = new BpoPropertySet();
            bps.GUID = guid;
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("ClassName,ChineseName,isInFlow,fSysUseBPO,fHavePageNavi,fRegisterBPO,fMutiLangugeSupport,fXHTMLForm,");
            sql.Append("EnableConfig,fUseSkin, SkinSrc,");
            sql.Append("JSCIPTInit,JSCIPTBeforeUpdate,JSCIPTAfterApplyScript,");
            sql.Append("InitScript,BeforeApplyScript,AfterApplyScript,UsesText");
            sql.Append(" from UCMLClassDataSet ");
            sql.Append("where UCMLClassDataSetOID='" + guid+"'");
            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                bps.GUID = guid;
                bps.Name = Util.GetPropString(reader, 0);
                bps.Capiton = Util.GetPropString(reader, 1);
                bps.fInFlow = Util.GetPropBool(reader, 2);
                bps.fSystemBPO = Util.GetPropBool(reader, 3);
                bps.fHavePageNavi = Util.GetPropBool(reader, 4);
                bps.fRegisterBPO = Util.GetPropBool(reader, 5);
                bps.fMutiLangugeSupport = Util.GetPropBool(reader, 6);
                bps.fXHTMLForm = Util.GetPropBool(reader, 7);
                bps.fEnableConfig = Util.GetPropBool(reader, 8);
                bps.fUseSkin = Util.GetPropBool(reader, 9);
                bps.SkinSrc = Util.GetPropString(reader, 10);
                bps.InitScript = Util.GetPropString(reader, 11);
                bps.BeforeSubmitScript = Util.GetPropString(reader, 12);
                bps.AfterSubmitScript = Util.GetPropString(reader, 13);
                bps.InitCSharpCode = Util.GetPropString(reader, 14);
                bps.BeforeSubmitCSharpCode = Util.GetPropString(reader, 15);
                bps.AfterSubmitCSharpCode = Util.GetPropString(reader, 16);
                bps.RefCSharpLibrary = Util.GetPropString(reader, 17);
            }

            reader.Close();
            reader = null;
            bps.CSharpFuncs = GetBpoFunction(conn, bps.GUID);
            bps.JsFuncs = GetBpoScript(conn, bps.GUID);
            cmd = null;
            return bps;
        }

        public static List<CSharpFunction> GetBpoFunction(SqlConnection conn, string bpoid)
        {
            List<CSharpFunction> CSharpFuncs = new List<CSharpFunction>();
            StringBuilder sql = new StringBuilder("select SourceCode,SupportWebService ");
            sql.Append("from MethodInfoDataSet ");
            sql.Append("where UCMLClassOID='"+bpoid+"' and OperationType=1");
            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader=cmd.ExecuteReader();
            while (reader.Read())
            {
                CSharpFunction func = new CSharpFunction();
                func.RawContent = Util.GetPropString(reader, 0);
                func.IsWebMethod = Util.GetPropBool(reader, 1);
                func.EnableSession = true;

                CSharpFuncs.Add(func);
            }
            reader.Close();
            return CSharpFuncs;
        }

        public static List<JsFunction> GetBpoScript(SqlConnection conn, string bpoid)
        {
            List<JsFunction> JsFuncs = new List<JsFunction>();
            StringBuilder sql = new StringBuilder("select SourceCode,MethodName ");
            sql.Append("from MethodInfoDataSet ");
            sql.Append("where UCMLClassOID='" + bpoid + "' and OperationType=20");
            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                JsFunction func = new JsFunction();
                func.RawContent = Util.GetPropString(reader, 0);
                func.Name = Util.GetPropString(reader, 1);
                JsFuncs.Add(func);
            }
            reader.Close();
            return JsFuncs;
        }

        public static List<UcmlVcTabPage> GetVcTab(SqlConnection conn, string bpoid)
        {
            List<UcmlVcTabPage> tabList = new List<UcmlVcTabPage>();
            List<UcmlViewCompnent> vcList = new List<UcmlViewCompnent>();

            //构造SQL函数，获取BPO下的所有VC
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("a.BusiViewCompLinkDataSetOID,a.ParentOID,b.AppletName,b.Caption,b.fTreeGridMode,b.fSubTableTreeMode,b.ImageLink,");
            sql.Append("b.SubBCs,b.SubParentFields,b.SubPicFields,b.SubLabelFields,b.SubFKFields,b.TargetHTMLSource ,b.AllowEdit,a.fHidden,");
            sql.Append("b.UserDesignWebPage,a.alignHeight,a.alignWidth,c.BCName,b.AppletDataSetOID,b.AllowAddNew,b.AppletKind,");
            sql.Append("b.JSCIPTInit,b.JSCIPTBeforeUpdate,b.JSCIPTAfterApplyScript,b.HttpGetScript,b.HttpPostScript,b.PageLoadScript");
            sql.Append(" from BusiViewCompLinkDataSet as a,AppletDataSet as b,BusinessTableDataSet as c ");
            sql.Append("where a.AppletOID=b.AppletDataSetOID and c.BusinessTableDataSetOID=b.BusinessTableOID and a.UCMLClassOID='" + bpoid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            //读取VC基本信息
            while (reader.Read())
            {
                UcmlViewCompnent vc = new UcmlViewCompnent();
                vc.LinkOID = Util.GetProperGuid(reader, 0);
                vc.LinkPOID = Util.GetProperGuid(reader, 1);
                vc.VCName = Util.GetPropString(reader, 2);
                vc.Caption = Util.GetPropString(reader, 3);
                vc.fTreeGridMode = Util.GetPropBool(reader, 4);
                vc.fSubTableTreeMode = Util.GetPropBool(reader, 5);
                vc.ImageLink = Util.GetPropString(reader, 6);
                vc.SubBCs = Util.GetPropString(reader, 7);
                vc.SubParentFields = Util.GetPropString(reader, 8);
                vc.SubPicFields = Util.GetPropString(reader, 9);
                vc.SubLabelFields = Util.GetPropString(reader, 10);
                vc.SubFKFields = Util.GetPropString(reader, 11);
                vc.SetVCNode(Util.GetPropString(reader, 12), "div");
                vc.EnabledEdit = Util.GetPropBool(reader, 13);
                vc.fHidden = Util.GetPropBool(reader, 14);
                vc.UserDefineHTML = Util.GetPropBool(reader, 15);
                vc.alignHeight = Util.GetPropBool(reader, 16);
                vc.alignWidth = Util.GetPropBool(reader, 17);
                vc.BCName = Util.GetPropString(reader, 18);
                vc.OID = Util.GetProperGuid(reader, 19);
                vc.haveMenu = Util.GetPropBool(reader, 20);
                vc.Kind = Util.GetProperInt(reader, 21);
                vc.InitScript = Util.GetPropString(reader, 22);
                vc.BeforeUpdateScript = Util.GetPropString(reader, 23);
                vc.AfterApplyScript = Util.GetPropString(reader, 24);
                vc.HttpGetCSharpCode = Util.GetPropString(reader, 25);
                vc.HttpPostCSharpCode = Util.GetPropString(reader, 26);
                vc.PageLoadCSharpCode = Util.GetPropString(reader, 27);

                vcList.Add(vc);
            }

            //关闭SqlDataReader
            reader.Close();

            foreach (UcmlViewCompnent vc in vcList)
            {
                //加载列信息
                vc.Columns = GetVcColumn(conn, vc.OID);
                vc.Buttons = GetVcButton(conn, vc.OID);
            }

            //将VC按照POID分类在不同的VCTab中
            List<string> OIDList = new List<string>();
            string tmpOID = "";
            for (int i = 0; i < vcList.Count; i++)
            {
                if (OIDList.Contains(vcList[i].LinkPOID)) continue;
                tmpOID = vcList[i].LinkPOID;
                UcmlVcTabPage vcTab = new UcmlVcTabPage();
                vcTab.Name = vcList[i].VCName;
                vcTab.Caption = vcList[i].Caption;
                vcTab.ParentOID = vcList[i].LinkPOID;
                vcTab.VCList.Add(vcList[i]);

                for (int j = i + 1; j < vcList.Count; j++)
                {
                    if (vcList[j].LinkPOID == tmpOID)
                    {
                        vcTab.VCList.Add(vcList[j]);
                        vcList.Remove(vcList[j]);
                    }
                }
                OIDList.Add(tmpOID);
                tabList.Add(vcTab);
            }

            return tabList;
        }

        public static List<UcmlVcColumn> GetVcColumn(SqlConnection conn, string vcOid)
        {
            List<UcmlVcColumn> columns = new List<UcmlVcColumn>();
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("FieldName,ChineseName,AllowEdit,Visible,Width,fFixColumn,FixColumnValue,fCustomerControl,CustomerControlHTC,ControlID,");
            sql.Append("EditContrl,LeftBracket,RightBracket,LogicConnect,CondiFieldValue,fIsFunctionValue,OperationIndent,InnerLinkComp ");
            sql.Append("from AppletColumnDataSet ");
            sql.Append("where AppletOID='" + vcOid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                UcmlVcColumn column = new UcmlVcColumn();
                column.FieldName = Util.GetPropString(reader, 0);
                column.Caption = Util.GetPropString(reader, 1);
                column.fCanModify = Util.GetPropBool(reader, 2);
                column.fDisplay = Util.GetPropBool(reader, 3);
                column.Width = Util.GetProperInt(reader, 4);
                column.fFixColumn = Util.GetPropBool(reader, 5);
                column.FixColumnValue = Util.GetPropString(reader, 6);
                column.fCustomerControl = Util.GetPropBool(reader, 7);
                column.CustomerControlHTC = Util.GetPropString(reader, 8);
                column.ControlID = Util.GetPropString(reader, 9);
                column.EditContrl = Util.GetProperInt(reader, 10).ToString();
                column.LeftBracket = Util.GetPropString(reader, 11);
                column.RightBracket = Util.GetPropString(reader, 12);
                column.Logic = Util.GetProperInt(reader, 13);
                column.CondiFieldValue = Util.GetPropString(reader, 14);
                column.fIsFunctionValue = Util.GetPropBool(reader, 15);
                column.Operation = Util.GetProperInt(reader, 16);
                column.InnerLinkComp = Util.GetPropBool(reader, 17);

                columns.Add(column);
            }
            reader.Close();

            return columns;
        }

        public static List<UcmlVcButton> GetVcButton(SqlConnection conn, string vcOid)
        {
            List<UcmlVcButton> buttons = new List<UcmlVcButton>();

            StringBuilder sql = new StringBuilder("select ");
            sql.Append("Caption,IamgeLink,Width,ToolTip,OnClickSourceCode,ButtonType,fOnlyPicture ");
            sql.Append("from BusinessButtonDataSet ");
            sql.Append("where AppletOID='" + vcOid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                UcmlVcButton button = new UcmlVcButton();
                button.Caption = Util.GetPropString(reader, 0);
                button.ImgeLink = Util.GetPropString(reader, 1);
                button.Width = Util.GetProperInt(reader, 2);
                button.ToolTip = Util.GetPropString(reader, 3);
                button.OnClickScript = Util.GetPropString(reader, 4);
                button.Type = Util.GetProperInt(reader, 5);
                button.fOnlyPicture = Util.GetPropBool(reader, 6);

                buttons.Add(button);
            }
            reader.Close();
            return buttons;
        }
        
        public static List<UcmlBusiCompPropSet> GetBC(SqlConnection conn, string bpoid)
        {
            List<UcmlBusiCompPropSet> bcList = new List<UcmlBusiCompPropSet>();
            //构造sql
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("b.BCName,b.ChineseName,a.RootTable,b.DataMember,a.AllowModifyJION,a.LinkKeyName,a.PK_COLUMN_NAME,c.fCustomKey,a.BusinessTableOID,");
            sql.Append("a.fIsActor,a.BusiCompLinkDataSetOID,a.ParentOID,a.LinkKeyType,a.ReadMaxCount,");
            sql.Append("b.OnCalculateEx,b.OnRecordChangeEx,b.OnBeforeInsertEx,b.OnAfterInsertEx,");
            sql.Append("b.JSCIPTInit,b.JSCIPTBeforeUpdate,b.JSCIPTAfterApplyScript");
            sql.Append(" from BusiCompLinkDataSet as a,BusinessTableDataSet as b ,UCMLClassDataSet as c ");
            sql.Append("where a.BusinessTableOID=b.BusinessTableDataSetOID and b.DataMember=c.ClassName  and a.UCMLClassOID='" + bpoid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                UcmlBusiCompPropSet bc = new UcmlBusiCompPropSet();
                bc.Name = Util.GetPropString(reader, 0);
                bc.Caption = Util.GetPropString(reader, 1);
                bc.IsRootBC = Util.GetPropBool(reader, 2);
                bc.TableName = Util.GetPropString(reader, 3);
                bc.fIDENTITYKey = false;//没找到该键在数据库的定义
                bc.AllowModifyJION = Util.GetPropBool(reader, 4);
                bc.LinkKeyName = Util.GetPropString(reader, 5);
                bc.PK_COLUMN_NAME = Util.GetPropString(reader, 6);
                bc.fHaveUCMLKey = !Util.GetPropBool(reader, 7);
                bc.OID = Util.GetProperGuid(reader, 8);
                bc.IsActor = Util.GetPropBool(reader, 9);
                bc.LinkOID = Util.GetProperGuid(reader, 10);
                bc.LinkPOID = Util.GetProperGuid(reader, 11);
                bc.LinkKeyType = Util.GetProperInt(reader, 12);
                bc.PageCount = Util.GetProperInt(reader, 13);
                bc.OnCalculateScript = Util.GetPropString(reader, 14);
                bc.OnRecordChangeScript = Util.GetPropString(reader, 15);
                bc.OnBeforeInsertScript = Util.GetPropString(reader, 16);
                bc.OnAfterInsertScript = Util.GetPropString(reader, 17);
                bc.InitScript = Util.GetPropString(reader, 18);
                bc.BeforeUpdateScript = Util.GetPropString(reader, 19);
                bc.AfterUpdateScript = Util.GetPropString(reader, 20);

                if (bc.fHaveUCMLKey) bc.PrimaryKey = bc.TableName + "OID";

                bcList.Add(bc);
            }
            reader.Close();

            foreach (UcmlBusiCompPropSet bc in bcList)
            {
                //获取列信息
                bc.Columns = GetBcColumn(conn, bc.OID);

                //获取where条件列信息
                bc.CondiColumns = GetCondiColumn(conn, bc.OID);

                //通过BC列信息设置CondiColumn中的FieldType,Pos属性
                foreach (BCCondiColumn condi in bc.CondiColumns)
                {
                    for (int i = 0; i < bc.Columns.Count;i++ )
                    {
                        if (condi.FieldName == bc.Columns[i].FieldName)
                        {
                            condi.FieldType = bc.Columns[i].FieldType;
                            condi.Pos = i;
                        }
                    }
                }

                //获取子BC
                foreach (UcmlBusiCompPropSet subBc in bcList)
                {
                    if (subBc.LinkPOID == bc.LinkOID) bc.ChildBC.Add(subBc);
                }
            }

            return bcList;
        }

        public static List<BusiCompColumn> GetBcColumn(SqlConnection conn, string bcOid)
        {
            List<BusiCompColumn> columns = new List<BusiCompColumn>();
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("FieldName,ChineseName,AllowEdit,Visible,");
            sql.Append("FieldLength,DecLength,FieldType,Width,SortMode,fUseCodeTable,CodeTable,fAllowNull,DefaultValue,");
            sql.Append("LookupField,LookupDataSet,fForeignKey,FieldKindEx,CustomSQLColumn,ExcelColNo,fFunctionInitValue,InitValueFunc,");
            sql.Append("OnFieldChangeEx,BusinessColumnDataSetOID");
            sql.Append(" from BusinessColumnDataSet ");
            sql.Append("where BusinessTableOID='" + bcOid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                BusiCompColumn column = new BusiCompColumn();
                column.FieldName = Util.GetPropString(reader, 0);
                column.Caption = Util.GetPropString(reader, 1);
                column.fCanModify = Util.GetPropBool(reader, 2);
                column.fDisplay = Util.GetPropBool(reader, 3);
                column.FieldLength = Util.GetProperInt(reader, 4);
                column.DecLength = Util.GetProperInt(reader, 5);
                column.FieldType = Util.GetProperInt(reader, 6);
                column.Width = Util.GetProperInt(reader, 7);
                column.SortMode = Util.GetProperInt(reader, 8);
                column.fUseCodeTable = Util.GetPropBool(reader, 9);
                column.CodeTable = Util.GetPropString(reader, 10);
                column.fAllowNull = Util.GetPropBool(reader, 11);
                column.DefaultValue = Util.GetPropString(reader, 12);
                column.LookupKeyField = Util.GetPropString(reader, 13);
                column.LookupDataSet = Util.GetPropString(reader, 14);
                column.fForeignKey = Util.GetPropBool(reader, 15);
                column.FieldKind = Util.GetProperInt(reader, 16);
                column.CustomSQLColumn = Util.GetPropString(reader, 17);
                column.ExcelColNo = Util.GetProperInt(reader, 18);
                column.fFunctionInitValue = Util.GetPropBool(reader, 19);
                column.InitValueFunc = Util.GetPropString(reader, 20);
                column.OnFieldChangeScript = Util.GetPropString(reader, 21);
                column.OID=Util.GetProperGuid(reader,22);

                columns.Add(column);
            }
            reader.Close();

            foreach (BusiCompColumn column in columns)
            {
                column.BCLink = GetBCLink(conn, column.OID);
            }
            return columns;
        }

        public static BCLinkProperty GetBCLink(SqlConnection conn,string colOID)
        {
            BCLinkProperty bcLink = new BCLinkProperty();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("AppletName,Caption,BCRunMode,srcFieldName,destFieldName,CondiFieldName,Value,");
            sql.Append("QueryFieldName,fQuickQuery,fDropDownMode,DropDownWidth,DropDownHeight ");
            sql.Append("from BCLinkDataSet where BusinessColumnOID='"+colOID+"'");

            cmd.CommandText = sql.ToString();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                bcLink.AppletName = Util.GetPropString(reader, 0);
                bcLink.Caption = Util.GetPropString(reader, 1);
                bcLink.RunMode = Util.GetProperInt(reader, 2);
                bcLink.srcFieldName = Util.GetPropString(reader, 3);
                bcLink.destFieldName = Util.GetPropString(reader, 4);
                bcLink.condiFieldName = Util.GetPropString(reader, 5);
                bcLink.Value = Util.GetPropString(reader, 6);
                bcLink.QueryFieldName = Util.GetPropString(reader, 7);
                bcLink.fQuickQuery = Util.GetPropBool(reader, 8);
                bcLink.fDropDownMode = Util.GetPropBool(reader, 9);
                bcLink.DropDownWidth = Util.GetProperInt(reader, 10);
                bcLink.DropDownHeight = Util.GetProperInt(reader, 11);
            }
            reader.Close();
            return bcLink;
        }

        public static List<BCCondiColumn> GetCondiColumn(SqlConnection conn, string bcOid)
        {
            List<BCCondiColumn> columns = new List<BCCondiColumn>();
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("FieldName,LeftBracket,RightBracket,OperationIndent,CondiFieldValue,");
            sql.Append("fIsFunctionValue,valueFunction,logicConnect,fFreeWhere,SQL");
            sql.Append(" from BsuiCompCondiDataSet ");
            sql.Append("where BusinessTableOID='" + bcOid + "'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                BCCondiColumn column = new BCCondiColumn();

                column.FieldName = Util.GetPropString(reader, 0);
                column.LeftBracket = Util.GetPropString(reader, 1);
                column.RightBracket = Util.GetPropString(reader, 2);
                column.Operation = Util.GetProperInt(reader, 3);
                column.CondiFieldValue = Util.GetPropString(reader, 4);
                column.fCondiField = true;
                column.fIsFunctionValue = Util.GetPropBool(reader, 5);
                column.valueFunction = Util.GetPropString(reader, 6);
                column.Logic = Util.GetProperInt(reader, 7);
                column.fFreeWhere = Util.GetPropBool(reader, 8);
                column.SQL = Util.GetPropString(reader, 9);

                columns.Add(column);
            }
            reader.Close();
            return columns;
        }

        public static UcmlViewCompnent GetVC(SqlConnection conn, string vcOid)
        {
            UcmlViewCompnent vc = new UcmlViewCompnent();
            vc.OID = vcOid;
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("a.AppletName,a.Caption,a.fTreeGridMode,a.fSubTableTreeMode,a.ImageLink,a.SubBCs,a.SubParentFields,a.SubPicFields,");
            sql.Append("a.SubLabelFields,a.SubFKFields,a.TargetHTMLSource ,a.AllowEdit,a.UserDesignWebPage,b.BCName,a.AllowAddNew,a.AppletKind,");
            sql.Append("a.JSCIPTInit,a.JSCIPTBeforeUpdate,a.JSCIPTAfterApplyScript,a.HttpGetScript,a.HttpPostScript,a.PageLoadScript,a.AppletName ");
            sql.Append("from AppletDataSet as a,BusinessTableDataSet as b ");
            sql.Append("where b.BusinessTableDataSetOID=a.BusinessTableOID and a.AppletDataSetOID='" + vcOid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            //读取VC基本信息
            if (reader.Read())
            {
                vc.VCName = Util.GetPropString(reader, 0);
                vc.Caption = Util.GetPropString(reader, 1);
                vc.fTreeGridMode = Util.GetPropBool(reader, 2);
                vc.fSubTableTreeMode = Util.GetPropBool(reader, 3);
                vc.ImageLink = Util.GetPropString(reader, 4);
                vc.SubBCs = Util.GetPropString(reader, 5);
                vc.SubParentFields = Util.GetPropString(reader, 6);
                vc.SubPicFields = Util.GetPropString(reader, 7);
                vc.SubLabelFields = Util.GetPropString(reader, 8);
                vc.SubFKFields = Util.GetPropString(reader, 9);
                vc.SetVCNode(Util.GetPropString(reader, 10), "div");
                vc.EnabledEdit = Util.GetPropBool(reader, 11);
                vc.UserDefineHTML = Util.GetPropBool(reader, 12);
                vc.BCName = Util.GetPropString(reader, 13);
                vc.haveMenu = Util.GetPropBool(reader, 14);
                vc.Kind = Util.GetProperInt(reader, 15);
                vc.InitScript = Util.GetPropString(reader, 16);
                vc.BeforeUpdateScript = Util.GetPropString(reader, 17);
                vc.AfterApplyScript = Util.GetPropString(reader, 18);
                vc.HttpGetCSharpCode = Util.GetPropString(reader, 19);
                vc.HttpPostCSharpCode = Util.GetPropString(reader, 20);
                vc.PageLoadCSharpCode = Util.GetPropString(reader, 21);
                vc.VCName = Util.GetPropString(reader, 22);
            }
            reader.Close();
            vc.Columns = GetVcColumn(conn, vc.OID);
            vc.Buttons = GetVcButton(conn, vc.OID);
            return vc;
        }

        public static UcmlBusiCompPropSet GetBC(SqlConnection conn, string bcOid, bool flag)
        {
            UcmlBusiCompPropSet bc = new UcmlBusiCompPropSet();
            bc.OID = bcOid;
            StringBuilder sql = new StringBuilder("select a.BCName,a.ChineseName,a.DataMember,b.fCustomKey,");
            sql.Append("a.OnCalculateEx,a.OnRecordChangeEx,a.OnBeforeInsertEx,a.OnAfterInsertEx,");
            sql.Append("a.JSCIPTInit,a.JSCIPTBeforeUpdate,a.JSCIPTAfterApplyScript,b.fAutoGenKey ");
            sql.Append("from BusinessTableDataSet as a ,UCMLClassDataSet as b ");
            sql.Append("where a.DataMember=b.ClassName  and a.BusinessTableDataSetOID='" + bcOid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                bc.Name = Util.GetPropString(reader, 0);
                bc.Caption = Util.GetPropString(reader, 1);
                bc.TableName = Util.GetPropString(reader, 2);
                bc.fHaveUCMLKey = !Util.GetPropBool(reader, 3);
                bc.OnCalculateScript = Util.GetPropString(reader, 4);
                bc.OnRecordChangeScript = Util.GetPropString(reader, 5);
                bc.OnBeforeInsertScript = Util.GetPropString(reader, 6);
                bc.OnAfterInsertScript = Util.GetPropString(reader, 7);
                bc.InitScript = Util.GetPropString(reader, 8);
                bc.BeforeUpdateScript = Util.GetPropString(reader, 9);
                bc.AfterUpdateScript = Util.GetPropString(reader, 10);
                bc.fIDENTITYKey = Util.GetPropBool(reader, 11);
                if (bc.fHaveUCMLKey) bc.PrimaryKey = bc.TableName + "OID";
            }
            reader.Close();

            bc.Columns = GetBcColumn(conn, bc.OID);
            return bc;
        }
    }
}
