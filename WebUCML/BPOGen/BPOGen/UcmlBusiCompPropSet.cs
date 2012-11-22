using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlBusiCompPropSet
    {
        public string Name;
        public string Caption;
        public string TableName;
        public List<BusiCompColumn> Columns;

        public UcmlBusiCompPropSet()
        {
            Columns = new List<BusiCompColumn>();
        }
    }

    public class BusiCompColumn
    {
        public string FieldName ="";
        public bool   fDisplay = false; 
        public bool   fCanModify = false; 
        public int    Pos = 0;
        public int    Width = 0;
        public int    FieldType = 46;
        public int    StatMode = 0;
        public int    SortMode = 0;
        public bool   fGroupBy = false; 
        public string Caption = "";
        public string EditType = "";
        public string CodeTable = "";
        public bool   fUseCodeTable = false;
        public bool   fAllowNull = false;
        public int    CurrentPos = 0;
        public string DefaultValue = "";
        public bool   fAllowPick = false;
        public string ForeignKeyField = "";
        public string LookupKeyField = "";
        public string LookupDataSet = "";
        public string LookupResultField = "";
        public bool   fForeignKey = false;
        public int    FieldKind = 0;
        public string CustomSQLColumn = "";
        public int    ExcelColNo = 0;
    }
}
