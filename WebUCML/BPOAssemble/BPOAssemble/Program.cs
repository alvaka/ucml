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
        static void Main(string[] args)
        {
            string connStr = Util.GetDBConnecString("(local)", "UCMLWebDev", "sa", "goodluck");
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            string bpoid = "000FE6D9-0000-0000-0000-00006A318A20";
            BpoPropertySet bps = BPOPrepare.GetBPS(conn, bpoid);
            UcmlBPO ubpo = new UcmlBPO(bps, "UCMLCommon");
            ubpo.CompileMode = false;
            ubpo.SavePath = @"E:\workspace\goldframe\web_platform\UCMLWebDev\BPObject";
            
            ubpo.VcTabList = BPOPrepare.GetVcTab(conn, bpoid);
            ubpo.BCList = BPOPrepare.GetBC(conn, bpoid);
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

            //Console.Write(ubpo.Page.ToString());
            //Console.ReadKey();
        }
       
    }
}
