using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_newsCategory : rui.pagerBase
    {
        public string newsCategoryID { get; set; }
        public string newsCategoryName { get; set; }

        public string status { get; set; }
        public override void Search()
        {
            this.ResourceCode = "NN_newsCategory";
            this.keyField = "newsCategoryID";
            this.getPageConfig();
            string querySql = @"SELECT * FROM NN_newsCategory WHERE  1=1";
            querySql += rui.dbTools.searchTbx("newsCategoryID", this.newsCategoryID, this.cmdPara);
            querySql += rui.dbTools.searchTbx("newsCategoryName", this.newsCategoryName, this.cmdPara);
            querySql += rui.dbTools.searchDdl("status", this.status, this.cmdPara);
            //利用搜索语句获取数据
            this.getPageConfig();
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("newsCategoryID", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();
        }
    }
}
