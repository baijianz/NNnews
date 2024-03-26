using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_newsItem : rui.pagerBase
    {

        public string newsID { get; set; }
        public string newsTitle { get; set; }

        public string userID { get; set; }
        public string userName { get; set; }

        public long readQuantity { get; set; }

        public string publishDateTimeStart { get; set; }
        public string publishDateTimeEND { get; set; }

        public override void Search()
        {
            this.ResourceCode = "NN_newsItem";
            this.keyField = "newsID";
            this.getPageConfig();
            string querySql = @"SELECT * FROM NN_newsItem JOIN NN_user on NN_user.userID = NN_newsItem.userID WHERE  1=1";
            //在这里个性化查询优化 searchTbx是模糊查询
            querySql += rui.dbTools.searchDdl("newsID", this.newsID, cmdPara);       //新闻ID搜索
            querySql += rui.dbTools.searchTbx("newsTitle", this.newsTitle, cmdPara); //新闻名搜索 
            querySql += rui.dbTools.searchTbx("userName", this.userName, cmdPara);   //作者名搜索
            querySql += rui.dbTools.searchDdl("userID", this.userID, cmdPara);       //作者ID搜索
            querySql += rui.dbTools.searchDate("publishDateTime", publishDateTimeStart, publishDateTimeEND, cmdPara);
            this.getPageConfig();
            if (this.readQuantity != 1) //在这里设置一个排序方式
            {
                rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("readQuantity", "asc"), this);
                ph.Execute(this.PageSize, this.PageIndex, this);
                this.dataTable = ph.Result;
                //获取要展示的列配置
                this.showColumn = this.getShowColumn();
            }
            else
            {
                rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("readQuantity", "desc"), this);
                ph.Execute(this.PageSize, this.PageIndex, this);
                this.dataTable = ph.Result;
                //获取要展示的列配置
                this.showColumn = this.getShowColumn();
            }
        }
    }
}
