using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class AsmxDoc
    {
        public string Name;
        public List<AspxDirective> Directives;
        public StringBuilder Content;

        public AsmxDoc(string name)
        {
            this.Name = name;
            Directives = new List<AspxDirective>();
            Content = new StringBuilder();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (AspxDirective directive in Directives)
            {
                sb.AppendLine(directive.ToString());
            }
            //sb.AppendLine(Content.ToString());
            return sb.ToString();
        }

        public bool Save(string path)
        {
            if (path == null || path == "") return false;
            if(!path.EndsWith("\\"))path+="\\";
            return Util.SaveTextFile(this.ToString(), path + this.Name);
        }
    }
}
