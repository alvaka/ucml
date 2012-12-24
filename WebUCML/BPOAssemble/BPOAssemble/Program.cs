using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace UCML.IDE.WebUCML
{
    class Program
    {
        static void AssembleBPO(SqlConnection conn,string bpoOID)
        {
            BpoPropertySet bps = BPOPrepare.GetBPS(conn, bpoOID);
            UcmlBPO ubpo = new UcmlBPO(bps, "UCMLCommon");
            ubpo.CompileMode = false;
            ubpo.SavePath = @"E:\workspace\goldframe\web_platform\UCMLWebDev\BPObject";

            ubpo.VcTabList = BPOPrepare.GetVcTab(conn, bpoOID);
            ubpo.BCList = BPOPrepare.GetBC(conn, bpoOID);
            ubpo.SetVCPostion();

            //生成bpo.aspx页面并保存
            ubpo.BuildAspxPage();
            ubpo.SaveAspxPage();

            //生成bpo.aspx.cs 页面并保存
            ubpo.BuildAspxPageCs();
            ubpo.SaveAspxCs();

            //生成bpo.htc 并保存
            ubpo.BuildBpoHtc();
            ubpo.SaveHtc();

            //生成bpoService.asmx
            ubpo.BuildAsmxPage();
            ubpo.SaveAsmxPage();

            //生成bpoName.asmx.cs
            ubpo.BuildAsmxCs();
            ubpo.SaveAsmxCs();

            //生成bpodesign.cs
            ubpo.BuildAspxPageDesignCs();
            ubpo.SavePageDesignCs();
        }

        static void GenEditPageSource(SqlConnection conn,string vcOID,string vcName,string bcOID)
        {
            VcSourceGenner editForm = new VcSourceGenner(vcName);
            editForm.Kind = 165;
            editForm.PrepareData(conn, vcOID, bcOID);
            string htmlSrc = editForm.GetEditPageSource(3);
            Console.WriteLine(htmlSrc);
            string upSql = "update AppletDataSet set TargetHTMLSource='" + htmlSrc + "',HTMLSource='" + htmlSrc + "' where AppletDataSetOID='" + vcOID + "'";
            SqlCommand cmd = new SqlCommand(upSql, conn);
            int ret = cmd.ExecuteNonQuery();
        }
        static void AssembleBCLink(SqlConnection conn, string vcOID, string bcOID)
        {
            BCLink link4Goods = new BCLink();
            link4Goods.IsCompileMode = true;
            link4Goods.PrepareBCLink(conn, vcOID,bcOID);
            link4Goods.BuildBCHtc();
            link4Goods.BuildBCAsmx();
            link4Goods.BuildBCAsmxCs();
            link4Goods.Save(@"E:\workspace\goldframe\web_platform\UCMLWebDev\BPObject");
        }

        static void Main(string[] args)
        {
            string connStr = Util.GetDBConnecString("(local)", "UCMLWebDev", "sa", "goodluck");
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            //string vcOID = "0012C278-0000-0000-0000-000092E2552C";
            //string vcName = "VC_StudentQuery";
            //string bcOID = "0008A077-0000-0000-0000-00006A19C1F3";
            //GenEditPageSource(conn, vcOID, vcName, bcOID);

            string bpoid = "000FE6D9-0000-0000-0000-00006A318A20";
            AssembleBPO(conn, bpoid);

            //string bVcOID = "0006B982-0000-0000-0000-00008A5546E8";
            //string bBcOID = "0008A077-0000-0000-0000-00006A19C1F3";
            //AssembleBCLink(conn, bVcOID, bBcOID);

            Console.WriteLine("Success!");
            conn.Close();
            Console.ReadKey();
        }
    }
}
