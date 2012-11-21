using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class AspxPage:HtmlDocument
    {
        public List<AspxDirective> Directives;
        public string PageName;
        public List<UcmlViewCompnent> VCList;//BPO包含的VC页面的集合
        public HtmlNode FormNode;
        public HtmlNode MainPanelNode;

        public AspxPage(string name):base()
        {
            this.PageName = name;
            Directives = new List<AspxDirective>();
            VCList = new List<UcmlViewCompnent>();
        }

        public AspxPage(string pageName, string title)
            : base(title)
        {
            this.PageName = pageName;
            Directives = new List<AspxDirective>();
            VCList = new List<UcmlViewCompnent>();
        }

        public AspxPage(string name, string docType, string charset)
            : base(docType, charset)
        {
            this.PageName = name;
            Directives = new List<AspxDirective>();
            VCList = new List<UcmlViewCompnent>();
        }

        public void InitPage()
        {
            FormNode = new AspxNode("form");//添加Form节点
            FormNode["id"] = "Form1";
            FormNode["method"] = "post";

            AspxNode naviBar = new AspxNode("asp:Panel");//添加导航条节点
            naviBar["id"] = "NaviPageBar";
            naviBar["visible"] = "false";
            naviBar["style"] = "Z-INDEX:100;LEFT:0px;TOP:0px";
            naviBar["height"] = "20px";
            naviBar["width"] = "100%";

            MainPanelNode = new HtmlNode("div");
            MainPanelNode["id"] = "MainPanel";
            MainPanelNode["style"] = "Z-INDEX:100;left:0px;top:0px";
            MainPanelNode["class"] = "UCML-MailPanel";

            FormNode.Append(naviBar);
            FormNode.Append(MainPanelNode);
            Body.Append(FormNode);
        }
        public void AddJsLink(string src)
        { }
        public void AddCssLink(string src)
        { }

        public void AddVCPage(UcmlViewCompnent vc)
        {
            VCList.Add(vc);
            MainPanelNode.Append(vc.VCNode);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (AspxDirective direct in Directives)
            {
                sb.AppendLine(direct.ToString());
            }
            return sb.Append(base.ToString()).ToString();
        }
    }

    public class AspxDirective
    {
        public string Name;
        public Dictionary<string, string> Attributes;

        public string this[string key] //获取或者设置指令属性
        {
            get { return Attributes[key]; }
            set { Attributes[key] = value; }
        }

        //带指令名称的构造函数
        public AspxDirective(string name)
        {
            this.Name = name;
            Attributes = new Dictionary<string, string>();
        }

        //重写ToString方法，返回指字符串
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("<%@ "+this.Name+" ");
            if (Attributes.Count != 0)
            {
                foreach (string key in Attributes.Keys)
                {
                    sb.Append(key+"=\""+Attributes[key]+"\" ");
                }
            }
            sb.Append("%>");
            return sb.ToString();
        }
    }

    public class AspxNode : HtmlNode
    {
        public AspxNode(string name):base(name)
        {
            this["runat"] = "server";
        }
    }
}
