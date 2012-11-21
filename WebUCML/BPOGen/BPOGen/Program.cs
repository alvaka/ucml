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
            ////添加JS引用，JS函数定义
            //HtmlNode jsLink = JsContext.GetJsLinkTag("js/ucmlapp.js");
            //JsContext jsContext = new JsContext();
            //jsContext.AddFunction("Init", "alert('Document Initial Event!')");
            //page.Head.Append(jsLink);
            //page.Head.Append(jsContext.GetJsBlockNode());

            ////初始化BODY
            //page.InitPage();

            ////添加VC页面
            //page.AddVCPage(vc);
            //Console.WriteLine(page.ToString());
            //CSharpFunction fun1 = new CSharpFunction("Function1");
            //fun1.AccessAuth = AccessAuthority.PUBLIC;
            //fun1.ReturnType = "void";
            //fun1["string"] = "para1";
            //fun1["int"] = "para2";
            //fun1.Append("int a=1;\r\nConsole.Write(para1+\" \"+para2);");
            //CSharpClassField field = new CSharpClassField("string", "_Name");
            //CSharpClassAttribute attr = new CSharpClassAttribute("Name",field);
            //CSharpClass testClass = new CSharpClass("Demo");
            //testClass.FunctionList.Add(fun1);
            //testClass.FieldList.Add(field);
            //testClass.AttributeList.Add(attr);
            ////Console.WriteLine(field.ToString());
            ////Console.WriteLine(attr.ToString());
            ////Console.WriteLine(fun1.ToString());

            //CSharpDoc doc = new CSharpDoc("page.cs", "ALVA");
            //doc.InnerClass.Add(testClass);
            //Console.Write(doc.ToString());
            //string connStr = Util.GetDBConnecString("(local)", "UCMLWEBIDEX", "sa", "goodluck");
            string connStr = Util.GetDBConnecString("(local)", "UCMLWEBIDEX", "sa", "goodluck");
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            int bpoid = 14356;
            BpoPropertySet bps = PrepareBPS(conn, bpoid);
            //BpoPropertySet bps = new BpoPropertySet();
            //bps.Name = "BPO_GoodsList";
            //bps.Capiton = "商品列表";
            UcmlBPO ubpo = new UcmlBPO(bps, "UCMLCommon");
            ubpo.VcTabList = PrepareVcTab(conn, bpoid);
            ubpo.BuildPageCs();
            ubpo.BuildAspxPage();
            //Console.Write(ubpo.PageCs.ToString());
            Console.Write(ubpo.Page.ToString());
            Console.ReadKey();
            
        }
        //根据BPO GUID准备BPO数据
        public static BpoPropertySet PrepareBPS(SqlConnection conn, int guid)
        {
            BpoPropertySet bps = new BpoPropertySet();
            bps.GUID = guid;
            string sql = "select ClassName,ChineseName from UCMLClassDataSet where UCMLClassOID="+guid;
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader= cmd.ExecuteReader();
            if (reader.Read())
            {
                bps.Name = reader.GetString(0);
                bps.Capiton = reader.GetString(1);
            }
            reader.Close();
            reader = null;
            cmd = null;
            return bps;
        }

        public static List<UcmlVcTabPage> PrepareVcTab(SqlConnection conn, int bpoid)
        {
            List<UcmlVcTabPage> tabList = new List<UcmlVcTabPage>();
            UcmlVcTabPage vcTab = new UcmlVcTabPage();
            UcmlViewCompnent vc = new UcmlViewCompnent();

            string sql = "select AppletName,Caption,TargetHTMLSource from AppletDataSet where AppletName='VC_GoodsList'";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                vc.VCName = Util.GetPropString(reader,0);
                vc.Caption = Util.GetPropString(reader,1);
                vc.SetVCNode(Util.GetPropString(reader,2),"div");
            }
            reader.Close();
            vcTab.Name = vc.VCName;
            vcTab.Caption = vc.Caption;
            vcTab.Add(vc);
            tabList.Add(vcTab);
            return tabList;
        }

        public static List<UcmlViewCompnent> PrepareBC(SqlConnection conn, int bpoid)
        {
            return null;
        }
    }
}
