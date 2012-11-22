using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class HtcDoc
    {
        public string Name;
        public StringBuilder Definition;
        public StringBuilder Statement;
        public List<JsFunction> FuncList;
        public HtcDoc(string name)
        {
            this.Name = name;
            this.Definition = new StringBuilder();
            this.Statement = new StringBuilder();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.Definition.ToString());
            sb.AppendLine("<script language=\"jscript\">");
            sb.AppendLine(this.Statement.ToString());
            foreach (JsFunction fun in this.FuncList)
            {
                sb.AppendLine();
            }
            sb.AppendLine("</script>");
            
            return base.ToString();
        }
    }
}
