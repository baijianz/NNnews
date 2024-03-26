using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class NN_apply
    {
        private class metaData
        {
            
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "用户ID")]
            public string userID { get; set; }
            [Display(Name = "申请类型")]
            public int type { get; set; }
            [Display(Name = "官方机构ID")]
            public string newsAgencyID { get; set; }
            
            [Display(Name = "申请时间")]
            public System.DateTime applicationDateTime { get; set; }
            [Display(Name = "处理时间")]
            public System.DateTime disposeTime { get; set; }
            
            [Display(Name = "状态")]
            public int status { get; set; }
            
            [Display(Name ="证明资料")]
            public string data { get; set; }
            [Display(Name ="申请号")]
            public string applyID { get; set; }
        }

    }
}
