using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace UCML.IDE.WebUCML
{
    class BPOPrepare
    {
        public static BpoPropertySet GetBPS(SqlConnection conn, string guid)
        {
            BpoPropertySet bps = new BpoPropertySet();
            bps.GUID = guid;
            StringBuilder sql = new StringBuilder("select ");
            sql.Append("ClassName,ChineseName,isInFlow,fSysUseBPO,fHavePageNavi,fRegisterBPO,fMutiLangugeSupport,fXHTMLForm,");
            sql.Append("EnableConfig,fUseSkin, SkinSrc,");
            sql.Append("JSCIPTInit,JSCIPTBeforeUpdate,JSCIPTAfterApplyScript,");
            sql.Append("InitScript,BeforeApplyScript,AfterApplyScript");
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
            }

            reader.Close();
            reader = null;
            cmd = null;
            return bps;
        }

        public static List<UcmlVcTabPage> GetVcTab(SqlConnection conn, string bpoid)
        {
            List<UcmlVcTabPage> tabList = new List<UcmlVcTabPage>();
            List<UcmlViewCompnent> vcList = new List<UcmlViewCompnent>();

            //构造SQL函数，获取BPO下的所有VC
            StringBuilder sql = new StringBuilder("select a.BusiViewCompLinkDataSetOID,a.ParentOID,b.AppletName,b.Caption,b.fTreeGridMode,b.fSubTableTreeMode,b.ImageLink,b.SubBCs,b.SubParentFields,b.SubPicFields,b.SubLabelFields,b.SubFKFields,b.TargetHTMLSource ,b.AllowEdit,a.fHidden,b.UserDesignWebPage,a.alignHeight,a.alignWidth,c.BCName,b.AppletDataSetOID,b.AllowAddNew");
            sql.Append(",b.AppletKind ");
            sql.Append("from BusiViewCompLinkDataSet as a,AppletDataSet as b,BusinessTableDataSet as c ");
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
                vcList.Add(vc);
            }
            //关闭SqlDataReader
            reader.Close();

            foreach (UcmlViewCompnent vc in vcList)
            {
                //加载列信息
                vc.Columns = GetVcColumn(conn, vc.OID);
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
            sql.Append("FieldName,ChineseName,AllowEdit,Visible,Width,fFixColumn,FixColumnValue,fCustomerControl,CustomerControlHTC,ControlID,EditContrl ");
            sql.Append("from AppletColumnDataSet ");
            sql.Append("where AppletOID='" + vcOid+"'");

            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();
            int i = 0;
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
                column.CurrentPos = i;
                i++;
                columns.Add(column);
            }
            reader.Close();

            return columns;
        }

        public static List<UcmlBusiCompPropSet> GetBC(SqlConnection conn, string bpoid)
        {
            List<UcmlBusiCompPropSet> bcList = new List<UcmlBusiCompPropSet>();
            //构造sql
            StringBuilder sql = new StringBuilder("select b.BCName,b.ChineseName,a.RootTable,b.DataMember,a.AllowModifyJION,a.LinkKeyName,a.PK_COLUMN_NAME,c.fCustomKey,a.BusinessTableOID");
            sql.Append(",a.fIsActor,a.BusiCompLinkDataSetOID,a.ParentOID,a.LinkKeyType,a.ReadMaxCount");
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

                if (bc.fHaveUCMLKey) bc.PrimaryKey = bc.TableName + "OID";

                bcList.Add(bc);
            }
            reader.Close();

            foreach (UcmlBusiCompPropSet bc in bcList)
            {
                //获取列信息
                bc.Columns = GetBcColumn(conn, bc.OID);

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
            sql.Append("LookupField,LookupDataSet,fForeignKey,FieldKindEx,CustomSQLColumn,ExcelColNo,fFunctionInitValue,InitValueFunc ");
            sql.Append("from BusinessColumnDataSet ");
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

                columns.Add(column);
            }
            reader.Close();

            return columns;
        }
    }
}
