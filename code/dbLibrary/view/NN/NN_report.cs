using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.view
{
    public class NN_report : rui.pagerBase
    {
        public string userID { get; set; }  //举报人ID

        public string userName { get; set; } //举报人姓名
        public string reportID { get; set; } //举报ID
        
        public string reportTimeDateStart { get; set; }  //举报时间
        public string reportTimeDateEnd { get; set; }

        public string category { get; set; }   //0 为新闻 1 为评论

        public string disposeStatus { get; set; }  //(0 未处理1已经处理)

        public string newsID { get; set; } //当category为0该字段有效

        public string newsTitle { get; set; } // 新闻ID

        public string commentID { get; set; } //当category为1该字段有效



        public override void Search()
        {
            this.ResourceCode = "NN_report";
            this.keyField = "reportID";
            this.getPageConfig();

            string querySql = @"SELECT NN_report.*, NN_user.userName, NN_newsItem.newsTitle  FROM NN_report inner join NN_user on NN_user.userID = NN_report.userID  left join NN_newsItem on NN_newsItem.newsID = NN_report.newsID WHERE  1=1";
            querySql += rui.dbTools.searchDdl("NN_report.userID", this.userID);           //用户人ID搜索
            querySql += rui.dbTools.searchDdl("reportID", this.reportID, cmdPara);       //举报ID搜索

            querySql += rui.dbTools.searchDate("reportTimeDateTime", reportTimeDateStart, reportTimeDateEnd, cmdPara);  //举报时间搜索
            querySql += rui.dbTools.searchDdl("category", this.category, cmdPara);            //举报类型搜索  0
            querySql += rui.dbTools.searchDdl("disposeStatus", this.disposeStatus, cmdPara);        //处理状态搜索

            querySql += rui.dbTools.searchTbx("userName", this.userName, cmdPara);            //举报人姓名搜索
            querySql += rui.dbTools.searchTbx("newsTitle", this.newsTitle, cmdPara);          //举报新闻标题

            
            querySql += rui.dbTools.searchDdl("NN_newsItem.newsID", this.newsID);            //当category为0该字段有效
            querySql += rui.dbTools.searchDdl("commentID", this.commentID);       //当category为0该字段有效

            this.getPageConfig();
            rui.pagerHelper ph = new rui.pagerHelper(querySql, this.getOrderSql("rowID", "asc"), this);
            ph.Execute(this.PageSize, this.PageIndex, this);
            this.dataTable = ph.Result;
            //获取要展示的列配置
            this.showColumn = this.getShowColumn();
        }
    }
}
