using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class NN_comment
    {
        private class metaData
        {

            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "评论ID")]
            public string commentID { get; set; }
            [Display(Name = "用户ID")]
            public string userID { get; set; }
            [Display(Name = "新闻ID")]
            public string newsID { get; set; }
            [Display(Name = "评论内容")]
            [Required(ErrorMessage ="评论内容不能为空")]
            public string commentContent { get; set; }
            [Display(Name = "父节点")]
            public string fatherCommentID { get; set; }
            [Display(Name = "状态")]
            public int status { get; set; }
            [Display(Name = "评论时间")]
            public Nullable<System.DateTime> commentTime { get; set; }
        }

    }
}
