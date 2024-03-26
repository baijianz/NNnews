using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class NN_report
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }

            [Display(Name ="用户ID")]
            public string userID { get; set; }
            [Display(Name = "举报ID")]
            public string reportID { get; set; }
            [Display(Name = "举报时间")]
            public System.DateTime reportTimeDateTime { get; set; }
            [Display(Name = "举报理由")]
            public string reason { get; set; }
            [Display(Name = "分类")]
            public Nullable<int> category { get; set; }
            [Display(Name = "处理状态")]
            public int disposeStatus { get; set; }
            [Display(Name = "新闻ID")]
            public string newsID { get; set; }
            [Display(Name = "评论ID")]
            public string commentID { get; set; }
        }

    }
}
