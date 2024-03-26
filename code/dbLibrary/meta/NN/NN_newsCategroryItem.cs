using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{

    [MetadataType(typeof(metaData))]
    public partial class NN_newsCategroryItem
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }

            [Display(Name ="标签号")]
            public string newsCateGoryID { get; set; }
            [Display(Name = "新闻号")]
            public string newsID { get; set; }
        }

    }
}
