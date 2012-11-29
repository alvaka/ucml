using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlViewCompnent
    {
        public int LinkOID;
        public int LinkPOID;
        public int OID;
        public string VCName;
        public string Caption;
        public string BCName;
        public HtmlNode VCNode;
        public bool fTreeGridMode;
        public bool fSubTableTreeMode;
        public string ImageLink;
        public string SubBCs;
        public string SubParentFields;
        public string SubLabelFields;
        public string SubPicFields;
        public string SubFKFields;
        public bool EnabledEdit;
        public bool haveMenu;
        public bool fHidden;
        public bool alignHeight;
        public bool alignWidth;
        public bool UserDefineHTML;
        public int Kind;

        public List<UcmlVcColumn> Columns;

        public UcmlViewCompnent()
        {
            VCName = "";
            ImageLink = "";
            SubBCs = "";
            SubParentFields = "";
            SubLabelFields = "";
            SubPicFields = "";
            SubFKFields = "";
            Columns = new List<UcmlVcColumn>();
        }

        /// <summary>
        /// 初始化VCName的构造函数
        /// </summary>
        public UcmlViewCompnent(string name)
        {
            this.VCName = name;
            ImageLink = "";
            SubBCs = "";
            SubParentFields = "";
            SubLabelFields = "";
            SubPicFields = "";
            SubFKFields = "";
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

    public class UcmlVcColumn
    {
        public string FieldName;
        public string Caption;
        public bool fDisplay;
        public bool fCanModify;
        public int CurrentPos;
        public int Width;
        public bool fFixColumn;
        public string FixColumnValue;
        public bool fCustomerControl;
        public string CustomerControlHTC;
        public string ControlID;
        public string EditContrl;


    }

    public class UcmlVcTabPage
    {
        public List<UcmlViewCompnent> VCList;
        public string Caption;
        public string Name;
        public int ParentOID;
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
