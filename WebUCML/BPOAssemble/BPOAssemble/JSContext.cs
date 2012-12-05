using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class JsContext
    {
        private Dictionary<string, string> JsStatement;
        private List<string> JsDeclaration;
        public List<JsFunction> JsFuncs;
        public StringBuilder Content;

        public JsContext()
        {
            JsFuncs = new List<JsFunction>();
            JsDeclaration = new List<string>();
            JsStatement = new Dictionary<string, string>();
            Content = new StringBuilder();
        }

        public HtmlNode GetJsBlockNode()
        {
            HtmlNode scriptNode = new HtmlNode("script");
            scriptNode.Attributes["type"] = "text/javascript";
            HtmlNode txtNode = HtmlNode.CreateTextNode(this.ToString());
            scriptNode.Append(txtNode);
            return scriptNode; 
        }

        public void AddFunction(string name,string text)
        {
            JsFunction func=new JsFunction();
            func.Name=name;
            func.Content.Append(text);
            JsFuncs.Add(func);
        }

        public void AddDeclaration(string varname)
        {
            if (varname != null && varname != "") JsDeclaration.Add(varname);
        }

        public void AddStatement(string name, string value)
        {
            if (name != null && name != "") JsStatement.Add(name, value);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (JsDeclaration.Count != 0)
            {
                foreach (string var in JsDeclaration) sb.AppendLine("var "+var+";");
            }
            if (JsStatement.Count != 0)
            {
                foreach (string var in JsStatement.Keys) sb.AppendLine(var+"="+JsStatement[var]+";");
            }
            sb.Append(Content);
            if (JsFuncs.Count != 0)
            {
                foreach (JsFunction fun in JsFuncs) sb.AppendLine(fun.ToString());
            }
            return sb.ToString();
        }

        public static HtmlNode GetJsLinkTag(string src)
        {
            HtmlNode node = new HtmlNode("script");
            node.Attributes["type"] = "text/javascript";
            node.Attributes["src"] = src;
            return node;
        }
    }

    public class JsFunction
    {
        public string Name;
        public List<string> Params;
        public StringBuilder Content;
        public JsFunction()
        {
            Params = new List<string>();
            Content = new StringBuilder();
        }
        public JsFunction(string name)
        {
            this.Name = name;
            Params = new List<string>();
            Content = new StringBuilder();
        }
        
        public override string ToString() //将JS函数转换成文本
        {
            StringBuilder sb = new StringBuilder("function "+this.Name+"(");
            if (Params != null && Params.Count != 0)
            {
                for (int i = 0; i < Params.Count; i++)
                {
                    sb.Append(Params[i]);
                    if (i != Params.Count - 1) sb.Append(",");
                }
            }

            sb.AppendLine("){");
            string indentStr = "   ";
            string[] lines = Util.SplitLine(Content.ToString());
            if (lines != null)
            {
                foreach (string line in lines) sb.AppendLine(indentStr+line);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
   
}
