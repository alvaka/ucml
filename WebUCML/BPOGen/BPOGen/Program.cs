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
            ////HtmlDocument doc = new HtmlDocument("New Page");
            ////HtmlNode txtNode = HtmlNode.CreateTextNode("This is Body");
            ////HtmlNode divNode = new HtmlNode("div");
            ////divNode.Attributes["id"] = "div1";
            ////divNode.Append(txtNode);
            ////doc.GetNodesByName("body")[0].Append(divNode);
            ////HtmlNode btnNode = HtmlNode.CreateInputNode("button", "click");
            ////doc.GetNodeById("div1").Append(btnNode);
            ////HtmlNode head = doc.GetNodesByName("head")[0];
            ////head.Append(JsContext.GetJsLinkTag("../js/fuck.js"));
            ////JsContext js = new JsContext();
            ////js.AddDeclaration("tagName");
            ////js.AddDeclaration("tagID");
            ////js.AddStatement("tagID", "'tag1'");
            ////js.AddFunction("fun1", "tagName=GetElementById(tagID)");
            ////HtmlNode css = CssContext.GetCssLinkNode("../css/main.css");
            ////HtmlNode jsNode = js.GetJsBlockNode();
            ////head.Append(jsNode);
            ////jsNode.After(css);
            
            ////Console.WriteLine(js.GetJsBlockNode().ToString());
            //AspxDirective direct = new AspxDirective("Page");
            //direct.Attributes["language"] = "c#";
            //direct.Attributes["CodeBehind"] = "bpolist.aspx.cs";
            //AspxPage page = new AspxPage("bpolist.aspx","ASP.NET");
            //page.Directives.Add(direct);
            //Console.Write(page.ToString());
            //Console.ReadKey();
            ////StringBuilder sb = new StringBuilder();
            ////string content1 = "aaaaaaaaaaaaaaaaaa\r\nbbbbbbbbbbbbbbbbbb\r\n";
            ////string content2 = "cccccccccccccccccc\ndddddddddddddddddddd\n";
            ////string content3 = "EEEEEEEEEEEEEEEEEEEEEEEEEEEEE";
            ////string[] lines = Util.SplitLine(content1);
            ////foreach (string line in lines) sb.AppendLine("+++" + line);
            ////Console.Write(sb.ToString()+"END");


            //string connStr = Util.GetDBConnecString("(local)", "UCMLWEBIDEX", "sa", "goodluck");
            //SqlConnection conn = new SqlConnection(connStr);
            //conn.Open();
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = conn;
            //string sql = "select AppletName,Caption,TargetHTMLSource from AppletDataSet where AppletName='VC_GoodsForm'";
            //cmd.CommandText = sql;
            //SqlDataReader reader = cmd.ExecuteReader();
            //string src = "";
            //string name = "";
            //string caption = "";
            //if (reader.Read()) 
            //{ 
            //    src = reader.GetString(2);
            //    name = reader.GetString(0);
            //    caption = reader.GetString(1);
            //}
            //UcmlViewCompnent vc = new UcmlViewCompnent(name);
            //vc.Caption = caption;
            //vc.SetVCNode(src, "div");

            //reader.Close();
            //conn.Close();
            //string bpoName = "demo";
            //string bpoCaption = "Page1";
            //AspxPage page = new AspxPage(bpoName+".aspx", bpoCaption);
            
            //AspxDirective direc4Page = new AspxDirective("Page");
            //direc4Page["language"]= "C#";
            //direc4Page["codeBehind"]= page.PageName+".cs";
            //direc4Page["Inherits"]="UCMLCommon."+bpoName;
            //direc4Page["AutoEvenWireup"]="False";

            //AspxDirective direc4Reg = new AspxDirective("Register");
            //direc4Reg["TagPrefix"]= "iewc";
            //direc4Reg["Namespace"]="Microsoft.Web.UI.WebControls";
            //direc4Reg["Assembly"]="Microsoft.Web.UI.WebControls";

            //page.Directives.Add(direc4Page);
            //page.Directives.Add(direc4Reg);

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
            //string connStr = Util.GetDBConnecString("(local)", "UCMLWEBIDEX", "sa", "goodluck");
            //SqlConnection conn = new SqlConnection(connStr);
            //conn.Open();
            //BpoPropertySet bps = PrepareBPS(conn, 14356);
            BpoPropertySet bps = new BpoPropertySet();
            bps.Name = "BPO_GoodsList";
            bps.Capiton = "商品列表";
            UcmlBPO ubpo = new UcmlBPO(bps, "UCMLCommon");
            ubpo.BuildPageCs();
            Console.Write(ubpo.PageCs.ToString());
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
            return null;
        }
    }
}
