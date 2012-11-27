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
            this.FuncList = new List<JsFunction>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.Definition.ToString());
            sb.AppendLine("<script language=\"jscript\">");
            sb.AppendLine(this.Statement.ToString());
            foreach (JsFunction fun in this.FuncList) 
            {
                sb.AppendLine(fun.ToString());
            }
            sb.AppendLine("</script>");
            
            return sb.ToString();
        }

        public bool Save(string path)
        {
            if (path == null || path == "") return false;
            if (!path.EndsWith("\\")) path += "\\";
            return Util.SaveTextFile(this.ToString(), path + this.Name);
        }
    }
}
