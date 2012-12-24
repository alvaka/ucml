using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class BpoPropertySet
    {
        public string GUID;
        public string Name;
        public string Capiton;
        public bool fInFlow;
        public bool fSystemBPO;
        public bool fXHTMLForm;
        public bool fHavePageNavi;
        public bool fRegisterBPO;
        public bool fMutiLangugeSupport;
        public bool fEnableConfig ;
        public bool fUseSkin;
        public string SkinSrc;
        public string BodyStyle;
        //Javascript Code
        public string InitScript;
        public string BeforeSubmitScript;
        public string AfterSubmitScript;
        public string RefJsLibrary;

        //C# Code
        public string InitCSharpCode;
        public string BeforeSubmitCSharpCode;
        public string AfterSubmitCSharpCode;
        public string RefCSharpLibrary;

        public List<CSharpFunction> CSharpFuncs;
        public List<JsFunction> JsFuncs;
    }

}
