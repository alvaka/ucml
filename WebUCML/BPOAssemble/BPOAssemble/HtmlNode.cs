using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UCML.IDE.WebUCML
{
    public class HtmlNode
    {
        private string _name;
        private string _type;
        private HtmlNode _father;
        private List<HtmlNode> _childs;
        private HtmlNode _next;
        private Dictionary<string, string> _attributes;
        private string _innerHTML;
        private string _innerText;
        private int _depth;
        private bool _fClosedTag;
        private bool _fNoID;
        private bool _fOnlyText;

        //属性定义
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string this[string key]
        {
            get { return Attributes[key]; }
            set { Attributes[key.ToLower()] = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public HtmlNode Father
        {
            get { return _father; }
            set { _father = value; }
        }
        public List<HtmlNode> Childs
        {
            get { return _childs; }
            set 
            {
                foreach (HtmlNode node in value)
                {
                    this.Append(node);
                }
                _childs = value; 
            }
        }

        public HtmlNode Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public Dictionary<string, string> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
        public string InnerHTML
        {
            get { return _innerHTML; }
            set { _innerHTML = value; }
        }
        public int Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }
        public bool NoID
        {
            get { return _fNoID; }
            set { _fNoID = value; }
        }
        public bool OnlyText
        {
            get { return _fOnlyText; }
            set { _fOnlyText = value; }
        }
        public bool ClosedTag
        {
            get { return _fClosedTag; }
            set { _fClosedTag = value; }
        }
        public string InnerText
        {
            get { return _innerText; }
            set 
            { 
                _innerText = value;
                HtmlNode txtNode = new HtmlNode();
                txtNode.OnlyText = true;
                txtNode.InnerHTML = value;
            }
        }
        public HtmlNode()//空构造函数
        {
            Attributes = new Dictionary<string, string>();
            Childs = new List<HtmlNode>();
        }
        public HtmlNode(string name)
        {
            Attributes = new Dictionary<string, string>();
            Childs = new List<HtmlNode>();
            this.InnerText = "";
            this.InnerHTML = "";
            this.Name = name;
        }

        public HtmlNode(string name,string type)
        {
            Attributes = new Dictionary<string, string>();
            Childs = new List<HtmlNode>();
            this.Name = name;
            this.Type = type;
        }
        //结点操作函数

        /// <summary>
        /// 在this节点前面插入node结点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Before(HtmlNode node)
        { return true; }

        /// <summary>
        /// 在this节点后面插入node结点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool After(HtmlNode node)
        {
            if (node == null) return false;
            if (this.Next == null) this.Next = node;
            else {
                HtmlNode tmp = this.Next;
                this.Next = node;
                node.Next = tmp;
                tmp = null;
            }
            node.Depth = this.Depth;
            node.Father = this.Father;
            return true; 
        }

        /// <summary>
        /// 将node节点添加为this的子节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Append(HtmlNode node)
        {
            if (node == null) return false;
            node.Next = null;              //丢弃node节点的下一节点信息
            this.Childs.Add(node);
            node.Depth = this.Depth + 1;
            node.Father = this;
            node.SetChildNodesDepth();
            return true; 
        }

        private void SetChildNodesDepth()
        {
            if (this.Childs.Count != 0)
            {
                foreach (HtmlNode node in this.Childs)
                {
                    node.Depth = this.Depth + 1;
                    node.SetChildNodesDepth();
                }
            }
        }
        private string GetIndentSpace()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Depth; i++) sb.Append("   ");
            return sb.ToString();
        }

        public override string ToString() //将节点信息转换成文本
        {
            string indentSpace = GetIndentSpace();
            StringBuilder sbNodeText = new StringBuilder();

            if (this.OnlyText)//如果是文本结点，则直接返回文本值
            {
                string[] lines = Util.SplitLine(this.InnerHTML);
                int i = 0;
                for(;i<lines.Length-1;i++){
                    sbNodeText.AppendLine(indentSpace + lines[i]);
                }
                sbNodeText.Append(indentSpace + lines[i]);
                return sbNodeText.ToString();
            }

            sbNodeText.Append(indentSpace + "<" + this.Name);
            if (this.Attributes.Count != 0)
            {
                sbNodeText.Append(" ");
                foreach (string attr in Attributes.Keys)
                {
                    if (Attributes[attr] == null) sbNodeText.Append(attr + " ");
                    else
                    {
                        if (Attributes[attr].IndexOf('\"') != -1) sbNodeText.Append(attr + "=\'" + Attributes[attr] + "\' ");
                        else sbNodeText.Append(attr + "=\"" + Attributes[attr] + "\" ");
                    }
                }
            }
            if (this.ClosedTag) sbNodeText.Append("/>");
            else if (this.Childs.Count == 0) sbNodeText.Append("></"+this.Name+">");
            else
            {
                sbNodeText.AppendLine(">");
                foreach (HtmlNode child in this.Childs)
                {
                    sbNodeText.AppendLine(child.ToString());
                }
                sbNodeText.Append(indentSpace + "</" + this.Name + ">");
            }
            return sbNodeText.ToString();
        }
        public static HtmlNode CreateTextNode(string text)
        {
            HtmlNode node = new HtmlNode();
            node.OnlyText = true;
            node.InnerHTML = text;
            node.InnerText = text;
            node.Name = "";
            return node;
        }
        public static HtmlNode CreateClosedNode(string name)
        {
            HtmlNode node = new HtmlNode(name);
            node.ClosedTag = true;
            return node;
        }
       
        public static HtmlNode CreateInputNode(string type,string value)
        {
            HtmlNode node = new HtmlNode("input");
            node.ClosedTag = true;
            node.Attributes["type"] = type;
            node.Attributes["value"]=value;
            return node;
        }

        public static void GetChildNodeById(List<HtmlNode> list, HtmlNode pNode, string id)
        {
            if (list.Count != 0) return;
            if (pNode.Attributes.Keys.Contains("id"))//不区分大小比较
            {
                if (pNode.Attributes["id"].ToLower() == id.ToLower())
                {
                    list.Add(pNode);
                    return;
                }
            }
            if (pNode.Childs.Count != 0)
            {
                foreach (HtmlNode childNode in pNode.Childs)
                {
                    GetChildNodeById(list, childNode, id);
                }
            }
        }

        public static void GetChildNodesByName(List<HtmlNode> list, HtmlNode node, string name)//递归按名称查找节点
        {
            if (node == null) return;
            if (node.Name.ToLower() == name.ToLower()) list.Add(node);
            if (node.Childs.Count != 0)
            {
                foreach (HtmlNode childNode in node.Childs)
                {
                    GetChildNodesByName(list, childNode, name);
                }
            }
        }
        

        public static bool IsValidHtmlNode(string tagName)
        {
            if(tagName=="")return false;
            Char[] digits=new []{'0','1','2','3','4','5','6','7','8','9'};
            if (tagName.IndexOfAny(digits) != -1) return false;
            return true;
        }
        
        /// <summary>
        /// 从一个表示标签的字符串中提取标签名
        /// </summary>
        /// <param name="nodeString"></param>
        /// <returns></returns>
        public static string GetNodeName(string nodeString)
        {
            string tmp = (new Regex("\r\n")).Replace(nodeString, " ");
            if (!Regex.IsMatch(tmp, "^<.*>$")) return "";
            else 
            {
                Match match = Regex.Match(tmp,"[a-zA-Z]+");
                return match.Value;
            }
        }

        public static int GetNodeStringType(string nodeString)
        {
            string tmp = (new Regex("\r\n")).Replace(nodeString," ");
            if (!Regex.IsMatch(tmp, "^<.*>$")) return 3;
            else if (Regex.IsMatch(tmp, "^<.*/>$")) return 2;
            else if (Regex.IsMatch(tmp, "^</.*>$")) return 1;
            else
            {
                string tagName = GetNodeName(tmp);
                if (tagName.ToLower() == "input" ||
                    tagName.ToLower() == "br"||
                    tagName.ToLower()=="img"||
                    tagName.ToLower()=="base"||
                    tagName.ToLower()=="area") return 2;
                else return 0;
            }

        }

        /// <summary>
        /// 从表示节点的字符串解析出节点
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static HtmlNode ParseNode(string text)
        {
            int type = GetNodeStringType(text);
            if (type == 3) return HtmlNode.CreateTextNode(text);//文本节点
            string tagName = HtmlNode.GetNodeName(text);
            if (type == 0)
            {
                HtmlNode node = new HtmlNode(tagName);
                node.Attributes = ParseAttribute(text);
                return node;
            }
            else if (type == 2)
            {
                HtmlNode node = HtmlNode.CreateClosedNode(tagName);
                node.Attributes = ParseAttribute(text);
                return node;
            }
            else return null;
        }
        /// <summary>
        /// 解析节点属性
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseAttribute(string text)
        {
            text=(new Regex("\r\n")).Replace(text, " ");
            string pattern1 = "[a-zA-Z]+=\"[^\"]+\"";// 解析属性值带双引号的
            Regex reg1 = new Regex(pattern1);
            MatchCollection matchs1 = reg1.Matches(text);
            Dictionary<string, string> attr = new Dictionary<string, string>();
            foreach (Match match in matchs1)
            {
                string item = match.Value;
                string[] parts=item.Split(new char[]{'='});
                string value = parts[1];
                value = value.Substring(1, value.Length - 2);
                attr.Add(parts[0].ToLower(), value);
            }

            string pattern2 = "[a-zA-Z]+=\'[^\']+\'";//解析属性值带单引号的
            Regex reg2 = new Regex(pattern2);
            MatchCollection matchs2 = reg2.Matches(text);
            foreach (Match match in matchs2)
            {
                string item = match.Value;
                string[] parts = item.Split(new char[] { '=' });
                string value = parts[1];
                value = value.Substring(1, value.Length - 2);
                attr.Add(parts[0].ToLower(), value);
            }

            string pattern3 = "[a-zA-Z]+=[^\\s\'\"/>]+";//解析属性值不带引号的
            Regex reg3 = new Regex(pattern3);
            MatchCollection matchs3 = reg3.Matches(text);
            foreach (Match match in matchs3)
            {
                string item = match.Value;
                string[] parts = item.Split(new char[] { '=' });
                string value = parts[1];
                attr.Add(parts[0].ToLower(), value.Trim());
            }

            string pattern4 = "\\s[a-zA-Z]+[\\s/>]";//解析不带属性值的
            Regex reg4 = new Regex(pattern4);
            MatchCollection matchs4 = reg4.Matches(text);
            foreach (Match match in matchs4)
            {
                attr.Add(match.Value.Substring(1, match.Value.Length - 2).ToLower(), null);
            }
            return attr;
        }
        public static string[] TagList={"html","head","body"};
    }
}
