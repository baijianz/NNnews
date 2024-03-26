using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class NN_newsCategory
    {
        private class metaData
        {
            
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "标签ID")]
            public string newsCategoryID { get; set; }
            [Display(Name = "标签名称")]
            [Required(ErrorMessage ="必填")]
            public string newsCategoryName { get; set; }
            [Display(Name = "标签状态")]
            public string status { get; set; }
            [Display(Name = "备注")]
            public string remark { get; set; }
        }

    }
}
