using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCML.IDE.WebUCML
{
    public class UcmlBusiCompPropSet
    {
        public string OID;
        public string LinkOID;
        public string LinkPOID;
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

        //Where条件列
        public List<BCCondiColumn> CondiColumns;

        //自定义代码
        public string OnCalculateScript;
        public string OnRecordChangeScript;
        public string OnBeforeInsertScript;
        public string OnAfterInsertScript;
        public string InitScript;
        public string BeforeUpdateScript;
        public string AfterUpdateScript;
        public string InitCSharpCode;
        public string BeforeSubmitCSharpCode;
        public string AfterSubmitCSharpCode;

        public bool ChangeOnlyOwnerBy;
        public List<UcmlBusiCompPropSet> ChildBC;
        public List<BusiCompColumn> Columns;

        public UcmlBusiCompPropSet()
        {
            Columns = new List<BusiCompColumn>();
            ChildBC = new List<UcmlBusiCompPropSet>();
        }
    }

    public class BCLinkProperty
    {
        public string AppletName;
        public string Caption;
        public int RunMode;
        public string srcFieldName;
        public string destFieldName;
        public string condiFieldName;
        public string Value;
        public int DropDownWidth;
        public int DropDownHeight;
        public string QueryFieldName;
        public bool fQuickQuery;
        public bool fDropDownMode;

        public BCLinkProperty()
        {
            AppletName = "";
            Caption = "";
            RunMode = 0;
            srcFieldName = "";
            destFieldName = "";
            condiFieldName = "";
            Value = "";
            DropDownWidth = 0;
            DropDownHeight = 0;
            QueryFieldName = "";
        }
    }


    public class BusiCompColumn
    {
        public string OID;
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

        //链接业务组件
        public BCLinkProperty BCLink;

        //自定义代码
        public string OnFieldChangeScript;

        public BusiCompColumn()
        {
            BCLink = new BCLinkProperty();
        }
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

    public class BCCondiColumn
    {
        public string FieldName;
        public int FieldType;

        public int Operation
        {
            set
            {
                if (value == 0) this.OperationIndent = "=";
                else if (value == 1) this.OperationIndent = ">=";
                else if (value == 2) this.OperationIndent = ">";
                else if (value == 3) this.OperationIndent = "<=";
                else if (value == 4) this.OperationIndent = "<";
                else if (value == 5) this.OperationIndent = "<>";
                else if (value == 6) this.OperationIndent = "Like";
            }
        }

        public string OperationIndent;

        public int Logic
        {
            set
            {
                if (value == 0) this.LogicConnect = "AND";
                else if (value == 1) this.LogicConnect = "OR";
            }
        }
        public string LogicConnect;
        public string CondiFieldValue;
        public string LeftBracket;
        public string RightBracket;
        public bool fCondiField;
        public bool fIsFunctionValue;
        public string valueFunction;
        public bool fFreeWhere;
        public string SQL;
        public int Pos;
    }
}
