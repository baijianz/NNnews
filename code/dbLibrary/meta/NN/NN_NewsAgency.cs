using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class NN_NewsAgency
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "机构ID")]
            public string newsAgencyID { get; set; }
            [Display(Name = "机构名称")]
            [Required(ErrorMessage ="必填")]
            public string newsAgencyName { get; set; }
            [Display(Name = "状态")]
            public int status { get; set; }
            [Display(Name = "备注")]
            public string remark { get; set; }
        }

    }
}
