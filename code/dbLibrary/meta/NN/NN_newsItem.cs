using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class NN_newsItem
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name ="新闻ID")]
            public string newsID { get; set; }
            [Required(ErrorMessage ="必填")]
            [Display(Name ="新闻标题")]
            public string newsTitle { get; set; }
            [Display(Name = "作者ID")]
            public string userID { get; set; }
            [Display(Name = "阅读量")]
            public long readQuantity { get; set; }
            [Display(Name = "发布时间")]
            public System.DateTime publishDateTime { get; set; }
            [Display(Name = "新闻内容")]
            [Required(ErrorMessage ="内容不能为空")]
            public string content { get; set; }
            [Display(Name ="状态")]
            public int status { get; set; }
            [Display(Name = "新闻封面")]
            public byte[] coverPic { get; set; }
            [Display(Name = "新闻描述")]
            public string describe { get; set; }
        }

    }
}
