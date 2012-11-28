using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class CSharpDoc
    {
        public string Name;
        public List<string> ReferenceNS;
        public string Namespace;
        public List<CSharpClass> InnerClass;

        public CSharpDoc(string name, string ns)
        {
            this.Name = name;
            this.Namespace = ns;
            ReferenceNS = new List<string>();
            InnerClass = new List<CSharpClass>();
            AddDefaultRefNS();
        }

        public void AddDefaultRefNS()
        {
            this.ReferenceNS.Add("System");
            this.ReferenceNS.Add("System.Collections.Generic");
            this.ReferenceNS.Add("System.Text");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string indent = "    ";
            foreach (string ns in ReferenceNS)
            {
                sb.AppendLine("using "+ns+";");
            }
            sb.AppendLine();
            sb.AppendLine("namespace "+Namespace);
            sb.AppendLine("{");
            foreach (CSharpClass cs in InnerClass)
            {
                string[] lines=Util.SplitLine(cs.ToString());
                for (int i = 0; i < lines.Length; i++)
                {
                    sb.AppendLine(indent+lines[i]); ;
                }
            }
            sb.Append("}");
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
