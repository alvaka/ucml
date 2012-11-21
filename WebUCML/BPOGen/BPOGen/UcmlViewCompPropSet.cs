using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlViewCompnent
    {
        public string VCName;
        public string Caption;
        public HtmlNode VCNode;

        /// <summary>
        /// 初始化VCName的构造函数
        /// </summary>
        public UcmlViewCompnent(string name)
        {
            this.VCName = name;
        }

        /// <summary>
        /// 设置VC的HTML节点
        /// </summary>
        /// <param name="context">HTML源码字符串</param>
        /// <param name="nodeName">根节点名称</param>
        /// <returns></returns>
        public bool SetVCNode(string context, string nodeName)
        {
            if (context == null || nodeName == null) return false ;
            if (context == "" || nodeName == "") return false;
            List<HtmlNode> nodesParsed = HtmlDocument.ParseNodeByName(context, nodeName);
            if (nodesParsed.Count == 0) return false;
            VCNode = nodesParsed[0];
            return true;
        }
    }

    public class UcmlVcTabPage
    {
        public List<UcmlViewCompnent> VCList;
        public string Caption;
        public string Name;
        public UcmlVcTabPage()
        {
            VCList = new List<UcmlViewCompnent>();
        }

        public void Add(UcmlViewCompnent vc)
        {
            VCList.Add(vc);
        }
    }
}
