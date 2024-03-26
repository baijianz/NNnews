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
    
    public partial class NN_newsItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NN_newsItem()
        {
            this.NN_comment = new HashSet<NN_comment>();
            this.NN_like = new HashSet<NN_like>();
            this.NN_report = new HashSet<NN_report>();
            this.NN_star = new HashSet<NN_star>();
        }
    
        public long rowNum { get; set; }
        public string rowID { get; set; }
        public string newsID { get; set; }
        public string newsTitle { get; set; }
        public string userID { get; set; }
        public long readQuantity { get; set; }
        public System.DateTime publishDateTime { get; set; }
        public string content { get; set; }
        public int status { get; set; }
        public byte[] coverPic { get; set; }
        public string describe { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NN_comment> NN_comment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NN_like> NN_like { get; set; }
        public virtual NN_user NN_user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NN_report> NN_report { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NN_star> NN_star { get; set; }
    }
}
