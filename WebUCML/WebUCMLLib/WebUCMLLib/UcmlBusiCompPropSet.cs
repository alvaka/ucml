using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlBusiCompPropSet
    {
        public int OID;
        public int LinkOID;
        public int LinkPOID;
        public string Name;
        public string Caption;
        public string TableName;
        public bool IsRootBC;
        public bool IsActor;
        public bool fIDENTITYKey;
        public bool AllowModifyJION;
        public bool fHaveUCMLKey;
        public int PageCount;
        public string PrimaryKey;
        public string LinkKeyName;
        public int LinkKeyType;
        public string PK_COLUMN_NAME;

        public bool ChangeOnlyOwnerBy;
        public List<UcmlBusiCompPropSet> ChildBC;
        public List<BusiCompColumn> Columns;

        public UcmlBusiCompPropSet()
        {
            Columns = new List<BusiCompColumn>();
            ChildBC = new List<UcmlBusiCompPropSet>();
        }
    }

    public class BusiCompColumn
    {
        public string FieldName ="";
        public int FieldLength;
        public int DecLength;
        public bool   fDisplay = false; 
        public bool   fCanModify = false; 
        public int    Pos = 0;
        public int    Width = 0;
        private int    _FieldType = 0;
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
        public string RoleTable;
        public int    ExcelColNo = 0;
        public string QueryBPOID;
        public bool IsMultiValueField;
        public string MultiValueTable;
        public bool fFunctionInitValue;
        public string InitValueFunc;

        public int FieldType
        {
            get { return _FieldType; }
            set 
            {
                _FieldType = value;
                switch (value)
                {
                    case 0: EditType = "INPUT"; break;
                    case 2: EditType = "INPUT"; break;
                    case 4: EditType = "INPUT"; break;
                    case 6: EditType = "TEXTAREA"; break;
                    case 8: EditType = "INPUT"; break;
                    case 10: EditType = "INPUT"; break;
                    case 11: EditType = "INPUT"; break;
                    case 12: EditType = "INPUT"; break;
                    case 13: EditType = "INPUT"; break;
                    case 14: EditType = "INPUT"; break;
                    case 15: EditType = "INPUT"; break;
                    case 17: EditType = "INPUT"; break;
                    case 20: EditType = "CHECKBOX"; break;
                    case 31: EditType = "INPUT"; break;
                    case 32: EditType = "INPUT"; break;
                    case 33: EditType = "INPUT"; break;
                    case 40: EditType = "IMG"; break;
                    case 41: EditType = "WORD"; break;
                    case 42: EditType = "EXCEL"; break;
                    case 43: EditType = "HTML"; break;
                    case 45: EditType = "INPUT"; break;
                    case 46: EditType = "INPUT"; break;
                }
            }
        }
    }
}
