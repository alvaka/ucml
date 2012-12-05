using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class CSharpClass
    {
        public string Name;
        public AccessAuthority AccessAuth;
        public bool IsStatic;
        public bool IsAbstract;
        public bool IsPartial;
        public List<CSharpClassField> FieldList;
        public List<CSharpClassAttribute> AttributeList;
        public List<CSharpFunction> FunctionList;
        public string BaseClass;
        public CSharpFunction DefaultConstruct;

        public CSharpClass(string name)
        {
            this.Name = name;
            FunctionList = new List<CSharpFunction>();
            FieldList = new List<CSharpClassField>();
            AttributeList = new List<CSharpClassAttribute>();
            this.AccessAuth = AccessAuthority.PRIVATE;
            this.BaseClass = "";
        }

        public void AddDefaultConstruct()
        {
            DefaultConstruct = CSharpFunction.GetConstruction(this.Name);
            FunctionList.Add(DefaultConstruct);
        }

        public void AddFunction(CSharpFunction fun)
        {
            FunctionList.Add(fun);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string indent="    ";
            sb.Append(AccessAuth.ToString().ToLower() + " ");
            if (this.IsStatic) sb.Append("static ");
            if (this.IsAbstract) sb.Append("abstract ");
            if (this.IsPartial) sb.Append("partial ");
            sb.Append("class "+this.Name);
            if (this.BaseClass != "") sb.AppendLine(":" + BaseClass);
            else sb.AppendLine();
            sb.AppendLine("{");

            foreach (CSharpClassField field in FieldList)
            {
                sb.AppendLine(indent+field.ToString());
            }
            foreach (CSharpClassAttribute attribute in AttributeList)
            {
                string[] lines = Util.SplitLine(attribute.ToString());
                for (int i = 0; i < lines.Length; i++)
                {
                    sb.AppendLine(indent + lines[i]); ;
                }
            }
            foreach (CSharpFunction function in FunctionList)
            {
                string[] lines = Util.SplitLine(function.ToString());
                for (int i = 0; i < lines.Length; i++)
                {
                    sb.AppendLine(indent + lines[i]); ;
                }
            }

            sb.Append("}");
            return sb.ToString();
        }
    }
  
    public class CSharpFunction
    {
        public string Name;
        public AccessAuthority AccessAuth;
        public bool IsStatic;
        public bool IsContruction;
        public bool IsOverride;
        public bool IsAbstract;
        public bool IsNew;
        public string ReturnType;
        public Dictionary<string, string> Parameters;
        public List<string> BaseParameters;

        public string this[string key]
        {
            get { return Parameters[key]; }
            set { Parameters[key] = value; }
        }

        public StringBuilder Content;
        public CSharpFunction(string name)
        {
            this.Name = name;
            Parameters = new Dictionary<string, string>();
            BaseParameters = new List<string>();
            Content = new StringBuilder();
        }

        public void Append(string context)
        {
            this.Content.Append(context);
        }
        public static CSharpFunction GetConstruction(string name)
        {
            CSharpFunction fun = new CSharpFunction(name);
            fun.IsContruction = true;
            fun.AccessAuth = AccessAuthority.PUBLIC;
            return fun;
        }
        public override string ToString()
        {
            string indent = "    ";
            StringBuilder sb = new StringBuilder();
            sb.Append(AccessAuth.ToString().ToLower()+" ");
            if(!this.IsContruction)
            {
                if (this.IsStatic) sb.Append("static ");
                if (this.IsOverride) sb.Append("override ");
                if (this.IsNew) sb.Append("new ");
                sb.Append(ReturnType+" ");
            }
            sb.Append(Name + "(");
            string param = "";
            foreach (string key in Parameters.Keys)
            {
                param += (key + " " + Parameters[key] + ",");
            }
            if (param != "") sb.Append(param.Substring(0, param.Length - 1));
            sb.Append(")");
            if (this.IsContruction && this.BaseParameters.Count != 0)
            {
                sb.Append(":base(");
                for (int i = 0; i < BaseParameters.Count - 1; i++)
                    sb.Append(BaseParameters[i] + ",");
                sb.AppendLine(BaseParameters[BaseParameters.Count - 1] + ")");
            }
            else sb.AppendLine();
            sb.AppendLine("{");
            string[] lines = Util.SplitLine(Content.ToString());
            for (int i = 0; i < lines.Length; i++)
            {
                sb.AppendLine(indent + lines[i]);
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class CSharpClassField
    {
        public AccessAuthority AccessAuth;
        public string Name;
        public string Type;
        public string InitStatment;
        public bool IsStatic;
        public CSharpClassField(string type, string name)
        {
            this.AccessAuth = AccessAuthority.PRIVATE;
            this.Type = type;
            this.Name = name;
            InitStatment = "";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(AccessAuth.ToString().ToLower() + " " + this.Type + " " + this.Name);
            if (InitStatment != "") sb.Append("="+InitStatment);
            sb.Append(";");
            return sb.ToString();
        }
    }

    public class CSharpClassAttribute 
    {
        public AccessAuthority AccessAuth;
        public string Name;
        public string Type;
        public bool IsOverride;
        //public string GetType;
        //public string SetType;
        public StringBuilder GetStatment;
        public StringBuilder SetStatment;

        public CSharpClassAttribute(string name)
        {
            this.Name = name;
            GetStatment = new StringBuilder();
            SetStatment = new StringBuilder();
        }
        /// <summary>
        /// 由字段构造的默认属性
        /// </summary>
        /// <param name="fieldName"></param>
        public CSharpClassAttribute(string name,CSharpClassField field)
        {
            this.AccessAuth = AccessAuthority.PUBLIC;
            this.Name = name;
            this.Type = field.Type;
            GetStatment = new StringBuilder("    return " + field.Name + ";");
            SetStatment = new StringBuilder("    " + field.Name + "=value;");
        }
        public override string ToString()
        {
            string indenet = "    ";
            StringBuilder sb = new StringBuilder();
            sb.Append(AccessAuth.ToString().ToLower()+" ");
            if (this.IsOverride) sb.Append("override ");
            sb.AppendLine(this.Type + " " + this.Name);
            sb.AppendLine("{");
            if (this.GetStatment.Length != 0)
            {
                sb.AppendLine(indenet + "get");//构造get部分
                sb.AppendLine(indenet + "{");
                string[] getLines = Util.SplitLine(this.GetStatment.ToString());
                foreach (string line in getLines)
                {
                    sb.AppendLine(indenet + line);
                }
                sb.AppendLine(indenet + "}");
            }
            else if (this.SetStatment.Length != 0)
            {
                sb.AppendLine(indenet + "set");
                sb.AppendLine(indenet + "{");
                string[] setLines = Util.SplitLine(this.SetStatment.ToString());
                foreach (string line in setLines)
                {
                    sb.AppendLine(indenet + line);
                }
                sb.AppendLine(indenet + this.SetStatment);
                sb.AppendLine(indenet + "}");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public enum AccessAuthority
    {
        PUBLIC,
        PROTECTED,
        PRIVATE
    }
}
