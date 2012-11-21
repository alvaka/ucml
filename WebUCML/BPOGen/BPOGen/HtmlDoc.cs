using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class HtmlDocument
    {
        protected HtmlNode _RootNode;
        protected string _CharSet="gb2312";
        protected string _DocType="html";
        protected HtmlNode _Head;
        protected HtmlNode _Body;
        public HtmlNode Head
        {
            get { return _Head; }
            set { _Head = value; }
        }
        public HtmlNode Body
        {
            get { return _Body; }
            set { _Body = value; }
        }
        public HtmlNode RootNode
        {
            get { return _RootNode; }
        }

        public string CharSet
        {
            get { return _CharSet; }
        }
        public string DocType
        {
            get { return _DocType; }
            set { _DocType = value; }
        }
        public HtmlDocument()
        {
        }
        public HtmlDocument(string title) 
        {
            _DocType = "html";
            _CharSet = "gb2312";
            InitStandardDocument(title);
        }
        public HtmlDocument(string docType,string charset)
        {
            _DocType = docType;
            _CharSet = charset;
            InitStandardDocument("");
        }

        public void InitStandardDocument(string title)
        {
            this._RootNode = new HtmlNode("html");
            HtmlNode head = new HtmlNode("head");
            HtmlNode titleNode = new HtmlNode("title");
            if(title!="")titleNode.Append(HtmlNode.CreateTextNode(title)); //标题非即添加文本节点
            head.Append(titleNode);

            HtmlNode metaChar = HtmlNode.CreateClosedNode("meta");
            metaChar.Attributes["http-equiv"] = "Content-type";
            metaChar.Attributes["content"] = "text/html; charset="+_CharSet;
            head.Append(metaChar);

            this.RootNode.Append(head);
            HtmlNode body = new HtmlNode("body");
            this.RootNode.Append(body);
            Head = head;
            Body = body;
        }

        public HtmlNode GetNodeById(string id)
        { 
            List<HtmlNode> list=new List<HtmlNode>();
            HtmlNode.GetChildNodeById(list,RootNode,id);
            if (list.Count == 0) return null;
            else return list[0];
        }

        public List<HtmlNode> GetNodesByName(string name)
        {
            List<HtmlNode> nodes = new List<HtmlNode>();
            HtmlNode.GetChildNodesByName(nodes, RootNode, name);
            return nodes; 
        }

        public bool AppendNode(HtmlNode node, string pNodeMark)
        {
            if(pNodeMark.StartsWith("#"))
            {
                HtmlNode p=GetNodeById(pNodeMark.Substring(1));
                if(p!=null)p.Append(node);
                else return false;
            }
            else
            {
                List<HtmlNode> list=GetNodesByName(pNodeMark);
                if(list.Count!=0)
                {
                   foreach(HtmlNode c in list)
                   {
                       c.Append(node);
                   }
                }
                else return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "<!DOCTYPE "+DocType+">\r\n"+RootNode.ToString();
        }

        public bool ParseDocument(string context)
        {
            try
            {
                _RootNode = ParseNode(context)[0];
            }
            catch (HtmlDocParseExeption e)
            {
                return false;
            }
            return true;
        }

        public bool CheckDocument(string rootName)
        {
            if (!CheckDocument("html")) return false;

            return true;
        }

        /// <summary>
        /// 解析传入的字符串，并返回最上层节点列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static  List<HtmlNode> ParseNode(string text)
        {
            if (text == "") return null;
            List<HtmlNode> nodeList = new List<HtmlNode>();
            int nodeIndex=0;
            while(nodeIndex<text.Length-1)
            {
               NodeString nodeStr = ParseNodeString(text,ref nodeIndex);
               if (nodeStr.Type == 3||nodeStr.Type==2)//文本节点和闭合标签
               {
                   nodeList.Add(HtmlNode.ParseNode(nodeStr.NodeBlock));
               }
               else if(nodeStr.Type==0)//处理开放节点,寻找结束标记
               {
                   int tmpIndex = nodeIndex;
                   string tagName=HtmlNode.GetNodeName(nodeStr.NodeBlock);
                   NodeString tmpNode=null;
                   Stack<int> cntStack = new Stack<int>();
                   cntStack.Push(0);
                   while (cntStack.Count != 0)
                   {
                       if (tmpIndex >= text.Length) break;//到文本结尾仍未找到结束标签,则跳出
                       tmpNode=ParseNodeString(text, ref tmpIndex);
                       string tmpTagName = HtmlNode.GetNodeName(tmpNode.NodeBlock);
                       if(tmpTagName.ToLower()==tagName.ToLower())
                       {
                           if (tmpNode.Type == 0) cntStack.Push(0);
                           else if (tmpNode.Type == 1) cntStack.Pop();
                       }
                   }
                   if (cntStack.Count == 0)//找到结束标签
                   {
                       HtmlNode node = HtmlNode.ParseNode(nodeStr.NodeBlock);
                       string innerNode = text.Substring(nodeIndex, tmpIndex - nodeIndex-tmpNode.NodeBlock.Length);
                       if (innerNode != "")//递归解析子节点
                       {
                           node.Childs = ParseNode(innerNode);
                       }
                       nodeList.Add(node);
                       nodeIndex = tmpIndex;
                   }
                   else
                   {
                       HtmlDocParseExeption err = new HtmlDocParseExeption("Bad Document,Miss enclosed tag for " + nodeStr.NodeBlock);
                       throw err;
                   }
               }

            }//循环结束
            return nodeList;
        }

        public static List<HtmlNode> ParseNodeByName(string text,string nodeName)
        {
            if (text == "") return null;
            List<HtmlNode> nodeList = new List<HtmlNode>();
            int nodeIndex = 0;
            while (nodeIndex < text.Length - 1)
            {
                NodeString nodeStr = ParseNodeStringByName(text, ref nodeIndex,nodeName);
                if (nodeStr == null) break;
                else if (nodeStr.Type == 2) nodeList.Add(HtmlNode.ParseNode(nodeStr.NodeBlock));
                else if(nodeStr.Type==0)
                {
                    int tmpIndex = nodeIndex;
                    string tagName = HtmlNode.GetNodeName(nodeStr.NodeBlock);
                    NodeString tmpNode = null;
                    Stack<int> cntStack = new Stack<int>();
                    cntStack.Push(0);
                    while (cntStack.Count != 0)
                    {
                        if (tmpIndex >= text.Length) break;//到文本结尾仍未找到结束标签,则跳出
                        tmpNode = ParseNodeString(text, ref tmpIndex);
                        string tmpTagName = HtmlNode.GetNodeName(tmpNode.NodeBlock);
                        if (tmpTagName.ToLower() == nodeName.ToLower())
                        {
                            if (tmpNode.Type == 0) cntStack.Push(0);
                            else if (tmpNode.Type == 1) cntStack.Pop();
                        }
                    }
                    if (cntStack.Count == 0)//找到结束标签
                    {
                        HtmlNode node = HtmlNode.ParseNode(nodeStr.NodeBlock);
                        string innerNode = text.Substring(nodeIndex, tmpIndex - nodeIndex - tmpNode.NodeBlock.Length);
                        if (innerNode != "")//递归解析子节点
                        {
                            node.Childs = ParseNode(innerNode);
                        }
                        nodeList.Add(node);
                        nodeIndex = tmpIndex;
                    }
                    else break;
                }

            }//循环结束
            return nodeList;
        }

        /// <summary>
        /// 解析第一个节点（包括文本节点）
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static  NodeString ParseNodeString(string text, ref int index)
        {
            if (text == "") return null;
            NodeString nodeStr = new NodeString();
            int start = text.IndexOf('<',index);
            if (start == -1)//文本节点
            {
                nodeStr.Type = 3;
                nodeStr.NodeBlock = text.Substring(index);
                index = text.Length - 1;
                return nodeStr;
            }
            else if (start > index)//前面有文本节点
            {
                nodeStr.Type = 3;
                nodeStr.NodeBlock = text.Substring(index, start - index);//保存文本节点
                index = start;
                int tmpIndex=index;
                NodeString tmp =ParseNodeString(text,ref tmpIndex);
                if(tmp.Type==3)
                {
                    nodeStr.NodeBlock=nodeStr.NodeBlock+tmp.NodeBlock;
                    index = tmpIndex;
                }
                //如果不是文本节点，则还原index到上一次位置
                return nodeStr;
            }
            int end = text.IndexOf('>', start);//若Tag内出现>会出错
            if (end == -1)//无>标记表示为文本节点
            {
                index = text.Length - 1;
                nodeStr.Type = 3;
                nodeStr.NodeBlock = text.Substring(index);
                return nodeStr;
            }
            int length = end - start+1;
            if (length < 3)//一个标签长度最少为3
            {
                nodeStr.Type = 3;
                nodeStr.NodeBlock = text.Substring(index, length);
                index = end + 1;
                int tmpIndex = index;
                NodeString tmp = ParseNodeString(text, ref tmpIndex);
                if (tmp.Type == 3)
                {
                    nodeStr.NodeBlock = nodeStr.NodeBlock + tmp.NodeBlock;
                    index = tmpIndex;
                }
                return  nodeStr; //若文本里出现<>继续搜索
            }
            else//解析到类常规标签,可能是<123>等不合法标签
            {
                string tagBlock = text.Substring(start, length);
                string tagName = HtmlNode.GetNodeName(tagBlock);
                if (HtmlNode.IsValidHtmlNode(tagName))//检验标签名是否合法
                {
                    index = end + 1;//索引前进
                    nodeStr.Type = HtmlNode.GetNodeStringType(tagBlock);
                    nodeStr.NodeBlock = tagBlock;
                    return nodeStr;
                }
                else//标签不合法则视为文本
                {
                    nodeStr.Type = 3;
                    nodeStr.NodeBlock = text.Substring(index, length);
                    index = end + 1;
                    int tmpIndex = index;
                    NodeString tmp = ParseNodeString(text, ref tmpIndex);
                    if (tmp.Type == 3)
                    {
                        nodeStr.NodeBlock = nodeStr.NodeBlock + tmp.NodeBlock;
                        index = tmpIndex;
                    }
                    return nodeStr; //若文本里出现<>继续搜索
                }
            }
        }

        private static NodeString ParseNodeStringByName(string text, ref int index,string nodeName)
        {
            if (text == "") return null;
            NodeString nodeStr = new NodeString();
            while (index < text.Length)
            {
                int start = text.IndexOf('<', index);
                if (start == -1) return null;
                int end = text.IndexOf('>', start);
                if (end == -1) return null;
                int length = end - start + 1;
                if (length < 3) continue;
                else
                {
                    string tagBlock = text.Substring(start, length);
                    string tagName = HtmlNode.GetNodeName(tagBlock);
                    index = end + 1;//索引前进
                    if (HtmlNode.IsValidHtmlNode(tagName)&&tagName.ToLower()==nodeName.ToLower())//检验标签名是否合法
                    {
                        nodeStr.Type = HtmlNode.GetNodeStringType(tagBlock);
                        nodeStr.NodeBlock = tagBlock;
                        return nodeStr;
                    }
                }
            }
            return null;
        }

        private class NodeString
        {
            //type:0表示开标签的前部，1表示开标签的后部，2表示闭标签，3表示文本标签
            public int Type;
            public string NodeBlock;
        }

        public class HtmlDocParseExeption : Exception
        {
            public HtmlDocParseExeption(string message)
                : base(message)
            { }
        }
        
    }
   
}
