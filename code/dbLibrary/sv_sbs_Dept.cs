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
    
    public partial class sv_sbs_Dept
    {
        public long rowNum { get; set; }
        public string rowID { get; set; }
        public string deptCode { get; set; }
        public string deptName { get; set; }
        public string deptType { get; set; }
        public string upDeptCode { get; set; }
        public string orgCode { get; set; }
        public string sourceFrom { get; set; }
        public Nullable<System.DateTime> importDate { get; set; }
        public string remark { get; set; }
        public string upDeptName { get; set; }
        public string orgName { get; set; }
    }
}