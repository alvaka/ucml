using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class CssContext
    {
        public static HtmlNode GetCssLinkNode(string src)
        {
            HtmlNode node = new HtmlNode("link");
            node.Attributes["rel"] = "stylesheet";
            node.Attributes["type"] = "text/css";
            node.Attributes["href"] = src;
            node.ClosedTag = true;
            return node;
        }
    }
}
