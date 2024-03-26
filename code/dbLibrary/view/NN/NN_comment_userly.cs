using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_comment_userly
    {
        public int index { get; set; } //索引

        public string commentID { get; set; }
        public string Name { get; set; }

        public byte[] proFile { get; set; }

        public long rowNum { get; set; }
        public string rowID { get; set; }
        public string userID { get; set; }
        public string newsID { get; set; }
        public string commentContent { get; set; }
        public string fatherCommentID { get; set; }

        public string fatherCommentUserName { get; set; }
        public int status { get; set; }
        public Nullable<System.DateTime> commentTime { get; set; }

        public NN_comment_userly() { }

        ~NN_comment_userly() { }
    }
}
