using System;
using System.Collections.Generic;
using System.Text;

namespace SuperLiner.Core
{
    public class Constants
    {
        public const string Slaver_Port_Key_Template = "__slaver_info_{0}_port__";
        public const string Slaver_Secure_Key_Template = "__slaver_info_{0}_secure__";
        public const string Default_Timeline= "__default_timeline__";
        public const string Current_Timeline_Key = "__current_timeline__";
        public const string Default_Stop_Timeline = "__default_stop_timeline__";
        public const string Stop_Timeline_Key = "__stop_timeline__";
        public const string Timeline_List_Key = "__timeline__";
        public const string Main_Func_Key = "__main__";
        public const string System_Register_Key = "__system_reg__";
        public const string Runtime_Register_key = "__script_reg__";
        public const string Http_Header_Key = "__http_headers__";
        public const string Debug_Stream_Key = "__debug_stream__";
        public const string If_Temp_Scripts = "__if_temp_scripts__";
        public class Op
        {
            public const string Op_Get_From_Register = "&";
            public const string Op_Comment_It = "#";
            public const string Op_Append_Script = "appendscript";
            public const string Op_Quote_Timeline = "--";
            public const string Op_Start_Define_Func = "func";
            public const string Op_End_Define_Func = "endfunc";
            
        }
    }
}
