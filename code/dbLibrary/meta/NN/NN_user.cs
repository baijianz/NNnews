using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class NN_user
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }

            [Display(Name ="用户编号")]
            
            public string userID { get; set; }

            [Display(Name = "用户昵称")]
            [Required(ErrorMessage = "必填")]
            public string userName { get; set; }
            
            
            [Display(Name = "头像")]
            public byte[] profile { get; set; }

            [Display(Name = "注册时间")]
            public System.DateTime registerDateTime { get; set; }
            [Display(Name = "密码")]
            [Required(ErrorMessage ="必填")]
            public string password { get; set; }
            [Display(Name ="性别")]
            public bool gender { get; set; }
            [Display(Name ="出生日期")]
            public System.DateTime birth { get; set; }
            [Display(Name = "类型")]

            public int type { get; set; }
            [Display(Name = "新闻机构ID")]
            public string newsAgencyID { get; set; }
            [Display(Name = "状态")]
            public bool status { get; set; }
            [Display(Name ="备注")]
            public string remark { get; set; }
        }

    }
}
