//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace db
{
    using System;
    using System.Collections.Generic;
    
    public partial class NN_apply
    {
        public long rowNum { get; set; }
        public string rowID { get; set; }
        public string userID { get; set; }
        public int type { get; set; }
        public string newsAgencyID { get; set; }
        public System.DateTime applicationDateTime { get; set; }
        public Nullable<System.DateTime> disposeTime { get; set; }
        public int status { get; set; }
        public string data { get; set; }
        public string applyID { get; set; }
    
        public virtual NN_user NN_user { get; set; }
    }
}
