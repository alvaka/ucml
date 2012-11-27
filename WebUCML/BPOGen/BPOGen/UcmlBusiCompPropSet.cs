using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlBusiCompPropSet
    {
        public int OID;
        public string Name;
        public string Caption;
        public string TableName;
        public bool IsRootBC;
        public bool fIDENTITYKey;
        public bool AllowModifyJION;
        public bool fHaveUCMLKey;
        public string PrimaryKey;
        public string LinkKeyName;
        public string PK_COLUMN_NAME;

        public bool ChangeOnlyOwnerBy;

        public List<BusiCompColumn> Columns;

        public UcmlBusiCompPropSet()
        {
            Columns = new List<BusiCompColumn>();
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
        public int    FieldType = 0;
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
    }
}
